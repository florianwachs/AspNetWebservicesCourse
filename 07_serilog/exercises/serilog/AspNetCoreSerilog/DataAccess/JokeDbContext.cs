using AspNetCoreSerilog.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSerilog.DataAccess
{
    public class JokeDbContext : DbContext
    {
        public JokeDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Joke> Jokes { get; set; }
        public DbSet<JokeCategory> JokeCategories { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
