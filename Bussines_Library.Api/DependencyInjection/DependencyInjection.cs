using Asp.Versioning;
using Bussines_Library.Api.Constants;
using Bussines_Library.Api.ExceptionHandling;
using Bussines_Library.Api.Middleware;
using Bussines_Library.Api.OpenApi;
using Bussines_Library.Api.Options;
using Bussines_Library.Application.Abstractions.CurrentUser;
using Bussines_Library.Infraestructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace Bussines_Library.Api.DependencyInjection
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Configurar controller
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddProblemDetails(options => options.CustomizeProblemDetails = context => context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier);

            //configuracion del CORS 
            services.AddOptions<CorsOptions>()
           .Bind(configuration.GetSection(CorsOptions.SectionName))
           .Validate(options => options.AllowedOrigins.All(origin => Uri.TryCreate(origin, UriKind.Absolute, out _)), "CORS origins must be absolute URLs.")
           .ValidateOnStart();

            //configuracion del JWT
            services.AddOptions<JWTOptions>()
                .Bind(configuration.GetSection(JWTOptions.SectionName))
                .Validate(options => !options.Enabled || (!string.IsNullOrWhiteSpace(options.Issuer) && !string.IsNullOrWhiteSpace(options.Audience) && options.Key.Length >= 32), "")
                .ValidateOnStart();

            //configuracion del rate limit
            services.AddOptions<RateLimitOptions>()
            .Bind(configuration.GetSection(RateLimitOptions.SectionName))
            .Validate(options => options.Limit > 0 && options.WindowInSeconds > 0, "Rate limit options must be positive.")
            .ValidateOnStart();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddOpenApiDocumentation();
            services.AddHealthChecks();
            services.AddResponseCompression();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
            services.ConfigureCors(configuration);
            services.ConfigureRateLimiting(configuration);
            services.ConfigureJwt(configuration);
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Books.Read", policy => policy.RequireAuthenticatedUser());
                options.AddPolicy("Books.Write", policy => policy.RequireRole("Administrator"));
            });
            return services;
        }

        private static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var cors = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>() ?? new CorsOptions();
            services.AddCors(options => options.AddPolicy(ApiPolicies.CorsPolicy, policy =>
            {
                var origins = cors.AllowedOrigins.Length == 0 ? ["https://localhost:3000"] : cors.AllowedOrigins;
                policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
            }));
        }

        private static void ConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(RateLimitOptions.SectionName).Get<RateLimitOptions>() ?? new RateLimitOptions();
            services.AddRateLimiter(rateLimiter =>
            {
                rateLimiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                rateLimiter.OnRejected = (context, _) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers["Retry-After"] = ((int)retryAfter.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }

                    return ValueTask.CompletedTask;
                };
                rateLimiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ => RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = options.Limit,
                    Window = TimeSpan.FromSeconds(options.WindowInSeconds),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));
            });
        }

        private static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwt = configuration.GetSection(JWTOptions.SectionName).Get<JWTOptions>() ?? new JWTOptions();
            var builder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            builder.AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwt.Enabled,//validamos que el token haya sido emitido por el api
                    ValidateAudience = jwt.Enabled,// valida que el token este destinado para tu aplicacion cliente.
                    ValidateIssuerSigningKey = jwt.Enabled,//Valida que la firma sea valida
                    ValidateLifetime = jwt.Enabled,//valida que no haya expirado el tiempo
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = string.IsNullOrWhiteSpace(jwt.Key) ? null : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    NameClaimType = ClaimTypes.Name,//agregamos el nombre del usuario al token
                    RoleClaimType = ClaimTypes.Role, // agregamos los roles del usuario al token
                    ClockSkew = TimeSpan.Zero// validamos que los 5 minutos de tolerancia para reactivar el jwt no se den.
                };
            });
        }

    }
}
