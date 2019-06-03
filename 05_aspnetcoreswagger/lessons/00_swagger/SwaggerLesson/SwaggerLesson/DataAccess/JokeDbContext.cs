using Microsoft.EntityFrameworkCore;
using SwaggerLesson.Models;

namespace SwaggerLesson.DataAccess
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
