using System.Net;

namespace CurrencyConverter.Domain.Exceptions
{
    public class ExternalApiException(string message, HttpStatusCode? statusCode = null) : Exception(message)
    {
        public HttpStatusCode? StatusCode { get; } = statusCode;
    }
}
