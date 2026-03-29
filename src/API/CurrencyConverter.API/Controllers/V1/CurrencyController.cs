using Asp.Versioning;
using CurrencyConverter.BusinessLogic.DTOs.Common;
using CurrencyConverter.BusinessLogic.DTOs.Currency;
using CurrencyConverter.BusinessLogic.Interfaces;
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
        /// <param name="quotes">Comma separated quote currencies code (e.g., USD,AED)</param>
        /// <param name="provider">Optional: Currency provider name (default: Frankfurter)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpGet("rates/latest")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<CurrencyRate>> GetLatestRates(
            [FromQuery] string baseCurrency = "EUR",
            [FromQuery] string? quotes = null,
            [FromQuery] string? providerName = null,
            CancellationToken cancellationToken = default)
        {
            var result = await currencyService.GetLatestRatesAsync(baseCurrency, quotes, providerName, cancellationToken);

            return Ok(result);
        }


        /// <summary>
        /// Converts an amount from one currency to another.
        /// TRY, PLN, THB, and MXN are not supported.
        /// </summary>
        /// <param name="fromCurrency">From currency code (e.g., EUR)</param>
        /// <param name="fromCurrency">To currency code (e.g., USD)</param>
        /// <param name="amount">Amount</param>
        /// <param name="provider">Optional: Currency provider name (default: Frankfurter)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpGet("convert")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ConversionResult>> Convert(
            [FromQuery] string fromCurrency = "EUR",
            [FromQuery] string toCurrency = "USD",
            [FromQuery] decimal amount = 1,
            [FromQuery] string? provider = null,
            CancellationToken cancellationToken = default)
        {
            var result = await currencyService.ConvertAsync(fromCurrency, toCurrency, amount, provider, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves historical exchange rates for a given period with pagination.
        /// </summary>
        /// <param name="baseCurrency">Base currency code (e.g., EUR)</param>
        /// <param name="startDate">StartDate (e.g., 2025-05-01), default UTC Time - 1 Month</param>
        /// <param name="endDate">EndDate (e.g., 2025-05-30), default UTC Time</param>
        /// <param name="page">Page (default: 1)</param>
        /// <param name="pageSize">PageSize (default: 10)</param>
        /// <param name="quotes">Comma separated quote currencies code (e.g., USD,AED)</param>
        /// <param name="provider">Optional: Currency provider name (default: Frankfurter)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpGet("rates/historical")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<PagedResponse<CurrencyRate>>> GetHistoricalRates(
            [FromQuery] string baseCurrency = "EUR",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? quotes = null,
            [FromQuery] string? provider = null,
            CancellationToken cancellationToken = default)
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var result = await currencyService.GetHistoricalRatesAsync(
                baseCurrency, start, end, page, pageSize, quotes, provider, cancellationToken);

            return Ok(result);
        }
    }
}
