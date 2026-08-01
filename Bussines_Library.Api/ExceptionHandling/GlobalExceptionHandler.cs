using Bussines_Library.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bussines_Library.Api.ExceptionHandling
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> logger;
        private readonly IProblemDetailsService problemDetailsService;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
        {
            this.logger = logger;
            this.problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title, detail, code) = exception switch
            {
                DomainException domainException => (StatusCodes.Status422UnprocessableEntity, "Domain rule violated", domainException.Message, domainException.Code),
                FluentValidation.ValidationException validationException => (StatusCodes.Status400BadRequest, "Validation failed", validationException.Message, "Validation.Failed"),
                _ => (StatusCodes.Status500InternalServerError, "Unexpected error", "An unexpected error occurred.", "Unexpected.Failure")
            };

            logger.LogError(exception, "Unhandled exception {ErrorCode} for {Method} {Path}", code, httpContext.Request.Method, httpContext.Request.Path);
            httpContext.Response.StatusCode = status;

            var problemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Type = $"https://api.example.com/errors/{code.ToLowerInvariant().Replace('.', '-')}",
                Instance = httpContext.Request.Path
            };
            problemDetails.Extensions["code"] = code;
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            return await problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails
                }
            );
        }
    }
}
