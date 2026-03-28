using CurrencyConverter.Domain.Models;
using CurrencyConverter.Infrastructure.ExternalModels.Frankfurter;

namespace CurrencyConverter.Infrastructure.Mapping
{
    public static class FrankfurterMapper
    {
        public static LatestRate ToLatestRate(this FrankfurterLatestRate src) =>
            new()
            {
                BaseCurrency = src.Base,
                Date = src.Date,
                Quote = src.Quote,
                Rate = src.Rate
            };

        public static IEnumerable<LatestRate> ToLatestRates(this List<FrankfurterLatestRate> src) =>
            src.Select(x => x.ToLatestRate());
    }
}
