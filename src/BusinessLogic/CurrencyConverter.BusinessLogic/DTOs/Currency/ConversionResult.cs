namespace CurrencyConverter.BusinessLogic.DTOs.Currency
{
    public class ConversionResult
    {
        public required string FromCurrency { get; init; }
        public required string ToCurrency { get; init; }
        public decimal Amount { get; init; }
        public decimal ConvertedAmount { get; init; }
        public decimal Rate { get; init; }
        public DateTime Date { get; init; }
    }
}
