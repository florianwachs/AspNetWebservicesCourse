using ChuckNorrisService.Models;
using Microsoft.EntityFrameworkCore;

namespace ChuckNorrisService.DataAccess;

public class JokeDbContext : DbContext
{
    public JokeDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Joke> Jokes => Set<Joke>();
    public DbSet<JokeCategory> JokeCategories => Set<JokeCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var jokeConfig = modelBuilder.Entity<Joke>();
        jokeConfig.Property(j => j.Id).ValueGeneratedNever();
        jokeConfig.HasMany(j => j.Categories).WithMany(c => c.Jokes);
        
        jokeConfig.Property(j => j.Value)
            .IsRequired()
            .HasMaxLength(500);

        var categoryConfig = modelBuilder.Entity<JokeCategory>();
        categoryConfig.Property(j => j.Id).ValueGeneratedNever();
    }
}
