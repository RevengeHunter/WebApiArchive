using System.Diagnostics;

namespace Bussines_Library.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestLoggingMiddleware> logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var startedAt = Stopwatch.GetTimestamp();
            await next(context);

            var elapseTime = Stopwatch.GetElapsedTime(startedAt);
            logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms with trace {TraceId}", context.Request.Method, context.Request.Path, context.Response.StatusCode, elapseTime.TotalMilliseconds, context.TraceIdentifier);
        }
    }
}
