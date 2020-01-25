using ChuckNorrisService.Models;
using Microsoft.EntityFrameworkCore;

namespace ChuckNorrisService.DataAccess
{
    public class JokeDbContext : DbContext
    {
        public JokeDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Joke> Jokes { get; set; }
        public DbSet<JokeCategory> JokeCategories { get; set; }
    }
}
