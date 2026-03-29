namespace CurrencyConverter.Domain.Interfaces
{
    public interface ICurrencyProviderFactory
    {
        ICurrencyProvider GetProvider(string? providerName = null);
    }
}
