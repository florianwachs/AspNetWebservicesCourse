using Microsoft.EntityFrameworkCore;
using TechConf.Microservices.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<EventsDbContext>("eventsdb");

var app = builder.Build();

await EventsSeedData.EnsureSeededAsync(app.Services);

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Redirect("/events"));

app.MapGet("/events", async (EventsDbContext db) =>
{
    var events = await db.Events
        .OrderBy(e => e.StartsOn)
        .Select(e => new EventSummary(e.Id, e.Name, e.City, e.StartsOn, e.EndsOn, e.Status))
        .ToListAsync();

    return Results.Ok(events);
});

app.MapGet("/events/{id:guid}", async (Guid id, EventsDbContext db) =>
{
    var eventSummary = await db.Events
        .Where(e => e.Id == id)
        .Select(e => new EventSummary(e.Id, e.Name, e.City, e.StartsOn, e.EndsOn, e.Status))
        .SingleOrDefaultAsync();

    return eventSummary is null ? Results.NotFound() : Results.Ok(eventSummary);
});

app.MapGet("/events/count", async (EventsDbContext db) =>
    Results.Ok(new CountResponse(await db.Events.CountAsync())));

app.Run();

public sealed class EventsDbContext(DbContextOptions<EventsDbContext> options) : DbContext(options)
{
    public DbSet<ConferenceEvent> Events => Set<ConferenceEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConferenceEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(160);
            entity.Property(e => e.City).HasMaxLength(80);
            entity.Property(e => e.Status).HasMaxLength(40);
        });
    }
}

public sealed class ConferenceEvent
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string City { get; set; }
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }
    public required string Status { get; set; }
}

public static class EventsSeedData
{
    public static async Task EnsureSeededAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EventsDbContext>();

        await db.Database.EnsureCreatedAsync();

        if (await db.Events.AnyAsync())
        {
            return;
        }

        db.Events.AddRange(
            new ConferenceEvent
            {
                Id = DemoIds.CloudNativeSummit,
                Name = "Cloud Native Summit",
                City = "Berlin",
                StartsOn = new DateOnly(2026, 6, 16),
                EndsOn = new DateOnly(2026, 6, 18),
                Status = "Registration open"
            },
            new ConferenceEvent
            {
                Id = DemoIds.ApiCraftDay,
                Name = "API Craft Day",
                City = "Hamburg",
                StartsOn = new DateOnly(2026, 8, 25),
                EndsOn = new DateOnly(2026, 8, 25),
                Status = "Agenda published"
            },
            new ConferenceEvent
            {
                Id = DemoIds.ArchitectureClinic,
                Name = "Architecture Clinic",
                City = "Munich",
                StartsOn = new DateOnly(2026, 10, 7),
                EndsOn = new DateOnly(2026, 10, 8),
                Status = "Call for papers"
            });

        await db.SaveChangesAsync();
    }
}
