using CurrencyConverter.Domain.Models;

namespace CurrencyConverter.BusinessLogic.Interfaces
{
    public interface ICurrencyService
    {
        Task<IEnumerable<LatestRate>> GetLatestRatesAsync(string baseCurrency, string? quotes = null, string? currencyProviderName = null, CancellationToken cancellationToken = default);
    }
}
