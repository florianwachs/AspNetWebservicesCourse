using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TechConf.Api.Models;

namespace TechConf.Api.Data;

public class AppDbContext : DbContext
{
    private static readonly ValueConverter<DateTime, DateTime> UtcDateTimeConverter = new(
        value => NormalizeToUtc(value),
        value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Date).HasConversion(UtcDateTimeConverter);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);

            entity.HasData(
                new Event { Id = 1, Name = ".NET Conf 2026", Date = new DateTime(2026, 11, 10, 0, 0, 0, DateTimeKind.Utc), City = "Online", Description = "The largest .NET event of the year" },
                new Event { Id = 2, Name = "TechConf Munich", Date = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), City = "Munich", Description = "Annual tech conference in Bavaria" },
                new Event { Id = 3, Name = "Cloud Summit", Date = new DateTime(2026, 9, 20, 0, 0, 0, DateTimeKind.Utc), City = "Berlin", Description = "Cloud-native development conference" }
            );
        });
    }

    private static DateTime NormalizeToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
