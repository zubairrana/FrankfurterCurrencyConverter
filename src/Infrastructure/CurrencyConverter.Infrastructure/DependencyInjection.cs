using CurrencyConverter.BusinessLogic.Interfaces;
using CurrencyConverter.BusinessLogic.Services;
using CurrencyConverter.Domain.Constants;
using CurrencyConverter.Domain.Interfaces;
using CurrencyConverter.Infrastructure.Configurations;
using CurrencyConverter.Infrastructure.Http;
using CurrencyConverter.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverter.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjections(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddScoped<ITokenService, TokenService>();

            // Register provider implementations
            services.AddScoped<FrankfurterCurrencyProvider>();

            // Register factory
            services.AddScoped<ICurrencyProviderFactory, CurrencyProviderFactory>();

            // Register application service
            services.AddScoped<ICurrencyService, CurrencyService>();

            services.Configure<CurrencyProviderApiSettings>(
                configuration.GetSection(CurrencyProviderApiSettings.SectionName));

            // Register the delegating handler
            services.AddTransient<CorrelationIdDelegatingHandler>();

            services.Configure<HttpResilienceSettings>(
                configuration.GetSection(HttpResilienceSettings.SectionName));

            services.AddHttpClient(CurrencyProviderConstants.CurrencyProviderHttpClient, client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "CurrencyConverterAPI");
                client.Timeout = TimeSpan.FromMinutes(5); // greater than resilience timeout with retries time margin
            })
                .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
                .AddConfiguredResilience("currency-pipeline");

            return services;
        }
    }
}
