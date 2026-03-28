namespace CurrencyConverter.Domain.Exceptions
{
    public class CurrencyNotFoundException(string currency) : Exception($"Currency {currency} was not found")
    {
        public string Currency { get; } = currency;
    }
}
