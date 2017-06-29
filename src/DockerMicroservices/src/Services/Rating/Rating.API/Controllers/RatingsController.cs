using Microsoft.AspNetCore.Mvc;
using Rating.API.Infrastructure;
using Rating.API.Models;
using System;
using System.Linq;

namespace Rating.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class RatingsController : Controller
    {
        private readonly RatingContext _context;

        public RatingsController(RatingContext context)
        {
            _context = context;
        }

        [HttpPost("bookratings")]
        public IActionResult GetRatingForBook([FromBody]RatingRequest request)
        {
            var bookIds = request?.BookIds ?? Array.Empty<int>();
            var avgRating = _context.BookRatings
                .Where(rating => bookIds.Contains(rating.BookId))
                .GroupBy(b => b.BookId)
                .Select(grp =>
                new AvgRatingDto
                {
                    BookId = grp.Key,
                    AvgRating = Math.Round(grp.Select(r => r.Rating).Average(), 1)
                }).ToArray();
            var result = new RatingResponse { Ratings = avgRating };
            return Ok(result);
        }
    }
}
