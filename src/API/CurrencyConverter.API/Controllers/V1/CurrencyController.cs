using Asp.Versioning;
using CurrencyConverter.BusinessLogic.Interfaces;
using CurrencyConverter.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CurrencyController(ICurrencyService currencyService) : ControllerBase
    {

        /// <summary>
        /// Retrieves the latest exchange rates for a specified base currency.
        /// </summary>
        /// <param name="baseCurrency">Base currency code (e.g., EUR)</param>
        /// <param name="provider">Optional: Currency provider name (default: Frankfurter)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpGet("rates/latest")]
        [ProducesResponseType(typeof(LatestRate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetLatestRates(
            [FromQuery] string baseCurrency = "USD",
            [FromQuery] string? quotes = null,
            [FromQuery] string? providerName = null,
            CancellationToken cancellationToken = default)
        {
            var result = await currencyService.GetLatestRatesAsync(baseCurrency, quotes, providerName, cancellationToken);

            return Ok(result);
        }
    }
}
