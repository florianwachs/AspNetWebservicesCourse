namespace TechConf.Akka.Api.Models;

public sealed record SessionDefinition(Guid Id, string Title, int Capacity);

public sealed record SessionSummary(Guid Id, string Title, int Capacity, int ReservedSeats, int AvailableSeats);

public sealed record SessionDetails(Guid Id, string Title, int Capacity, IReadOnlyCollection<Guid> AttendeeIds)
{
    public int ReservedSeats => AttendeeIds.Count;
    public int AvailableSeats => Capacity - ReservedSeats;
}

public sealed record SessionListResponse(IReadOnlyCollection<SessionSummary> Sessions);

public sealed record ReserveSeatRequest(Guid AttendeeId);

public sealed record ReleaseSeatRequest(Guid AttendeeId);

public sealed record ErrorResponse(string Message);

public static class SessionCatalog
{
    public static readonly SessionDefinition[] All =
    [
        new(
            Guid.Parse("8f2c38b7-2b10-4dc7-9174-4b4f8d0ac001"),
            "High-Scale Seat Reservations with Actors",
            2),
        new(
            Guid.Parse("8f2c38b7-2b10-4dc7-9174-4b4f8d0ac002"),
            "Event-Driven Workflows without Shared State",
            3),
        new(
            Guid.Parse("8f2c38b7-2b10-4dc7-9174-4b4f8d0ac003"),
            "Streaming Conference Updates with WebSockets",
            4)
    ];
}
