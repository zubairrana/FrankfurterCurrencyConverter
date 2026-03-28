namespace CurrencyConverter.Infrastructure.Configurations
{
    public sealed class HttpResilienceSettings
    {
        public const string SectionName = "HttpResilienceSettings";

        // Retry
        public int RetryCount { get; init; }
        public int RetryDelaySeconds { get; init; }
        public bool Jitter { get; init; }

        // Circuit Breaker
        public int CircuitBreakerSamplingSeconds { get; init; }
        public int CircuitBreakerMinimumThroughput { get; init; }
        public double CircuitBreakerFailureRatio { get; init; }
        public int CircuitBreakerBreakSeconds { get; init; }

        // Timeout
        public int TimeoutSeconds { get; init; }
    }
}
