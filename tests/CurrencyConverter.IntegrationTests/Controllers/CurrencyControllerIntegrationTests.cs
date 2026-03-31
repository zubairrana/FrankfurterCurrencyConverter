using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace CurrencyConverter.IntegrationTests.Controllers
{
    public class CurrencyControllerIntegrationTests : IClassFixture<CurrencyConverterWebAppFactory>
    {
        private readonly HttpClient _client;
        private readonly CurrencyConverterWebAppFactory _factory;

        public CurrencyControllerIntegrationTests(CurrencyConverterWebAppFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<string> GetTokenAsync(string username = "user", string password = "user123")
        {
            var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new { username, password });
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            return body.GetProperty("token").GetString()!;
        }

        [Fact]
        public async Task GetLatestRates_WithoutAuth_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/v1/currency/rates/latest?baseCurrency=EUR");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetLatestRates_WithAuth_ReturnsOK()
        {
            var token = await GetTokenAsync("admin", "admin123");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/v1/currency/rates/latest?baseCurrency=EUR");

            // Depends on external API availability; accept OK or ServiceUnavailable
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
        }

        [Theory]
        [InlineData("TRY")]
        [InlineData("PLN")]
        [InlineData("THB")]
        [InlineData("MXN")]
        public async Task GetLatestRates_RestrictedCurrency_ReturnsBadRequest(string currency)
        {
            var token = await GetTokenAsync("admin", "admin123");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/api/v1/currency/rates/latest?baseCurrency={currency}");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("TRY", "USD")]
        [InlineData("USD", "PLN")]
        public async Task Convert_RestrictedCurrency_ReturnsBadRequest(string from, string to)
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync(
                $"/api/v1/currency/convert?fromCurrency={from}&toCurrency={to}&amount=100");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetHistoricalRates_RestrictedBase_ReturnsBadRequest()
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync(
                "/api/v1/currency/rates/historical?baseCurrency=TRY&startDate=2020-01-01&endDate=2020-01-31");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
