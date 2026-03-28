using CurrencyConverter.BusinessLogic.Interfaces;
using CurrencyConverter.Domain.Constants;
using CurrencyConverter.Domain.Exceptions;
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
            string baseCurrency, string? quotes = null, string? currencyProviderName = null,
            CancellationToken cancellationToken = default)
        {
            ValidateCurrency(baseCurrency);

            _logger.LogInformation("Fetching latest rates for currency: {BaseCurrency}", baseCurrency);

            var currencyProvider = _currencyProviderFactory.GetProvider(currencyProviderName);
            var result = await currencyProvider.GetLatestRatesAsync(baseCurrency, quotes, cancellationToken);

            return RemoveRestrictedCurrencies(result);
        }

        private static void ValidateCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency code cannot be empty.");

            if (CurrencyConstants.RestrictedCurrencies.Contains(currency.ToUpperInvariant()))
                throw new RestrictedCurrencyException(currency.ToUpperInvariant());
        }

        private static IEnumerable<LatestRate> RemoveRestrictedCurrencies(IEnumerable<LatestRate> rates) 
        {
            return rates.Where(x => !CurrencyConstants.RestrictedCurrencies.Contains(x.Quote.ToUpperInvariant()));
        }

        public async Task<ConversionResult> ConvertAsync(
            string fromCurrency, string toCurrency, decimal amount, string? currencyProviderName = null,
            CancellationToken cancellationToken = default)
        {
            ValidateCurrency(fromCurrency);
            ValidateCurrency(toCurrency);

            var latestRates = await GetLatestRatesAsync(fromCurrency, toCurrency, currencyProviderName, cancellationToken);
            var rate = latestRates.SingleOrDefault();

            return rate is null
                ? throw new CurrencyNotFoundException(toCurrency)
                : new ConversionResult
                {
                    FromCurrency = fromCurrency.ToUpperInvariant(),
                    ToCurrency = toCurrency.ToUpperInvariant(),
                    Amount = amount,
                    ConvertedAmount = amount * rate.Rate,
                    Rate = rate.Rate,
                    Date = rate.Date
                };
        }
    }
}
