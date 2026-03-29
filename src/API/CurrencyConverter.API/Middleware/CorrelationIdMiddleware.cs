using CurrencyConverter.Domain.Constants;

namespace CurrencyConverter.API.Middleware
{
    public class CorrelationIdMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            // Read from request OR generate new
            var correlationId = context.Request.Headers[HttpHeaderConstants.CorrelationId].FirstOrDefault()
                ?? Guid.NewGuid().ToString();

            // Store in HttpContext
            context.Items[HttpHeaderConstants.CorrelationId] = correlationId;

            // Return it in response (very useful for clients)
            context.Response.Headers[HttpHeaderConstants.CorrelationId] = correlationId;

            await next(context);
        }
    }
}
