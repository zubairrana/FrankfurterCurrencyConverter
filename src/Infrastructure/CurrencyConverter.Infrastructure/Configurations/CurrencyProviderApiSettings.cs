namespace CurrencyConverter.Infrastructure.Configurations
{
    public sealed class CurrencyProviderApiSettings
    {
        public const string SectionName = "CurrencyProviderApis";

        public FrankfurterApiSettings Frankfurter { get; init; } = new ();
    }
}
