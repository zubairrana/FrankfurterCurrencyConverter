using Serilog;

namespace CurrencyConverter.API.Extensions
{
    public static class SerilogExtensions
    {
        public static IApplicationBuilder UseApiRequestLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                // Customise the message template
                options.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms | ClientIP: {ClientIP} | ClientId: {ClientId}";

                // Add custom properties to every request log
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    // Client IP
                    diagnosticContext.Set("ClientIP",
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? null);

                    // ClientId from JWT token
                    var clientId = httpContext.User?.FindFirst("client_id")?.Value
                    ?? httpContext.User?.FindFirst("sub")?.Value
                    ?? null;
                    diagnosticContext.Set("ClientId", clientId);
                };
            });

            return app;
        }
    }
}
