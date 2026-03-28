namespace CurrencyConverter.Infrastructure.Configurations
{
    public class CurrencyProviderApiSettings
    {
        public const string SectionName = "CurrencyProviderApis";

        public FrankfurterOptions Frankfurter { get; init; } = new ();
    }
}
