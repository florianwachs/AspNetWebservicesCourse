using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TechConf.Microservices.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<RegistrationsDbContext>("registrationsdb");
builder.AddRabbitMQClient("messaging");
builder.Services.AddSingleton<RegistrationEventPublisher>();

var app = builder.Build();

await RegistrationsSeedData.EnsureCreatedAsync(app.Services);

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Redirect("/registrations/recent"));

app.MapGet("/registrations/recent", async (RegistrationsDbContext db) =>
{
    var registrations = await db.Registrations
        .OrderByDescending(r => r.RegisteredAt)
        .Take(10)
        .Select(r => new RegistrationCreatedResponse(
            r.Id,
            r.EventId,
            r.SessionId,
            r.AttendeeName,
            r.AttendeeEmail,
            r.RegisteredAt))
        .ToListAsync();

    return Results.Ok(registrations);
});

app.MapGet("/registrations/count", async (RegistrationsDbContext db) =>
    Results.Ok(new CountResponse(await db.Registrations.CountAsync())));

app.MapPost("/registrations", async (
    RegistrationRequest request,
    RegistrationsDbContext db,
    RegistrationEventPublisher publisher,
    ILoggerFactory loggerFactory,
    CancellationToken cancellationToken) =>
{
    if (request.EventId == Guid.Empty || request.SessionId == Guid.Empty)
    {
        return Results.BadRequest(new { error = "EventId and SessionId are required." });
    }

    if (string.IsNullOrWhiteSpace(request.AttendeeName) || string.IsNullOrWhiteSpace(request.AttendeeEmail))
    {
        return Results.BadRequest(new { error = "Attendee name and email are required." });
    }

    var registration = new Registration
    {
        Id = Guid.NewGuid(),
        EventId = request.EventId,
        SessionId = request.SessionId,
        AttendeeName = request.AttendeeName.Trim(),
        AttendeeEmail = request.AttendeeEmail.Trim(),
        RegisteredAt = DateTimeOffset.UtcNow
    };

    db.Registrations.Add(registration);
    await db.SaveChangesAsync(cancellationToken);

    var message = new RegistrationCreated(
        registration.Id,
        registration.EventId,
        registration.SessionId,
        registration.AttendeeName,
        registration.AttendeeEmail,
        registration.RegisteredAt);

    await publisher.PublishAsync(message, cancellationToken);

    loggerFactory.CreateLogger("Registrations")
        .LogInformation(
            "Created registration {RegistrationId} for {Email} and published RegistrationCreated",
            registration.Id,
            registration.AttendeeEmail);

    return Results.Created($"/registrations/{registration.Id}", new RegistrationCreatedResponse(
        registration.Id,
        registration.EventId,
        registration.SessionId,
        registration.AttendeeName,
        registration.AttendeeEmail,
        registration.RegisteredAt));
});

app.Run();

public sealed class RegistrationsDbContext(DbContextOptions<RegistrationsDbContext> options) : DbContext(options)
{
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.AttendeeName).HasMaxLength(140);
            entity.Property(r => r.AttendeeEmail).HasMaxLength(180);
        });
    }
}

public sealed class Registration
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid SessionId { get; set; }
    public required string AttendeeName { get; set; }
    public required string AttendeeEmail { get; set; }
    public DateTimeOffset RegisteredAt { get; set; }
}

public sealed class RegistrationEventPublisher(IConnection connection)
{
    public async Task PublishAsync(RegistrationCreated message, CancellationToken cancellationToken)
    {
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: Messaging.RegistrationCreatedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: Messaging.RegistrationCreatedQueue,
            body: body,
            cancellationToken: cancellationToken);
    }
}

public static class RegistrationsSeedData
{
    public static async Task EnsureCreatedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RegistrationsDbContext>();

        await db.Database.EnsureCreatedAsync();
    }
}

public static class Messaging
{
    public const string RegistrationCreatedQueue = "registration-created";
}
