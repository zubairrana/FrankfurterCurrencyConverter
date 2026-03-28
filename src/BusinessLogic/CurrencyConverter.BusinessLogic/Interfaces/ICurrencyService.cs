using CurrencyConverter.Domain.Models;

namespace CurrencyConverter.BusinessLogic.Interfaces
{
    public interface ICurrencyService
    {
        Task<IEnumerable<LatestRate>> GetLatestRatesAsync(
            string baseCurrency, string? quotes = null, string? currencyProviderName = null,
            CancellationToken cancellationToken = default);
        Task<ConversionResult> ConvertAsync(
            string fromCurrency, string toCurrency, decimal amount, string? currencyProviderName = null,
            CancellationToken cancellationToken = default);
    }
}
