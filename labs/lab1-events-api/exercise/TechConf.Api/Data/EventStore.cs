using TechConf.Api.Models;

namespace TechConf.Api.Data;

public static class EventStore
{
    private static int _nextId = 4;

    public static List<Event> Events { get; } =
    [
        new(1, "TechConf 2026", new DateTime(2026, 9, 15), "Munich", "The premier tech conference in Bavaria"),
        new(2, ".NET Conf Local", new DateTime(2026, 11, 10), "Berlin", "Local .NET community event"),
        new(3, "Cloud Summit", new DateTime(2026, 6, 20), "Vienna", "Cloud-native development conference")
    ];

    public static int GetNextId() => _nextId++;
}
