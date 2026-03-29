namespace CurrencyConverter.Domain.Models.Frankfurter
{
    public class FrankfurterCurrencyRate
    {
        public DateTime Date { get; set; }
        public required string Base { get; set; }
        public required string Quote { get; set; }
        public decimal Rate { get; set; }
    }
}
