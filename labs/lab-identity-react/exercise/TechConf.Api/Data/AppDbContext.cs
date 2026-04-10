using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechConf.Api.Models;

namespace TechConf.Api.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Date).HasColumnType("date");
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);

            entity.HasData(
                new Event { Id = 1, Name = ".NET Conf 2026", Date = new DateTime(2026, 11, 10), City = "Online", Description = "The largest .NET event of the year", CreatedByUserId = "" },
                new Event { Id = 2, Name = "TechConf Munich", Date = new DateTime(2026, 6, 15), City = "Munich", Description = "Annual tech conference in Bavaria", CreatedByUserId = "" },
                new Event { Id = 3, Name = "Cloud Summit", Date = new DateTime(2026, 9, 20), City = "Berlin", Description = "Cloud-native development conference", CreatedByUserId = "" }
            );
        });
    }
}
