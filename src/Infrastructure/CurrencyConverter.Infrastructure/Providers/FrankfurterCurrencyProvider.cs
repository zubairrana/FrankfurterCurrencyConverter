using CurrencyConverter.BusinessLogic.Interfaces;
using CurrencyConverter.Domain.Exceptions;
using CurrencyConverter.Domain.Models;
using CurrencyConverter.Infrastructure.ExternalModels.Frankfurter;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using CurrencyConverter.Infrastructure.Mapping;

namespace CurrencyConverter.Infrastructure.Providers
{
    public class FrankfurterCurrencyProvider : ICurrencyProvider
    {
        private readonly ILogger<FrankfurterCurrencyProvider> _logger;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        private const string CacheKeyPrefix = "frankfurter_";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public FrankfurterCurrencyProvider(
            ILogger<FrankfurterCurrencyProvider> logger,
            HttpClient httpClient,
            IMemoryCache cache)
        {
            _logger = logger;
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<IEnumerable<LatestRate>> GetLatestRatesAsync(string baseCurrency, string? quotes = null, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}latest_{baseCurrency.ToUpperInvariant()}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<LatestRate>? cached) && cached is not null)
            {
                _logger.LogDebug("Latest rates from cache: {BaseCurrency}", baseCurrency);
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

            var data = await response.Content.ReadFromJsonAsync<List<FrankfurterLatestRate>>(cancellationToken: cancellationToken)
                ?? throw new ExternalApiException("Failed to deserialize response from Frankfurter API.");

            var result = data.ToLatestRates();

            _cache.Set(cacheKey, result, CacheDuration);
            return result;
        }
    }
}
