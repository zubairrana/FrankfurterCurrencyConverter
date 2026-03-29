using CurrencyConverter.Domain.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using CurrencyConverter.Infrastructure.Configurations;
using CurrencyConverter.Domain.Models.Frankfurter;
using CurrencyConverter.Domain.Interfaces;
using CurrencyConverter.Domain.Constants;

namespace CurrencyConverter.Infrastructure.Providers
{
    public class FrankfurterCurrencyProvider : ICurrencyProvider
    {
        private readonly ILogger<FrankfurterCurrencyProvider> _logger;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration;

        private const string CacheKeyPrefix = "frankfurter_";

        public FrankfurterCurrencyProvider(
            ILogger<FrankfurterCurrencyProvider> logger,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            IOptions<CurrencyProviderApiSettings> options)
        {
            _logger = logger;
            _cache = cache;

            var apiOptions = options.Value.Frankfurter;

            _cacheDuration = TimeSpan.FromMinutes(apiOptions.CacheTimeMinutes);

            _httpClient = httpClientFactory.CreateClient(CurrencyProviderConstants.CurrencyProviderHttpClient);
            _httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
        }

        public async Task<IEnumerable<FrankfurterCurrencyRate>> GetLatestRatesAsync(string baseCurrency, string? quotes = null, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}latest_{baseCurrency.ToUpperInvariant()}_{quotes?.ToUpperInvariant()}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<FrankfurterCurrencyRate>? cached) && cached is not null)
            {
                _logger.LogInformation("Latest rates from cache: {BaseCurrency}", baseCurrency);
                return cached;
            }

            var query = new Dictionary<string, string?>
            {
                ["base"] = baseCurrency.ToUpperInvariant(),
                ["quotes"] = quotes
            };

            var url = QueryHelpers.AddQueryString("rates", query);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Frankfurter API error: {StatusCode} - {Content}", response.StatusCode, content);
                throw new ExternalApiException($"External API returned {response.StatusCode}", response.StatusCode);
            }

            var data = await response.Content.ReadFromJsonAsync<List<FrankfurterCurrencyRate>>(cancellationToken: cancellationToken)
                ?? throw new ExternalApiException("Failed to deserialize response from Frankfurter API.");
            
            _logger.LogInformation("Latest rates from API: {BaseCurrency}", baseCurrency);

            _cache.Set(cacheKey, data, _cacheDuration);
            return data;
        }

        public async Task<IEnumerable<FrankfurterCurrencyRate>> GetHistoricalRatesAsync(
            string baseCurrency, DateTime fromDate, DateTime toDate,
            int page, int pageSize, string? quotes = null, 
            CancellationToken cancellationToken = default)
        {
            var fromDateString = fromDate.ToString("yyyy-MM-dd");
            var toDateString = toDate.ToString("yyyy-MM-dd");
            var cacheKey = $"{CacheKeyPrefix}historical_{baseCurrency.ToUpperInvariant()}_{fromDateString}_{toDateString}_{quotes}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<FrankfurterCurrencyRate>? cached) && cached is not null)
            {
                _logger.LogInformation("Cached historical rates: {BaseCurrency} {FromDate}-{ToDate}", baseCurrency, fromDateString, toDateString);
                return cached;
            }

            var query = new Dictionary<string, string?>
            {
                ["base"] = baseCurrency.ToUpperInvariant(),
                ["from"] = fromDateString,
                ["to"] = toDateString,
                ["quotes"] = quotes
            };

            var url = QueryHelpers.AddQueryString("rates", query);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Frankfurter API error: {StatusCode} - {Content}", response.StatusCode, content);
                throw new ExternalApiException($"External API returned {response.StatusCode}", response.StatusCode);
            }

            var data = await response.Content.ReadFromJsonAsync<List<FrankfurterCurrencyRate>>(cancellationToken: cancellationToken)
                ?? throw new ExternalApiException("Failed to deserialize response from Frankfurter API.");

            _logger.LogInformation("Historical rates from API: {BaseCurrency} {FromDate}-{ToDate}", baseCurrency, fromDateString, toDateString);

            _cache.Set(cacheKey, data, _cacheDuration);
            return data;
        }
    }
}
