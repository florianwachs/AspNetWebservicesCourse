using Microsoft.EntityFrameworkCore;
using TechConf.Api.Models;

namespace TechConf.Api.Data;

public static class DbSeeder
{
    public static async Task MigrateAndSeedAsync(IServiceProvider services)
    {
        // AppDbContext is registered as scoped, so startup work creates its own scope.
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Apply any pending migrations first so the database schema matches the current model.
        await db.Database.MigrateAsync();

        // Seeding should be safe to run more than once. If data already exists, stop here.
        if (await db.Events.AnyAsync())
        {
            return;
        }

        // Reusing the same speaker instances across multiple sessions lets EF Core create
        // the related speaker rows and many-to-many join rows when the graph is saved.
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

        // This object graph demonstrates both Event -> Session and Session <-> Speaker relationships.
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
                        Duration = TimeSpan.FromMinutes(45),
                        Speakers = [clara]
                    },
                    new Session
                    {
                        Title = "Dependency Injection for Minimal APIs",
                        Abstract = "Why repositories and DbContext use scoped lifetimes.",
                        Duration = TimeSpan.FromMinutes(60),
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
                        Duration = TimeSpan.FromMinutes(50),
                        Speakers = [bob]
                    }
                ]
            }
        };

        // Adding only the root events is enough; EF Core tracks the full graph and inserts
        // the related sessions, speakers, and join-table rows in one SaveChangesAsync call.
        db.Events.AddRange(events);
        await db.SaveChangesAsync();
    }
}
