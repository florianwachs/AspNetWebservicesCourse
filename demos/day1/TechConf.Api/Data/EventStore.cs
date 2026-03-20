using TechConf.Api.Models;

namespace TechConf.Api.Data;

public static class EventStore
{
    private static int _nextId = 4;

    // A static list keeps day 1 focused on HTTP APIs before we introduce a database.
    public static List<Event> Events { get; } =
    [
        new(1, "TechConf 2026", new DateTime(2026, 9, 15), "Munich", "A two-day conference about modern web development."),
        new(2, ".NET Conf Local", new DateTime(2026, 11, 10), "Berlin", "Community-driven .NET talks and workshops."),
        new(3, "Cloud Summit", new DateTime(2026, 6, 20), "Vienna", "Practical sessions on cloud-native architecture.")
    ];

    public static int GetNextId() => _nextId++;
}
