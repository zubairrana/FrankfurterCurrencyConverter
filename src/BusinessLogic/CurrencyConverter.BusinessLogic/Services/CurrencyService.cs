using CurrencyConverter.BusinessLogic.Constants;
using CurrencyConverter.BusinessLogic.DTOs.Common;
using CurrencyConverter.BusinessLogic.DTOs.Currency;
using CurrencyConverter.BusinessLogic.Extensions;
using CurrencyConverter.BusinessLogic.Interfaces;
using CurrencyConverter.Domain.Exceptions;
using CurrencyConverter.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CurrencyConverter.BusinessLogic.Services
{
    public class CurrencyService
        (ILogger<ICurrencyService> logger,
        ICurrencyProviderFactory currencyProviderFactory) : ICurrencyService
    {
        private readonly ILogger<ICurrencyService> _logger = logger;
        private readonly ICurrencyProviderFactory _currencyProviderFactory = currencyProviderFactory;

        public async Task<IEnumerable<CurrencyRate>> GetLatestRatesAsync(
            string baseCurrency, string? quotes = null, string? currencyProviderName = null,
            CancellationToken cancellationToken = default)
        {
            ValidateCurrency(baseCurrency);

            _logger.LogInformation("Fetching latest rates for currency: {BaseCurrency}", baseCurrency);

            var currencyProvider = _currencyProviderFactory.GetProvider(currencyProviderName);
            var data = await currencyProvider.GetLatestRatesAsync(baseCurrency, quotes, cancellationToken);
            
            var result = data.ToCurrencyRates();
            
            return RemoveRestrictedCurrencies(result);
        }

        private static void ValidateCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency code cannot be empty.");

            if (CurrencyConstants.RestrictedCurrencies.Contains(currency.ToUpperInvariant()))
                throw new RestrictedCurrencyException(currency.ToUpperInvariant());
        }

        private static IEnumerable<CurrencyRate> RemoveRestrictedCurrencies(IEnumerable<CurrencyRate> rates) 
        {
            return rates.Where(x => !CurrencyConstants.RestrictedCurrencies.Contains(x.Quote.ToUpperInvariant()));
        }

        public async Task<ConversionResult> ConvertAsync(
            string fromCurrency, string toCurrency, decimal amount, string? currencyProviderName = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching currency conversion: From: {FromCurrency}, To: {ToCurrency}", fromCurrency, toCurrency);

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

        public async Task<PagedResponse<CurrencyRate>> GetHistoricalRatesAsync(
            string baseCurrency, DateTime fromDate, DateTime toDate,
            int page, int pageSize, string? quotes = null,
            string? currencyProviderName = null,
            CancellationToken cancellationToken = default)
        {
            ValidateCurrency(baseCurrency);

            _logger.LogInformation("Fetching Historical rates for currency: {BaseCurrency}", baseCurrency);

            var currencyProvider = _currencyProviderFactory.GetProvider(currencyProviderName);
            var data = await currencyProvider.GetHistoricalRatesAsync(
                baseCurrency, fromDate, toDate, page, pageSize, quotes, cancellationToken);
            
            var result = data.ToCurrencyRates();
            result = RemoveRestrictedCurrencies(result);

            return result.ToPagedResponse(page, pageSize);
        }
    }
}
