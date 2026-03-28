using CurrencyConverter.Domain.Models;

namespace CurrencyConverter.BusinessLogic.Interfaces
{
    public interface ICurrencyProvider
    {
        public Task<IEnumerable<LatestRate>> GetLatestRatesAsync(
            string baseCurrency, string? quotes = null,
            CancellationToken cancellationToken = default);
    }
}
