namespace Bussines_Library.Api.Middleware
{
    public sealed class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-ID";
        private readonly RequestDelegate next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {

            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var value) && !string.IsNullOrWhiteSpace(value) ? value.ToString() : context.TraceIdentifier;

            context.TraceIdentifier = correlationId;
            context.Response.Headers[HeaderName] = correlationId;

            await next(context);
        }
    }
}
