using Microsoft.AspNetCore.Mvc;
using Rating.API.Infrastructure;
using Rating.API.Models;
using System;
using System.Linq;

namespace Rating.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class RaitingsController : Controller
    {
        private readonly RaitingContext _context;

        public RaitingsController(RaitingContext context)
        {
            _context = context;
        }

        [HttpPost("bookratings")]
        public IActionResult GetRatingForBook([FromBody]RaitingRequest request)
        {
            var bookIds = request?.BookIds ?? Array.Empty<int>();
            var avgRating = _context.BookRaitings
                .Where(rating => bookIds.Contains(rating.BookId))
                .GroupBy(b => b.BookId)
                .Select(grp =>
                new AvgRaitingDto
                {
                    BookId = grp.Key,
                    AvgRaiting = Math.Round(grp.Select(r => r.Rating).Average(), 1)
                }).ToArray();
            var result = new RaitingResponse { Raitings = avgRating };
            return Ok(result);
        }
    }
}
