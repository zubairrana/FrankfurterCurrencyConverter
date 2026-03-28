namespace CurrencyConverter.Domain.Exceptions
{
    public class RestrictedCurrencyException(string currency) : Exception($"Currency '{currency}' is restricted and cannot be used.")
    {
        public string Currency { get; } = currency;
    }
}
