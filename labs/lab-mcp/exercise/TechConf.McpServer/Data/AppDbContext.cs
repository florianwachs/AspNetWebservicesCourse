using Microsoft.EntityFrameworkCore;
using TechConf.McpServer.Models;

namespace TechConf.McpServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Speaker> Speakers => Set<Speaker>();
    public DbSet<Attendee> Attendees => Set<Attendee>();
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Speakers
        var speakers = new[]
        {
            new Speaker { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Alice Johnson", Company = "Microsoft", Bio = "Cloud architect and .NET expert with 15 years of experience" },
            new Speaker { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Bob Smith", Company = "Google", Bio = "Distributed systems engineer and Kubernetes specialist" },
            new Speaker { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Carol Williams", Company = "Amazon", Bio = "Serverless computing advocate and AWS solutions architect" },
            new Speaker { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Dave Brown", Company = "GitHub", Bio = "DevOps lead and open-source contributor" },
        };
        modelBuilder.Entity<Speaker>().HasData(speakers);

        // Events
        var events = new[]
        {
            new Event { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Title = "TechConf 2026", Description = "The premier technology conference covering cloud, AI, and modern development", StartDate = new DateTime(2026, 6, 15), EndDate = new DateTime(2026, 6, 17), Location = "Munich, Germany", MaxAttendees = 500, Status = EventStatus.Published },
            new Event { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Title = ".NET Conf Local 2026", Description = "A local community event focused on the latest in .NET development", StartDate = new DateTime(2026, 9, 20), EndDate = new DateTime(2026, 9, 21), Location = "Berlin, Germany", MaxAttendees = 200, Status = EventStatus.Published },
            new Event { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Title = "Cloud Summit 2026", Description = "Deep dive into cloud-native architectures and DevOps practices", StartDate = new DateTime(2026, 11, 5), EndDate = new DateTime(2026, 11, 7), Location = "Vienna, Austria", MaxAttendees = 300, Status = EventStatus.Draft },
        };
        modelBuilder.Entity<Event>().HasData(events);

        // Sessions
        var sessions = new[]
        {
            new { Id = Guid.Parse("d1111111-d111-d111-d111-d11111111111"), Title = "Keynote: The Future of .NET", Description = "Opening keynote covering the .NET roadmap and vision", StartTime = new DateTime(2026, 6, 15, 9, 0, 0), EndTime = new DateTime(2026, 6, 15, 10, 0, 0), Room = "Main Hall", EventId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), SpeakerId = Guid.Parse("11111111-1111-1111-1111-111111111111") },
            new { Id = Guid.Parse("d2222222-d222-d222-d222-d22222222222"), Title = "Building Scalable Microservices", Description = "Patterns and practices for microservice architecture", StartTime = new DateTime(2026, 6, 15, 10, 30, 0), EndTime = new DateTime(2026, 6, 15, 11, 30, 0), Room = "Room A", EventId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), SpeakerId = Guid.Parse("22222222-2222-2222-2222-222222222222") },
            new { Id = Guid.Parse("d3333333-d333-d333-d333-d33333333333"), Title = "Serverless with Azure Functions", Description = "Building event-driven applications with serverless computing", StartTime = new DateTime(2026, 6, 16, 9, 0, 0), EndTime = new DateTime(2026, 6, 16, 10, 0, 0), Room = "Room B", EventId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), SpeakerId = Guid.Parse("33333333-3333-3333-3333-333333333333") },
            new { Id = Guid.Parse("d4444444-d444-d444-d444-d44444444444"), Title = "CI/CD with GitHub Actions", Description = "Automating your deployment pipeline with GitHub Actions", StartTime = new DateTime(2026, 6, 16, 10, 30, 0), EndTime = new DateTime(2026, 6, 16, 11, 30, 0), Room = "Room A", EventId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), SpeakerId = Guid.Parse("44444444-4444-4444-4444-444444444444") },
            new { Id = Guid.Parse("d5555555-d555-d555-d555-d55555555555"), Title = "What's New in C# 14", Description = "Exploring the latest C# language features", StartTime = new DateTime(2026, 9, 20, 9, 0, 0), EndTime = new DateTime(2026, 9, 20, 10, 0, 0), Room = "Main Hall", EventId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), SpeakerId = Guid.Parse("11111111-1111-1111-1111-111111111111") },
            new { Id = Guid.Parse("d6666666-d666-d666-d666-d66666666666"), Title = "Kubernetes for .NET Developers", Description = "Deploying and managing .NET applications on Kubernetes", StartTime = new DateTime(2026, 9, 20, 10, 30, 0), EndTime = new DateTime(2026, 9, 20, 11, 30, 0), Room = "Room A", EventId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), SpeakerId = Guid.Parse("22222222-2222-2222-2222-222222222222") },
        };
        modelBuilder.Entity<Session>().HasData(sessions);

        // Attendees
        var attendees = new[]
        {
            new Attendee { Id = Guid.Parse("e1111111-e111-e111-e111-e11111111111"), Name = "Eva Martinez", Email = "eva.martinez@example.com" },
            new Attendee { Id = Guid.Parse("e2222222-e222-e222-e222-e22222222222"), Name = "Frank Lee", Email = "frank.lee@example.com" },
            new Attendee { Id = Guid.Parse("e3333333-e333-e333-e333-e33333333333"), Name = "Grace Chen", Email = "grace.chen@example.com" },
        };
        modelBuilder.Entity<Attendee>().HasData(attendees);

        // Registrations
        var registrations = new[]
        {
            new { Id = Guid.Parse("f1111111-f111-f111-f111-f11111111111"), RegisteredAt = new DateTime(2026, 3, 1), EventId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), AttendeeId = Guid.Parse("e1111111-e111-e111-e111-e11111111111") },
            new { Id = Guid.Parse("f2222222-f222-f222-f222-f22222222222"), RegisteredAt = new DateTime(2026, 3, 5), EventId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), AttendeeId = Guid.Parse("e2222222-e222-e222-e222-e22222222222") },
            new { Id = Guid.Parse("f3333333-f333-f333-f333-f33333333333"), RegisteredAt = new DateTime(2026, 3, 10), EventId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), AttendeeId = Guid.Parse("e3333333-e333-e333-e333-e33333333333") },
            new { Id = Guid.Parse("f4444444-f444-f444-f444-f44444444444"), RegisteredAt = new DateTime(2026, 7, 1), EventId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), AttendeeId = Guid.Parse("e1111111-e111-e111-e111-e11111111111") },
            new { Id = Guid.Parse("f5555555-f555-f555-f555-f55555555555"), RegisteredAt = new DateTime(2026, 7, 15), EventId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), AttendeeId = Guid.Parse("e2222222-e222-e222-e222-e22222222222") },
        };
        modelBuilder.Entity<Registration>().HasData(registrations);
    }
}
