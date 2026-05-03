using TechConf.Api.Models;

namespace TechConf.Api.Data;

public static class EventStore
{
    public static List<Event> Events { get; } =
    [
        new(
            1,
            "TechConf 2026",
            "The flagship conference for modern .NET and cloud-native architecture.",
            new DateTime(2026, 9, 15, 9, 0, 0),
            new DateTime(2026, 9, 15, 17, 0, 0),
            new Venue("Main Hall", "Rosenheimer Str. 10", "Munich", 500),
            342,
            EventStatus.Published),
        new(
            2,
            ".NET Conf Local",
            "A community-driven event focused on practical ASP.NET Core topics.",
            new DateTime(2026, 11, 10, 10, 0, 0),
            new DateTime(2026, 11, 10, 16, 30, 0),
            new Venue("Innovation Hub", "Invalidenstr. 117", "Berlin", 180),
            126,
            EventStatus.Published),
        new(
            3,
            "Cloud Summit",
            "Hands-on sessions about distributed systems, observability, and messaging.",
            new DateTime(2026, 6, 20, 8, 30, 0),
            new DateTime(2026, 6, 20, 18, 0, 0),
            new Venue("Expo Center", "Praterstr. 1", "Vienna", 350),
            214,
            EventStatus.Draft)
    ];
}
