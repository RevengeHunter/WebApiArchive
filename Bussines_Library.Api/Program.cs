using Bussines_Library.Api.Constants;
using Bussines_Library.Api.DependencyInjection;
using Bussines_Library.Api.Middleware;
using Bussines_Library.Application.DependencyInjection;
using Bussines_Library.Infraestructure.DepencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseResponseCompression();
app.UseCors(ApiPolicies.CorsPolicy);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()  || app.Configuration.GetValue<bool>("OpenApi:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("v1/swagger.json", "Bussines_Library.Api v1"));
}

app.MapControllerRoute(
    name: "default",
    pattern: "api/v{version:apiVersion}/{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapControllers();
app.Run();

public partial class Program;