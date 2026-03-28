using CurrencyConverter.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;

namespace CurrencyConverter.Infrastructure.Http
{
    public static class HttpClientResilienceExtensions
    {
        public static IHttpResiliencePipelineBuilder AddConfiguredResilience(
            this IHttpClientBuilder builder,
            string pipelineName = "default")
        {
            return builder.AddResilienceHandler(pipelineName, (pipeline, context) =>
            {
                var settings = context.ServiceProvider
                    .GetRequiredService<IOptions<HttpResilienceSettings>>()
                    .Value;

                // 🔁 Retry
                pipeline.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = settings.RetryCount,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromSeconds(settings.RetryDelaySeconds),
                    UseJitter = settings.Jitter
                });

                // ⚡ Circuit Breaker
                pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    SamplingDuration = TimeSpan.FromSeconds(settings.CircuitBreakerSamplingSeconds),
                    MinimumThroughput = settings.CircuitBreakerMinimumThroughput,
                    FailureRatio = settings.CircuitBreakerFailureRatio,
                    BreakDuration = TimeSpan.FromSeconds(settings.CircuitBreakerBreakSeconds)
                });

                // ⏱ Timeout
                pipeline.AddTimeout(TimeSpan.FromSeconds(settings.TimeoutSeconds));
            });
        }
    }
}
