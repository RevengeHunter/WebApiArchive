using Bussines_Library.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bussines_Library.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        #region Async Conversion Methods

        /// <summary>
        /// Convierte un Result a IActionResult de forma asincrónica
        /// </summary>
        protected Task<IActionResult> FromResultAsync(Result result)
        {
            var response = result.IsSuccess
                ? (IActionResult)NoContentResponse()
                : ToProblem(result);

            return Task.FromResult(response);
        }

        /// <summary>
        /// Convierte un Result<TResponse> a ActionResult<TResponse> de forma asincrónica
        /// </summary>
        protected Task<ActionResult<TResponse>> FromResultAsync<TResponse>(Result<TResponse> result) where TResponse : class
        {
            ActionResult<TResponse> response = FromResultTyped(result);
            return Task.FromResult(response);
        }

        /// <summary>
        /// Convierte un Result<TResponse> a IActionResult de forma asincrónica (ignora la parte genérica)
        /// </summary>
        protected Task<IActionResult> FromResultIActionAsync<TResponse>(Result<TResponse> result) where TResponse : class
        {
            var response = result.IsSuccess
                ? (IActionResult)NoContentResponse()
                : ToProblem(result);

            return Task.FromResult(response);
        }

        #endregion

        #region Sync Conversion Methods (Deprecated - Use Async versions)

        /// <summary>
        /// [DEPRECATED] Usa FromResultAsync en su lugar
        /// Versión sincrónica - Convierte un Result a IActionResult
        /// </summary>
        protected IActionResult FromResult(Result result)
        {
            if (result.IsSuccess)
                return NoContentResponse();

            return ToProblem(result) as IActionResult ?? NoContentResponse();
        }

        /// <summary>
        /// [DEPRECATED] Usa FromResultIActionAsync en su lugar
        /// Versión sincrónica - Convierte un Result<TResponse> a IActionResult (ignora el genérico)
        /// </summary>
        protected IActionResult FromResult<TResponse>(Result<TResponse> result) where TResponse : class
        {
            if (result.IsSuccess)
                return NoContentResponse();

            return ToProblem(result) as IActionResult ?? NoContentResponse();
        }

        /// <summary>
        /// [DEPRECATED] Usa FromResultAsync<TResponse> en su lugar
        /// Versión sincrónica - Convierte un Result<TResponse> a ActionResult<TResponse> con tipado genérico
        /// </summary>
        protected ActionResult<TResponse> FromResultTyped<TResponse>(Result<TResponse> result) where TResponse : class
        {
            if (result.IsSuccess && result.Value is not null)
                return OkResponse(result.Value);

            var problemActionResult = ToProblem(result);
            return new ActionResult<TResponse>(problemActionResult);
        }

        #endregion

        #region Response Methods - All Async Compatible

        /// <summary>
        /// Respuesta OK con datos (200)
        /// </summary>
        protected Task<ActionResult<TResponse>> OkResponseAsync<TResponse>(TResponse data) where TResponse : class
            => Task.FromResult<ActionResult<TResponse>>(Ok(data));

        /// <summary>
        /// Respuesta OK con datos - Sincrónica (200)
        /// </summary>
        protected ActionResult<TResponse> OkResponse<TResponse>(TResponse data) where TResponse : class
            => Ok(data);

        /// <summary>
        /// Respuesta 201 Created - Asincrónica
        /// </summary>
        protected Task<ActionResult<TResponse>> CreatedResponseAsync<TResponse>(string actionName, object routeValues, TResponse data) where TResponse : class
            => Task.FromResult<ActionResult<TResponse>>(CreatedAtAction(actionName, routeValues, data));

        /// <summary>
        /// Respuesta 201 Created - Sincrónica
        /// </summary>
        protected ActionResult<TResponse> CreatedResponse<TResponse>(string actionName, object routeValues, TResponse data) where TResponse : class
            => CreatedAtAction(actionName, routeValues, data);

        /// <summary>
        /// Respuesta 204 No Content - Asincrónica
        /// </summary>
        protected Task<IActionResult> NoContentResponseAsync()
            => Task.FromResult<IActionResult>(NoContent());

        /// <summary>
        /// Respuesta 204 No Content - Sincrónica
        /// </summary>
        protected IActionResult NoContentResponse()
            => NoContent();

        /// <summary>
        /// Respuesta 400 Bad Request - Asincrónica
        /// </summary>
        protected Task<IActionResult> BadRequestResponseAsync(string code, string message, object? details = null)
        {
            var response = (IActionResult)ProblemResponse(StatusCodes.Status400BadRequest, code, "Bad request", message, details);
            return Task.FromResult(response);
        }

        /// <summary>
        /// Respuesta 400 Bad Request - Sincrónica
        /// </summary>
        protected IActionResult BadRequestResponse(string code, string message, object? details = null)
            => ProblemResponse(StatusCodes.Status400BadRequest, code, "Bad request", message, details);

        /// <summary>
        /// Respuesta de validación (400) - Asincrónica
        /// </summary>
        protected Task<IActionResult> ValidationResponseAsync(IReadOnlyDictionary<string, string[]> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var (key, values) in errors)
            {
                foreach (var value in values)
                {
                    modelState.AddModelError(key, value);
                }
            }

            var problem = new ValidationProblemDetails(modelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed",
                Detail = "One or more validation errors occurred.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Instance = HttpContext.Request.Path
            };

            problem.Extensions["code"] = "Validation.Failed";
            problem.Extensions["traceId"] = HttpContext.TraceIdentifier;

            return Task.FromResult<IActionResult>(BadRequest(problem));
        }

        /// <summary>
        /// Respuesta de validación (400) - Sincrónica
        /// </summary>
        protected IActionResult ValidationResponse(IReadOnlyDictionary<string, string[]> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var (key, values) in errors)
            {
                foreach (var value in values)
                {
                    modelState.AddModelError(key, value);
                }
            }

            var problem = new ValidationProblemDetails(modelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed",
                Detail = "One or more validation errors occurred.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Instance = HttpContext.Request.Path
            };

            problem.Extensions["code"] = "Validation.Failed";
            problem.Extensions["traceId"] = HttpContext.TraceIdentifier;
            return BadRequest(problem);
        }

        /// <summary>
        /// Respuesta 401 Unauthorized - Asincrónica
        /// </summary>
        protected Task<IActionResult> UnauthorizedResponseAsync(string message = "Authentication is required.")
        {
            var response = (IActionResult)ProblemResponse(StatusCodes.Status401Unauthorized, "Authentication.Required", "Unauthorized", message);
            return Task.FromResult(response);
        }

        /// <summary>
        /// Respuesta 401 Unauthorized - Sincrónica
        /// </summary>
        protected IActionResult UnauthorizedResponse(string message = "Authentication is required.")
            => ProblemResponse(StatusCodes.Status401Unauthorized, "Authentication.Required", "Unauthorized", message);

        /// <summary>
        /// Respuesta 403 Forbidden - Asincrónica
        /// </summary>
        protected Task<IActionResult> ForbiddenResponseAsync(string message = "You do not have permission to perform this operation.")
        {
            var response = (IActionResult)ProblemResponse(StatusCodes.Status403Forbidden, "Authorization.Forbidden", "Forbidden", message);
            return Task.FromResult(response);
        }

        /// <summary>
        /// Respuesta 403 Forbidden - Sincrónica
        /// </summary>
        protected IActionResult ForbiddenResponse(string message = "You do not have permission to perform this operation.")
            => ProblemResponse(StatusCodes.Status403Forbidden, "Authorization.Forbidden", "Forbidden", message);

        /// <summary>
        /// Respuesta 404 Not Found - Asincrónica
        /// </summary>
        protected Task<IActionResult> NotFoundResponseAsync(string resource, object? identifier = null)
        {
            var message = identifier is null ? $"{resource} was not found." : $"{resource} '{identifier}' was not found.";
            var response = (IActionResult)ProblemResponse(StatusCodes.Status404NotFound, $"{resource}.NotFound", "Not found", message);
            return Task.FromResult(response);
        }

        /// <summary>
        /// Respuesta 404 Not Found - Sincrónica
        /// </summary>
        protected IActionResult NotFoundResponse(string resource, object? identifier = null)
            => ProblemResponse(StatusCodes.Status404NotFound, $"{resource}.NotFound", "Not found",
                identifier is null ? $"{resource} was not found." : $"{resource} '{identifier}' was not found.");

        /// <summary>
        /// Respuesta 409 Conflict - Asincrónica
        /// </summary>
        protected Task<IActionResult> ConflictResponseAsync(string code, string message)
        {
            var response = (IActionResult)ProblemResponse(StatusCodes.Status409Conflict, code, "Conflict", message);
            return Task.FromResult(response);
        }

        /// <summary>
        /// Respuesta 409 Conflict - Sincrónica
        /// </summary>
        protected IActionResult ConflictResponse(string code, string message)
            => ProblemResponse(StatusCodes.Status409Conflict, code, "Conflict", message);

        /// <summary>
        /// Respuesta 422 Unprocessable Entity - Asincrónica
        /// </summary>
        protected Task<IActionResult> UnprocessableResponseAsync(string code, string message, object? details = null)
        {
            var response = (IActionResult)ProblemResponse(StatusCodes.Status422UnprocessableEntity, code, "Unprocessable entity", message, details);
            return Task.FromResult(response);
        }

        /// <summary>
        /// Respuesta 422 Unprocessable Entity - Sincrónica
        /// </summary>
        protected IActionResult UnprocessableResponse(string code, string message, object? details = null)
            => ProblemResponse(StatusCodes.Status422UnprocessableEntity, code, "Unprocessable entity", message, details);

        /// <summary>
        /// Respuesta 202 Accepted - Asincrónica
        /// </summary>
        protected Task<IActionResult> AcceptedResponseAsync(string? statusUrl = null)
            => Task.FromResult<IActionResult>(statusUrl is null ? Accepted() : Accepted(statusUrl));

        /// <summary>
        /// Respuesta 202 Accepted - Sincrónica
        /// </summary>
        protected IActionResult AcceptedResponse(string? statusUrl = null)
            => statusUrl is null ? Accepted() : Accepted(statusUrl);

        /// <summary>
        /// Respuesta 429 Too Many Requests - Asincrónica
        /// </summary>
        protected Task<IActionResult> TooManyRequestsResponseAsync(TimeSpan? retryAfter = null)
        {
            if (retryAfter.HasValue)
                Response.Headers["Retry-After"] = ((int)retryAfter.Value.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);

            var response = (IActionResult)ProblemResponse(StatusCodes.Status429TooManyRequests, "RateLimit.Exceeded", "Too many requests", "Too many requests.");
            return Task.FromResult(response);
        }

        /// <summary>
        /// Respuesta 429 Too Many Requests - Sincrónica
        /// </summary>
        protected IActionResult TooManyRequestsResponse(TimeSpan? retryAfter = null)
        {
            if (retryAfter.HasValue)
                Response.Headers["Retry-After"] = ((int)retryAfter.Value.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);

            return ProblemResponse(StatusCodes.Status429TooManyRequests, "RateLimit.Exceeded", "Too many requests", "Too many requests.");
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Convierte un Result a ActionResult basado en el tipo de error
        /// </summary>
        private ActionResult ToProblem(Result result)
        {
            if (result.Error.Type == ErrorType.Validation)
                return (ActionResult)ValidationResponse(result.ValidationErrors);

            return result.Error.Type switch
            {
                ErrorType.NotFound => ProblemResponse(StatusCodes.Status404NotFound, result.Error.Code, "Not Found", result.Error.Message),
                ErrorType.Conflict => ProblemResponse(StatusCodes.Status409Conflict, result.Error.Code, "Conflict", result.Error.Message),
                ErrorType.Unauthorized => (ActionResult)UnauthorizedResponse(result.Error.Message),
                ErrorType.Forbidden => (ActionResult)ForbiddenResponse(result.Error.Message),
                ErrorType.Failure => ProblemResponse(StatusCodes.Status500InternalServerError, result.Error.Code, "Internal Server Error", result.Error.Message),
                _ => ProblemResponse(StatusCodes.Status400BadRequest, result.Error.Code, "Bad request", result.Error.Message)
            };
        }

        /// <summary>
        /// Genera una respuesta de problema con detalles específicos (Asincrónica)
        /// </summary>
        private Task<ObjectResult> ProblemResponseAsync(int status, string code, string title, string message, object? details = null)
        {
            return Task.FromResult(ProblemResponse(status, code, title, message, details));
        }

        /// <summary>
        /// Genera una respuesta de problema con detalles específicos (Sincrónica)
        /// </summary>
        private ObjectResult ProblemResponse(int status, string code, string title, string message, object? details = null)
        {
            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = message,
                Type = $"https://tools.ietf.org/html/rfc7231#section-{status}",
                Instance = HttpContext.Request.Path
            };

            problem.Extensions["code"] = code;
            problem.Extensions["traceId"] = HttpContext.TraceIdentifier;

            if (details is not null)
                problem.Extensions["details"] = details;

            return StatusCode(status, problem);
        }

        #endregion
    }
}