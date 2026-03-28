namespace CurrencyConverter.Infrastructure.Configurations
{
    public class CurrencyProviderOptions
    {
        public string BaseUrl { get; init; } = string.Empty;
        public int TimeoutSeconds { get; init; } = 30;
        public int CacheTimeMinutes { get; init; } = 10;

    }
}
