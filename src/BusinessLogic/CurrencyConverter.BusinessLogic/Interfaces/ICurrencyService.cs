using CurrencyConverter.BusinessLogic.DTOs.Common;
using CurrencyConverter.BusinessLogic.DTOs.Currency;

namespace CurrencyConverter.BusinessLogic.Interfaces
{
    public interface ICurrencyService
    {
        Task<IEnumerable<CurrencyRate>> GetLatestRatesAsync(
            string baseCurrency, string? quotes = null, string? currencyProviderName = null,
            CancellationToken cancellationToken = default);
        Task<ConversionResult> ConvertAsync(
            string fromCurrency, string toCurrency, decimal amount, string? currencyProviderName = null,
            CancellationToken cancellationToken = default);
        Task<PagedResponse<CurrencyRate>> GetHistoricalRatesAsync(
            string baseCurrency, DateTime fromDate, DateTime toDate, int page, int pageSize,
            string? quotes = null, string? currencyProviderName = null, CancellationToken cancellationToken = default);
    }
}
