using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.API.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CurrencyController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Version()
        {
            return Ok("This is a test V2 endpoint");
        }
    }
}
