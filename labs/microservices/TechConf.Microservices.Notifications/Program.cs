using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TechConf.Microservices.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<NotificationsDbContext>("notificationsdb");
builder.AddRabbitMQClient("messaging");
builder.Services.AddHostedService<RegistrationNotificationWorker>();

var app = builder.Build();

await NotificationsSeedData.EnsureCreatedAsync(app.Services);

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Redirect("/notifications/recent"));

app.MapGet("/notifications/recent", async (NotificationsDbContext db) =>
{
    var notifications = await db.Notifications
        .OrderByDescending(n => n.CreatedAt)
        .Take(12)
        .Select(n => new NotificationSummary(
            n.Id,
            n.RegistrationId,
            n.Recipient,
            n.Subject,
            n.Channel,
            n.CreatedAt,
            n.Status))
        .ToListAsync();

    return Results.Ok(notifications);
});

app.MapGet("/notifications/count", async (NotificationsDbContext db) =>
    Results.Ok(new CountResponse(await db.Notifications.CountAsync())));

app.Run();

public sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public DbSet<NotificationRecord> Notifications => Set<NotificationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationRecord>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Recipient).HasMaxLength(180);
            entity.Property(n => n.Subject).HasMaxLength(220);
            entity.Property(n => n.Channel).HasMaxLength(40);
            entity.Property(n => n.Status).HasMaxLength(40);
        });
    }
}

public sealed class NotificationRecord
{
    public Guid Id { get; set; }
    public Guid RegistrationId { get; set; }
    public required string Recipient { get; set; }
    public required string Subject { get; set; }
    public required string Channel { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public required string Status { get; set; }
}

public sealed class RegistrationNotificationWorker(
    IConnection connection,
    IServiceScopeFactory scopeFactory,
    ILogger<RegistrationNotificationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: Messaging.RegistrationCreatedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            var json = Encoding.UTF8.GetString(args.Body.Span);
            var message = JsonSerializer.Deserialize<RegistrationCreated>(json);

            if (message is null)
            {
                logger.LogWarning("Ignored invalid registration notification payload: {Payload}", json);
                return;
            }

            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

            db.Notifications.Add(new NotificationRecord
            {
                Id = Guid.NewGuid(),
                RegistrationId = message.RegistrationId,
                Recipient = message.AttendeeEmail,
                Subject = $"Registration confirmed for session {message.SessionId}",
                Channel = "email",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = "Queued"
            });

            await db.SaveChangesAsync(stoppingToken);

            logger.LogInformation(
                "Stored notification for registration {RegistrationId} to {Recipient}",
                message.RegistrationId,
                message.AttendeeEmail);
        };

        await channel.BasicConsumeAsync(
            queue: Messaging.RegistrationCreatedQueue,
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);

        logger.LogInformation("Notification worker is consuming RabbitMQ queue {Queue}", Messaging.RegistrationCreatedQueue);

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Notification worker is stopping");
        }
    }
}

public static class NotificationsSeedData
{
    public static async Task EnsureCreatedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        await db.Database.EnsureCreatedAsync();
    }
}

public static class Messaging
{
    public const string RegistrationCreatedQueue = "registration-created";
}
