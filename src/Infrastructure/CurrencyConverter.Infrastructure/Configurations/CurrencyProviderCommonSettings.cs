namespace CurrencyConverter.Infrastructure.Configurations
{
    public abstract class CurrencyProviderCommonSettings
    {
        public string BaseUrl { get; init; } = string.Empty;
        public int CacheTimeMinutes { get; init; } = 10;
    }
}
