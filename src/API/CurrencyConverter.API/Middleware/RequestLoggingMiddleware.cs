using System.Diagnostics;

namespace CurrencyConverter.API.Middleware
{
    public class RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var clientId = context.User?.Identity?.IsAuthenticated == true
                ? context.User.FindFirst("client_id")?.Value
                ?? context.User.FindFirst("sub")?.Value
                : null;

            await next(context);

            stopwatch.Stop();

            logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedTimeSpan} | ClientIP: {ClientIP} | ClientId: {ClientId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                TimeSpan.FromMicroseconds(stopwatch.ElapsedMilliseconds),
                clientIp,
                clientId);
        }
    }
}
