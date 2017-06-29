using Microsoft.EntityFrameworkCore;
using Rating.API.Models;

namespace Rating.API.Infrastructure
{
    public class RaitingContext : DbContext
    {
        public RaitingContext(DbContextOptions<RaitingContext> options) : base(options) { }

        public DbSet<BookRaiting> BookRaitings { get; set; }
    }
}
