using Microsoft.EntityFrameworkCore;
using TechConf.Microservices.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<SessionsDbContext>("sessionsdb");

var app = builder.Build();

await SessionsSeedData.EnsureSeededAsync(app.Services);

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Redirect("/sessions"));

app.MapGet("/sessions", async (SessionsDbContext db) =>
{
    var sessions = await db.Sessions
        .OrderBy(s => s.StartsAt)
        .Select(s => new SessionSummary(s.Id, s.EventId, s.Title, s.Speaker, s.StartsAt, s.SeatsAvailable))
        .ToListAsync();

    return Results.Ok(sessions);
});

app.MapGet("/events/{eventId:guid}/sessions", async (Guid eventId, SessionsDbContext db) =>
{
    var sessions = await db.Sessions
        .Where(s => s.EventId == eventId)
        .OrderBy(s => s.StartsAt)
        .Select(s => new SessionSummary(s.Id, s.EventId, s.Title, s.Speaker, s.StartsAt, s.SeatsAvailable))
        .ToListAsync();

    return Results.Ok(sessions);
});

app.MapGet("/sessions/{id:guid}", async (Guid id, SessionsDbContext db) =>
{
    var session = await db.Sessions
        .Where(s => s.Id == id)
        .Select(s => new SessionSummary(s.Id, s.EventId, s.Title, s.Speaker, s.StartsAt, s.SeatsAvailable))
        .SingleOrDefaultAsync();

    return session is null ? Results.NotFound() : Results.Ok(session);
});

app.MapGet("/sessions/count", async (SessionsDbContext db) =>
    Results.Ok(new CountResponse(await db.Sessions.CountAsync())));

app.Run();

public sealed class SessionsDbContext(DbContextOptions<SessionsDbContext> options) : DbContext(options)
{
    public DbSet<ConferenceSession> Sessions => Set<ConferenceSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConferenceSession>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Title).HasMaxLength(180);
            entity.Property(s => s.Speaker).HasMaxLength(120);
        });
    }
}

public sealed class ConferenceSession
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public required string Title { get; set; }
    public required string Speaker { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public int SeatsAvailable { get; set; }
}

public static class SessionsSeedData
{
    public static async Task EnsureSeededAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SessionsDbContext>();

        await db.Database.EnsureCreatedAsync();

        if (await db.Sessions.AnyAsync())
        {
            return;
        }

        db.Sessions.AddRange(
            new ConferenceSession
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                EventId = DemoIds.CloudNativeSummit,
                Title = "Designing service boundaries without fooling yourself",
                Speaker = "Mira Hoffmann",
                StartsAt = new DateTimeOffset(2026, 6, 16, 8, 0, 0, TimeSpan.Zero),
                SeatsAvailable = 28
            },
            new ConferenceSession
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                EventId = DemoIds.CloudNativeSummit,
                Title = "Tracing a registration across five processes",
                Speaker = "Jonas Weber",
                StartsAt = new DateTimeOffset(2026, 6, 16, 11, 30, 0, TimeSpan.Zero),
                SeatsAvailable = 16
            },
            new ConferenceSession
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                EventId = DemoIds.ApiCraftDay,
                Title = "Gateway APIs that do not become secret monoliths",
                Speaker = "Aylin Kramer",
                StartsAt = new DateTimeOffset(2026, 8, 25, 7, 30, 0, TimeSpan.Zero),
                SeatsAvailable = 35
            },
            new ConferenceSession
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
                EventId = DemoIds.ApiCraftDay,
                Title = "Contracts, versioning, and boring success",
                Speaker = "Felix Roth",
                StartsAt = new DateTimeOffset(2026, 8, 25, 12, 0, 0, TimeSpan.Zero),
                SeatsAvailable = 22
            },
            new ConferenceSession
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc1"),
                EventId = DemoIds.ArchitectureClinic,
                Title = "When a modular monolith is still the sharper tool",
                Speaker = "Nora Schmitt",
                StartsAt = new DateTimeOffset(2026, 10, 7, 9, 0, 0, TimeSpan.Zero),
                SeatsAvailable = 18
            });

        await db.SaveChangesAsync();
    }
}
