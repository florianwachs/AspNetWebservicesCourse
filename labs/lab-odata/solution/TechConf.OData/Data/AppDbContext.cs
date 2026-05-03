using Microsoft.EntityFrameworkCore;
using TechConf.OData.Models;

namespace TechConf.OData.Data;

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
        // Speakers
        var speaker1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var speaker2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var speaker3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var speaker4Id = Guid.Parse("44444444-4444-4444-4444-444444444444");

        modelBuilder.Entity<Speaker>().HasData(
            new Speaker { Id = speaker1Id, Name = "Alice Johnson", Company = "Microsoft", Bio = "Cloud architecture expert and Azure MVP" },
            new Speaker { Id = speaker2Id, Name = "Bob Smith", Company = "Google", Bio = "Kubernetes and cloud-native specialist" },
            new Speaker { Id = speaker3Id, Name = "Clara Weber", Company = "JetBrains", Bio = "Developer tooling and IDE productivity advocate" },
            new Speaker { Id = speaker4Id, Name = "David Kim", Company = "Amazon", Bio = "Serverless computing and event-driven architecture lead" }
        );

        // Events
        var event1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var event2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var event3Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        modelBuilder.Entity<Event>().HasData(
            new Event { Id = event1Id, Title = "TechConf 2026", Description = "The premier tech conference in Bavaria", StartDate = new DateTime(2026, 9, 15, 9, 0, 0), EndDate = new DateTime(2026, 9, 17, 17, 0, 0), Location = "Munich", MaxAttendees = 500, Status = "Published" },
            new Event { Id = event2Id, Title = ".NET Conf Local", Description = "Local .NET community event", StartDate = new DateTime(2026, 11, 10, 9, 0, 0), EndDate = new DateTime(2026, 11, 10, 18, 0, 0), Location = "Berlin", MaxAttendees = 200, Status = "Draft" },
            new Event { Id = event3Id, Title = "Cloud Summit", Description = "Cloud-native development conference", StartDate = new DateTime(2026, 6, 20, 9, 0, 0), EndDate = new DateTime(2026, 6, 21, 17, 0, 0), Location = "Vienna", MaxAttendees = 300, Status = "Published" }
        );

        // Sessions
        var session1Id = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");
        var session2Id = Guid.Parse("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2");
        var session3Id = Guid.Parse("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1");
        var session4Id = Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2");
        var session5Id = Guid.Parse("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1");
        var session6Id = Guid.Parse("c2c2c2c2-c2c2-c2c2-c2c2-c2c2c2c2c2c2");

        modelBuilder.Entity<Session>().HasData(
            new Session { Id = session1Id, Title = "Building Scalable APIs with ASP.NET Core", Description = "Learn how to build high-performance REST APIs", StartTime = new DateTime(2026, 9, 15, 10, 0, 0), EndTime = new DateTime(2026, 9, 15, 11, 30, 0), Room = "Hall A", EventId = event1Id, SpeakerId = speaker1Id },
            new Session { Id = session2Id, Title = "Kubernetes for .NET Developers", Description = "Deploy and manage .NET apps on Kubernetes", StartTime = new DateTime(2026, 9, 15, 13, 0, 0), EndTime = new DateTime(2026, 9, 15, 14, 30, 0), Room = "Hall B", EventId = event1Id, SpeakerId = speaker2Id },
            new Session { Id = session3Id, Title = "What's New in .NET 10", Description = "Overview of the latest .NET features", StartTime = new DateTime(2026, 11, 10, 10, 0, 0), EndTime = new DateTime(2026, 11, 10, 11, 30, 0), Room = "Main Hall", EventId = event2Id, SpeakerId = speaker3Id },
            new Session { Id = session4Id, Title = "Blazor Full-Stack Development", Description = "Build interactive web apps with Blazor", StartTime = new DateTime(2026, 11, 10, 13, 0, 0), EndTime = new DateTime(2026, 11, 10, 14, 30, 0), Room = "Main Hall", EventId = event2Id, SpeakerId = speaker1Id },
            new Session { Id = session5Id, Title = "Serverless with AWS Lambda", Description = "Event-driven serverless architectures", StartTime = new DateTime(2026, 6, 20, 10, 0, 0), EndTime = new DateTime(2026, 6, 20, 11, 30, 0), Room = "Room 1", EventId = event3Id, SpeakerId = speaker4Id },
            new Session { Id = session6Id, Title = "Cloud-Native Observability", Description = "Monitoring and tracing in distributed systems", StartTime = new DateTime(2026, 6, 20, 13, 0, 0), EndTime = new DateTime(2026, 6, 20, 14, 30, 0), Room = "Room 2", EventId = event3Id, SpeakerId = speaker2Id }
        );

        // Attendees
        var attendee1Id = Guid.Parse("ee111111-ee11-ee11-ee11-ee1111111111");
        var attendee2Id = Guid.Parse("ee222222-ee22-ee22-ee22-ee2222222222");
        var attendee3Id = Guid.Parse("ee333333-ee33-ee33-ee33-ee3333333333");

        modelBuilder.Entity<Attendee>().HasData(
            new Attendee { Id = attendee1Id, Name = "Max Müller", Email = "max.mueller@example.com" },
            new Attendee { Id = attendee2Id, Name = "Sophie Bauer", Email = "sophie.bauer@example.com" },
            new Attendee { Id = attendee3Id, Name = "Liam O'Brien", Email = "liam.obrien@example.com" }
        );

        // Registrations
        modelBuilder.Entity<Registration>().HasData(
            new Registration { Id = Guid.Parse("ff111111-ff11-ff11-ff11-ff1111111111"), RegisteredAt = new DateTime(2026, 7, 1), EventId = event1Id, AttendeeId = attendee1Id },
            new Registration { Id = Guid.Parse("ff222222-ff22-ff22-ff22-ff2222222222"), RegisteredAt = new DateTime(2026, 7, 5), EventId = event1Id, AttendeeId = attendee2Id },
            new Registration { Id = Guid.Parse("ff333333-ff33-ff33-ff33-ff3333333333"), RegisteredAt = new DateTime(2026, 7, 10), EventId = event3Id, AttendeeId = attendee1Id },
            new Registration { Id = Guid.Parse("ff444444-ff44-ff44-ff44-ff4444444444"), RegisteredAt = new DateTime(2026, 7, 15), EventId = event3Id, AttendeeId = attendee3Id },
            new Registration { Id = Guid.Parse("ff555555-ff55-ff55-ff55-ff5555555555"), RegisteredAt = new DateTime(2026, 8, 1), EventId = event1Id, AttendeeId = attendee3Id }
        );
    }
}
