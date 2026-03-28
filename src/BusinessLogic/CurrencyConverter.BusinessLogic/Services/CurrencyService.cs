using CurrencyConverter.BusinessLogic.Interfaces;
using CurrencyConverter.Domain.Models;
using Microsoft.Extensions.Logging;

namespace CurrencyConverter.BusinessLogic.Services
{
    public class CurrencyService
        (ILogger<ICurrencyService> logger,
        ICurrencyProviderFactory currencyProviderFactory) : ICurrencyService
    {
        private readonly ILogger<ICurrencyService> _logger = logger;
        private readonly ICurrencyProviderFactory _currencyProviderFactory = currencyProviderFactory;

        public async Task<IEnumerable<LatestRate>> GetLatestRatesAsync(
            string baseCurrency, string? quotes = null,string? currencyProviderName = null,
            CancellationToken cancellationToken = default)
        {
            // TODO: Validate baseCurrency

            _logger.LogInformation("Fetching latest rates for currency: {BaseCurrency}", baseCurrency);

            var currencyProvider = _currencyProviderFactory.GetProvider(currencyProviderName);
            var result = await currencyProvider.GetLatestRatesAsync(baseCurrency, quotes, cancellationToken);

            // TODO: Add currency filter requirement as asked in the Task.

            return result;
        }
    }
}
