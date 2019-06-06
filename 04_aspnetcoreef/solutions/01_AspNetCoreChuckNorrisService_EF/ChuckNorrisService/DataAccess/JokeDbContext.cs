using ChuckNorrisService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
