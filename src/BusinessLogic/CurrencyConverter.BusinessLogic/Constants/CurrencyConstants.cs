namespace CurrencyConverter.BusinessLogic.Constants
{
    public static class CurrencyConstants
    {
        public static readonly HashSet<string> RestrictedCurrencies = new(StringComparer.OrdinalIgnoreCase)
        {
            "TRY", "PLN", "THB", "MXN"
        };
    }
}
