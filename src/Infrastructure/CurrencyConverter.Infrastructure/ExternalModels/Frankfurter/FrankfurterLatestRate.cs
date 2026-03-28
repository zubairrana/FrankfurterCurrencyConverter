namespace CurrencyConverter.Infrastructure.ExternalModels.Frankfurter
{
    public class FrankfurterLatestRate
    {
        public DateTime Date { get; set; }
        public required string Base { get; set; }
        public required string Quote { get; set; }
        public decimal Rate { get; set; }
    }
}