using BackendApi.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendApi.DataAccess
{
    public class JokeDbContext : DbContext
    {
        public JokeDbContext(DbContextOptions<JokeDbContext> options) : base(options)
        {

        }
        public DbSet<Joke> Jokes { get; set; }
    }
}
