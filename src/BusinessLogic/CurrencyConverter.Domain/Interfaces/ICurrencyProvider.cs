using CurrencyConverter.Domain.Models.Frankfurter;

namespace CurrencyConverter.Domain.Interfaces
{
    public interface ICurrencyProvider
    {
        public Task<IEnumerable<FrankfurterCurrencyRate>> GetLatestRatesAsync(
            string baseCurrency, string? quotes = null,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<FrankfurterCurrencyRate>> GetHistoricalRatesAsync(
            string baseCurrency, DateTime fromDate, DateTime toDate,
            int page, int pageSize, string? quotes = null,
            CancellationToken cancellationToken = default);
    }
}
