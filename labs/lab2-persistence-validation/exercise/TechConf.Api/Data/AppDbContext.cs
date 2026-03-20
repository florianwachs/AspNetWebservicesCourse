using Microsoft.EntityFrameworkCore;
using TechConf.Api.Models;

namespace TechConf.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Speaker> Speakers => Set<Speaker>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
