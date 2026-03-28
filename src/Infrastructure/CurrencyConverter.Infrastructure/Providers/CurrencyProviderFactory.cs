using CurrencyConverter.BusinessLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CurrencyConverter.Infrastructure.Providers
{
    public class CurrencyProviderFactory
        (IServiceProvider serviceProvider, ILogger<CurrencyProviderFactory> logger) : ICurrencyProviderFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<CurrencyProviderFactory> _logger = logger;
        private static readonly Dictionary<string, Type> CurrencyProviders = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Frankfurter", typeof(FrankfurterCurrencyProvider) },
            //{"NewProvider", typeof(NewProvider)}
        };

        private const string DefaultProvider = "Frankfurter";

        public ICurrencyProvider GetProvider(string? providerName = null)
        {
            var name = string.IsNullOrWhiteSpace(providerName) ? DefaultProvider : providerName;

            if (!CurrencyProviders.TryGetValue(name, out var provider))
            {
                _logger.LogWarning("Provider '{ProviderName}' not found, falling back to default: {Default}", name, DefaultProvider);
                provider = CurrencyProviders[DefaultProvider];
            }

            _logger.LogDebug("CurrencyProvider: {ProviderName}", name);
            return (ICurrencyProvider)_serviceProvider.GetRequiredService(provider);
        }
    }
}
