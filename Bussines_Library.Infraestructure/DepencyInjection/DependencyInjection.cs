using Bussines_Library.Application.Abstractions.Authentication;
using Bussines_Library.Application.Abstractions.Clock;
using Bussines_Library.Application.Abstractions.Data;
using Bussines_Library.Application.Abstractions.Idempotency;
using Bussines_Library.Application.Features.Authors.Repositories;
using Bussines_Library.Application.Features.Books.Repositories;
using Bussines_Library.Application.Features.Loans.Repositories;
using Bussines_Library.Infraestructure.Authentication;
using Bussines_Library.Infraestructure.Persistence;
using Bussines_Library.Infraestructure.Persistence.Repositories;
using Bussines_Library.Infraestructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bussines_Library.Infraestructure.DepencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var useInMemoryDatabase = configuration.GetValue<bool>("Persistence:UseInMemoryDatabase");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (useInMemoryDatabase)
                {
                    options.UseInMemoryDatabase(configuration.GetValue<string>("Persistence:InMemoryDatabaseName") ?? "BussinesLibraryDb");
                    return;
                }
                else
                {
                    var connectionString = configuration.GetConnectionString("SQLSERVER_CONECTION");
                    options.UseSqlServer(connectionString);
                }

            });

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IIdempotencyService, NoOpIdempotencyService>();
            services.AddScoped<IJwtGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}
