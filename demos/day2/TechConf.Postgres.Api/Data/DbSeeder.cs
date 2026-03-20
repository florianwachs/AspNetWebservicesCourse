using Microsoft.EntityFrameworkCore;
using TechConf.Api.Models;

namespace TechConf.Api.Data;

public static class DbSeeder
{
    public static async Task MigrateAndSeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Using migrations keeps the day 2 sample aligned with the EF Core workflow we teach.
        await db.Database.MigrateAsync();

        if (await db.Events.AnyAsync())
        {
            return;
        }

        var alice = new Speaker
        {
            Name = "Alice Meyer",
            Company = "TechConf",
            Bio = "Builds developer platforms and speaks about API design."
        };

        var bob = new Speaker
        {
            Name = "Bob Novak",
            Company = "Cloud Atlas",
            Bio = "Focuses on observability and cloud-native systems."
        };

        var clara = new Speaker
        {
            Name = "Clara Fischer",
            Company = "Data Forge",
            Bio = "Helps teams design practical data access layers with EF Core."
        };

        var events = new[]
        {
            new Event
            {
                Name = "TechConf Architecture Day",
                Date = new DateTime(2026, 6, 18),
                City = "Munich",
                Description = "A compact event about modern API architecture.",
                Sessions =
                [
                    new Session
                    {
                        Title = "From In-Memory Lists to Real Databases",
                        Abstract = "Comparing the first EF Core steps with the in-memory version from day 1.",
                        DurationMinutes = 45,
                        Speakers = [clara]
                    },
                    new Session
                    {
                        Title = "Dependency Injection for Minimal APIs",
                        Abstract = "Why repositories and DbContext use scoped lifetimes.",
                        DurationMinutes = 60,
                        Speakers = [alice, bob]
                    }
                ]
            },
            new Event
            {
                Name = "Cloud Native Debugging Workshop",
                Date = new DateTime(2026, 7, 2),
                City = "Berlin",
                Description = "Hands-on debugging with logs, ProblemDetails, and OpenAPI.",
                Sessions =
                [
                    new Session
                    {
                        Title = "Reading ProblemDetails Like a Pro",
                        Abstract = "How to turn standardized error responses into faster debugging sessions.",
                        DurationMinutes = 50,
                        Speakers = [bob]
                    }
                ]
            }
        };

        db.Events.AddRange(events);
        await db.SaveChangesAsync();
    }
}
