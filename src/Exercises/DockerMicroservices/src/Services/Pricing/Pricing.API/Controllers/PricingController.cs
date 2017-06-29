using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pricing.API.Infrastructure;
using Pricing.API.Models;
using System.Linq;

namespace Pricing.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class PricingController : Controller
    {
        private readonly PriceEngine _priceEngine;

        public PricingController(PriceEngine priceEngine)
        {
            _priceEngine = priceEngine;
        }

        [HttpGet("books/{bookId}")]
        [ProducesResponseType(typeof(Price), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Price), StatusCodes.Status400BadRequest)]
        public IActionResult GetPriceForBook(int bookId)
        {
            return Ok(_priceEngine.GetPriceForBook(bookId));
        }

        [HttpPost("bookprices")]
        [ProducesResponseType(typeof(PriceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PriceResponse), StatusCodes.Status400BadRequest)]
        public IActionResult GetPricesForBooks([FromBody]PriceRequest priceRequest)
        {
            if ((priceRequest?.BookIds?.Length ?? 0) == 0)
                return BadRequest("A valid PriceRequest must contain BookIds");

            var response = new PriceResponse
            {
                Prices = priceRequest.BookIds.Select(bookId => _priceEngine.GetPriceForBook(bookId)).ToList()
            };

            return Ok(response);
        }
    }
}
