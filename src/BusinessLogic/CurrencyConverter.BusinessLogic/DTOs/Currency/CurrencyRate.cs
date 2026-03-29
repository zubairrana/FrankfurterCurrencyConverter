namespace CurrencyConverter.BusinessLogic.DTOs.Currency
{
    public class CurrencyRate
    {
        public DateTime Date { get; set; }
        public required string BaseCurrency { get; set; }
        public required string Quote { get; set; }
        public decimal Rate { get; set; }
    }
}
