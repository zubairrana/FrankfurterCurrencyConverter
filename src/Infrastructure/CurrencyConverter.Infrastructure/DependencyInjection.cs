using CurrencyConverter.BusinessLogic.Interfaces;
using CurrencyConverter.BusinessLogic.Services;
using CurrencyConverter.Infrastructure.Configurations;
using CurrencyConverter.Infrastructure.Constants;
using CurrencyConverter.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace CurrencyConverter.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjections(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            // Register provider implementations
            services.AddScoped<FrankfurterCurrencyProvider>();

            // Register factory
            services.AddScoped<ICurrencyProviderFactory, CurrencyProviderFactory>();

            // Register application service
            services.AddScoped<ICurrencyService, CurrencyService>();

            services.Configure<CurrencyProviderApiSettings>(
                configuration.GetSection(CurrencyProviderApiSettings.SectionName));

            services.AddHttpClient(CurrencyProviderConstants.CurrencyProviderHttpClient, client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "CurrencyConverterAPI");
            })
                .AddResilienceHandler("currency-pipeline", pipeline =>
                {
                    // Retry policy with exponential backoff
                    pipeline.AddRetry(new HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        BackoffType = DelayBackoffType.Exponential,
                        Delay = TimeSpan.FromSeconds(1),
                        UseJitter = true
                    });

                    // Circuit breaker
                    pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                    {
                        SamplingDuration = TimeSpan.FromSeconds(30),
                        MinimumThroughput = 5,
                        FailureRatio = 0.5,
                        BreakDuration = TimeSpan.FromSeconds(30)
                    });

                    // Timeout per attempt
                    pipeline.AddTimeout(TimeSpan.FromSeconds(10));
                });

            return services;
        }
    }
}
