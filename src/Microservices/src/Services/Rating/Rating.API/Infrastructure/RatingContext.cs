using Microsoft.EntityFrameworkCore;
using Rating.API.Models;

namespace Rating.API.Infrastructure
{
    public class RatingContext : DbContext
    {
        public RatingContext(DbContextOptions<RatingContext> options) : base(options) { }

        public DbSet<BookRating> BookRatings { get; set; }
    }
}
