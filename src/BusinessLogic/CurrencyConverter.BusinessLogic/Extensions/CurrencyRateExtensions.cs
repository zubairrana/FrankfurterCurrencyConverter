using CurrencyConverter.BusinessLogic.DTOs.Currency;
using CurrencyConverter.Domain.Models.Frankfurter;

namespace CurrencyConverter.BusinessLogic.Extensions
{
    public static class CurrencyRateExtensions
    {
        public static CurrencyRate ToCurrencyRate(this FrankfurterCurrencyRate src) =>
            new()
            {
                BaseCurrency = src.Base,
                Date = src.Date,
                Quote = src.Quote,
                Rate = src.Rate
            };

        public static IEnumerable<CurrencyRate> ToCurrencyRates(this IEnumerable<FrankfurterCurrencyRate> src) =>
            src.Select(x => x.ToCurrencyRate());
    }
}
