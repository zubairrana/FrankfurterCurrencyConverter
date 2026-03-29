using CurrencyConverter.Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CurrencyConverter.Infrastructure.Http
{
    public class CorrelationIdDelegatingHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CorrelationIdDelegatingHandler> logger) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage httpRequest,
            CancellationToken cancellationToken)
        {
            var correlationId = httpContextAccessor.HttpContext?
                .Items[HttpHeaderConstants.CorrelationId]?.ToString();

            if (!string.IsNullOrEmpty(correlationId))
            {
                httpRequest.Headers.TryAddWithoutValidation(
                    HttpHeaderConstants.CorrelationId,
                    correlationId
                );
            }
            
            logger.LogInformation(
                "Calling API {Url} | CorrelationId: {CorrelationId}",
                httpRequest.RequestUri, correlationId);

            return base.SendAsync(httpRequest, cancellationToken);
        }
    }
}
