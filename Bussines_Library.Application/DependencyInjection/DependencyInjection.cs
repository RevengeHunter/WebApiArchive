using Bussines_Library.Application.Features.Authentication.Services;
using Bussines_Library.Application.Features.Authors.Services;
using Bussines_Library.Application.Features.Books.Services;
using Bussines_Library.Application.Features.Loans.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bussines_Library.Application.DependencyInjection
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services, handlers, and other dependencies here
            // Example: services.AddScoped<IBookRepository, BookRepository>();
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            return services;

        }
    }
}
