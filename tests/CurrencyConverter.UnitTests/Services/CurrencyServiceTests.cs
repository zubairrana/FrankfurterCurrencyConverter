using CurrencyConverter.BusinessLogic.Services;
using CurrencyConverter.Domain.Exceptions;
using CurrencyConverter.Domain.Interfaces;
using CurrencyConverter.Domain.Models.Frankfurter;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CurrencyConverter.UnitTests.Services
{
    public class CurrencyServiceTests
    {
        private readonly Mock<ICurrencyProviderFactory> _currencyProviderFactoryMock;
        private readonly Mock<ICurrencyProvider>  _currencyProviderMock;
        private readonly Mock<ILogger<CurrencyService>> _currencyServiceloggerMock;

        private readonly CurrencyService _sut;

        public CurrencyServiceTests() 
        {
            _currencyProviderFactoryMock = new Mock<ICurrencyProviderFactory>();
            _currencyProviderMock = new Mock<ICurrencyProvider>();
            _currencyServiceloggerMock = new Mock<ILogger<CurrencyService>>();

            _currencyProviderFactoryMock.Setup(x => x.GetProvider(It.IsAny<string?>())).Returns(_currencyProviderMock.Object);
            _sut = new CurrencyService(_currencyServiceloggerMock.Object, _currencyProviderFactoryMock.Object);
        }

        [Fact]
        public async Task GetLatestRatesAsync_ValidCurrency_ReturnsRate()
        {
            // Arrange
            var expectedResult = BuildFrankfurterCurrencyRates("EUR", [("USD", 1.08m), ("GBP", 0.85m)]);
            _currencyProviderMock.Setup(x => x.GetLatestRatesAsync("EUR", null, TestContext.Current.CancellationToken)).ReturnsAsync(expectedResult);

            // Act
            var result = await _sut.GetLatestRatesAsync("EUR", null, null, TestContext.Current.CancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(x => x.Quote == "USD");
        }

        [Theory]
        [InlineData("TRY")]
        [InlineData("MXN")]
        public async Task GetLatestRatesAsync_RestrictedBaseCurrency_ThrowsRestrictedCurrencyException(string baseCurrency)
        {
            // Act
            var act = () =>  _sut.GetLatestRatesAsync(baseCurrency, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            await act.Should().ThrowAsync<RestrictedCurrencyException>()
            .WithMessage($"*'{baseCurrency}'*");
        }

        [Fact]
        public async Task ConvertAsync_ValidCurrencies_ReturnsConversionResult()
        {
            // Arrange
            var rates = BuildFrankfurterCurrencyRates("EUR", [("USD", 1.08m)]);
            _currencyProviderMock.Setup(p => p.GetLatestRatesAsync("EUR", "USD", TestContext.Current.CancellationToken)).ReturnsAsync(rates);

            // Act
            var result = await _sut.ConvertAsync("EUR", "USD", 100m, null, TestContext.Current.CancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.FromCurrency.Should().Be("EUR");
            result.ToCurrency.Should().Be("USD");
            result.Amount.Should().Be(100m);
            result.ConvertedAmount.Should().BeApproximately(108m, 0.001m);
            result.Rate.Should().Be(1.08m);
        }

        [Fact]
        public async Task GetHistoricalRatesAsync_ValidRequest_ReturnsPagedResponse()
        {
            // Arrange
            var start = new DateTime(2020, 1, 1);
            var end = new DateTime(2020, 1, 31);
            var rates = BuildFrankfurterCurrencyRates("EUR", [("USD", 1.12m), ("GBP", 0.85m)]);
            _currencyProviderMock.Setup(p => p.GetHistoricalRatesAsync("EUR", start, end, 1, 10, null, TestContext.Current.CancellationToken))
                .ReturnsAsync(rates);

            // Act
            var result = await _sut.GetHistoricalRatesAsync("EUR", start, end, 1, 10, null, null, TestContext.Current.CancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeEmpty();
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
        }

        #region Helpers

        private static IEnumerable<FrankfurterCurrencyRate> BuildFrankfurterCurrencyRates(
         string baseCurrency,
         IEnumerable<(string Quote, decimal Rate)> rates) =>
            rates.Select(r => new FrankfurterCurrencyRate
            {
                Base = baseCurrency,
                Quote = r.Quote,
                Rate = r.Rate,
                Date = DateTime.Today
            });

        #endregion
    }
}
