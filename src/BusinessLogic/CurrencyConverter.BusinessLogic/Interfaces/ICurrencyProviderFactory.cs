namespace CurrencyConverter.BusinessLogic.Interfaces
{
    public interface ICurrencyProviderFactory
    {
        ICurrencyProvider GetProvider(string? providerName = null);
    }
}
