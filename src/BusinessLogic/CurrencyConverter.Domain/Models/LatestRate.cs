namespace CurrencyConverter.Domain.Models
{
    public class LatestRate
    {
        public DateTime Date { get; set; }
        public required string BaseCurrency { get; set; }
        public required string Quote { get; set; }
        public decimal Rate { get; set; }
    }
}
