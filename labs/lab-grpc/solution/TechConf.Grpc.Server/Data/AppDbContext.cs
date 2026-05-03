using Microsoft.EntityFrameworkCore;
using TechConf.Grpc.Server.Models;

namespace TechConf.Grpc.Server.Data;

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
        // Seed speakers
        var speaker1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var speaker2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var speaker3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var speaker4Id = Guid.Parse("44444444-4444-4444-4444-444444444444");

        modelBuilder.Entity<Speaker>().HasData(
            new Speaker { Id = speaker1Id, Name = "Alice Johnson", Company = "Microsoft", Bio = "Cloud architect and .NET expert" },
            new Speaker { Id = speaker2Id, Name = "Bob Smith", Company = "Google", Bio = "API design specialist" },
            new Speaker { Id = speaker3Id, Name = "Carol Williams", Company = "Amazon", Bio = "Distributed systems engineer" },
            new Speaker { Id = speaker4Id, Name = "Dave Brown", Company = "GitHub", Bio = "DevOps and CI/CD advocate" }
        );

        // Seed events
        var event1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var event2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var event3Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        modelBuilder.Entity<Event>().HasData(
            new Event { Id = event1Id, Title = "TechConf 2026", Description = "Annual developer conference", StartDate = new DateTime(2026, 9, 15, 9, 0, 0), EndDate = new DateTime(2026, 9, 17, 17, 0, 0), Location = "Munich Convention Center", MaxAttendees = 500, Status = EventStatus.Published },
            new Event { Id = event2Id, Title = ".NET Conf Local", Description = "Local .NET community event", StartDate = new DateTime(2026, 11, 10, 10, 0, 0), EndDate = new DateTime(2026, 11, 10, 18, 0, 0), Location = "Berlin Tech Hub", MaxAttendees = 200, Status = EventStatus.Published },
            new Event { Id = event3Id, Title = "Cloud Summit", Description = "Cloud-native architecture conference", StartDate = new DateTime(2026, 6, 20, 9, 0, 0), EndDate = new DateTime(2026, 6, 22, 17, 0, 0), Location = "Vienna Conference Center", MaxAttendees = 300, Status = EventStatus.Draft }
        );

        // Seed sessions
        modelBuilder.Entity<Session>().HasData(
            new Session { Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"), Title = "Building APIs with .NET 10", Description = "Deep dive into Minimal APIs", StartTime = new DateTime(2026, 9, 15, 10, 0, 0), EndTime = new DateTime(2026, 9, 15, 11, 30, 0), Room = "Hall A", EventId = event1Id, SpeakerId = speaker1Id },
            new Session { Id = Guid.Parse("d2222222-2222-2222-2222-222222222222"), Title = "GraphQL in Practice", Description = "Hot Chocolate workshop", StartTime = new DateTime(2026, 9, 15, 13, 0, 0), EndTime = new DateTime(2026, 9, 15, 14, 30, 0), Room = "Hall B", EventId = event1Id, SpeakerId = speaker2Id },
            new Session { Id = Guid.Parse("d3333333-3333-3333-3333-333333333333"), Title = "Cloud-Native Patterns", Description = "Microservices architecture", StartTime = new DateTime(2026, 9, 16, 10, 0, 0), EndTime = new DateTime(2026, 9, 16, 11, 30, 0), Room = "Hall A", EventId = event1Id, SpeakerId = speaker3Id },
            new Session { Id = Guid.Parse("d4444444-4444-4444-4444-444444444444"), Title = "CI/CD with GitHub Actions", Description = "Automated deployments", StartTime = new DateTime(2026, 9, 16, 13, 0, 0), EndTime = new DateTime(2026, 9, 16, 14, 30, 0), Room = "Hall B", EventId = event1Id, SpeakerId = speaker4Id },
            new Session { Id = Guid.Parse("d5555555-5555-5555-5555-555555555555"), Title = "What's New in .NET 10", Description = "Overview of new features", StartTime = new DateTime(2026, 11, 10, 10, 30, 0), EndTime = new DateTime(2026, 11, 10, 12, 0, 0), Room = "Main Room", EventId = event2Id, SpeakerId = speaker1Id },
            new Session { Id = Guid.Parse("d6666666-6666-6666-6666-666666666666"), Title = "Kubernetes for .NET Developers", Description = "Container orchestration basics", StartTime = new DateTime(2026, 11, 10, 14, 0, 0), EndTime = new DateTime(2026, 11, 10, 15, 30, 0), Room = "Main Room", EventId = event2Id, SpeakerId = speaker3Id }
        );

        // Seed attendees
        var attendee1Id = Guid.Parse("e1111111-1111-1111-1111-111111111111");
        var attendee2Id = Guid.Parse("e2222222-2222-2222-2222-222222222222");
        var attendee3Id = Guid.Parse("e3333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Attendee>().HasData(
            new Attendee { Id = attendee1Id, Name = "Eva Martinez", Email = "eva@example.com" },
            new Attendee { Id = attendee2Id, Name = "Frank Lee", Email = "frank@example.com" },
            new Attendee { Id = attendee3Id, Name = "Grace Chen", Email = "grace@example.com" }
        );

        // Seed registrations
        modelBuilder.Entity<Registration>().HasData(
            new Registration { Id = Guid.Parse("f1111111-1111-1111-1111-111111111111"), EventId = event1Id, AttendeeId = attendee1Id, RegisteredAt = new DateTime(2026, 1, 15) },
            new Registration { Id = Guid.Parse("f2222222-2222-2222-2222-222222222222"), EventId = event1Id, AttendeeId = attendee2Id, RegisteredAt = new DateTime(2026, 2, 1) },
            new Registration { Id = Guid.Parse("f3333333-3333-3333-3333-333333333333"), EventId = event2Id, AttendeeId = attendee1Id, RegisteredAt = new DateTime(2026, 3, 10) },
            new Registration { Id = Guid.Parse("f4444444-4444-4444-4444-444444444444"), EventId = event2Id, AttendeeId = attendee3Id, RegisteredAt = new DateTime(2026, 3, 12) },
            new Registration { Id = Guid.Parse("f5555555-5555-5555-5555-555555555555"), EventId = event3Id, AttendeeId = attendee2Id, RegisteredAt = new DateTime(2026, 4, 5) }
        );
    }
}
