using Akka.Actor;
using TechConf.Akka.Api.Models;

namespace TechConf.Akka.Api.Actors;

public sealed class SeatReservationsActorKey;

public sealed record ListSessions;

public sealed record GetSession(Guid SessionId);

public sealed record ReserveSeat(Guid SessionId, Guid AttendeeId);

public sealed record ReleaseSeat(Guid SessionId, Guid AttendeeId);

public sealed record SeatReserved(SessionDetails Session);

public sealed record SeatReleased(SessionDetails Session);

public sealed record SessionNotFound(Guid SessionId);

public sealed record SessionFull(Guid SessionId, string Title);

public sealed record SeatAlreadyReserved(Guid SessionId, Guid AttendeeId);

public sealed record SeatNotReserved(Guid SessionId, Guid AttendeeId);

public sealed class SeatReservationsActor : ReceiveActor
{
    private static readonly TimeSpan ValidationTimeout = TimeSpan.FromSeconds(3);

    private readonly Dictionary<Guid, SessionState> _sessions;
    private readonly IActorRef _reservationValidation;

    public SeatReservationsActor(IEnumerable<SessionDefinition> sessions)
    {
        _sessions = sessions.ToDictionary(
            session => session.Id,
            session => new SessionState(session.Id, session.Title, session.Capacity));
        _reservationValidation = Context.ActorOf(ReservationValidationActor.Create(), "reservation-validation");

        Receive<ListSessions>(_ => Sender.Tell(new SessionListResponse(
            _sessions.Values
                .Select(ToSummary)
                .OrderBy(summary => summary.Title)
                .ToArray())));

        Receive<GetSession>(message =>
        {
            var response = TryGetSession(message.SessionId) ?? (object)new SessionNotFound(message.SessionId);
            Sender.Tell(response);
        });

        ReceiveAsync<ReserveSeat>(HandleReserveSeatAsync);
        ReceiveAsync<ReleaseSeat>(HandleReleaseSeatAsync);
    }

    public static Props Create(IEnumerable<SessionDefinition> sessions) =>
        Props.Create(() => new SeatReservationsActor(sessions));

    private async Task HandleReserveSeatAsync(ReserveSeat message)
    {
        var replyTo = Sender;

        if (!_sessions.TryGetValue(message.SessionId, out var session))
        {
            replyTo.Tell(new SessionNotFound(message.SessionId));
            return;
        }

        var validation = await _reservationValidation.Ask<object>(
            new ValidateSeatReservation(ToSnapshot(session), message.AttendeeId),
            ValidationTimeout);

        // TODO Task 3 - ask the validation actor before mutating reservation state.
        // Map the response like this:
        // 1. ReservationAccepted -> add the attendee and reply with SeatReserved
        // 2. ReservationRejectedAlreadyReserved -> reply with SeatAlreadyReserved
        // 3. ReservationRejectedFull -> reply with SessionFull
        // 4. ErrorResponse -> bubble the starter TODO message through unchanged
        replyTo.Tell(validation switch
        {
            ErrorResponse error => error,
            _ => new ErrorResponse("TODO: Map reserve validation results in SeatReservationsActor.")
        });
    }

    private async Task HandleReleaseSeatAsync(ReleaseSeat message)
    {
        var replyTo = Sender;

        if (!_sessions.TryGetValue(message.SessionId, out var session))
        {
            replyTo.Tell(new SessionNotFound(message.SessionId));
            return;
        }

        var validation = await _reservationValidation.Ask<object>(
            new ValidateSeatRelease(ToSnapshot(session), message.AttendeeId),
            ValidationTimeout);

        // TODO Task 5 - ask the validation actor before removing a reservation.
        // Map the response like this:
        // 1. ReleaseAccepted -> remove the attendee and reply with SeatReleased
        // 2. ReleaseRejectedNotReserved -> reply with SeatNotReserved
        // 3. ErrorResponse -> bubble the starter TODO message through unchanged
        replyTo.Tell(validation switch
        {
            ErrorResponse error => error,
            _ => new ErrorResponse("TODO: Map release validation results in SeatReservationsActor.")
        });
    }

    private SessionDetails? TryGetSession(Guid sessionId) =>
        _sessions.TryGetValue(sessionId, out var session)
            ? ToDetails(session)
            : null;

    private static SessionSummary ToSummary(SessionState session) =>
        new(
            session.Id,
            session.Title,
            session.Capacity,
            session.AttendeeIds.Count,
            session.Capacity - session.AttendeeIds.Count);

    private static SessionReservationSnapshot ToSnapshot(SessionState session) =>
        new(
            session.Id,
            session.Title,
            session.Capacity,
            session.AttendeeIds.OrderBy(id => id).ToArray());

    private static SessionDetails ToDetails(SessionState session) =>
        new(
            session.Id,
            session.Title,
            session.Capacity,
            session.AttendeeIds.OrderBy(id => id).ToArray());

    private sealed class SessionState(Guid id, string title, int capacity)
    {
        public Guid Id { get; } = id;
        public string Title { get; } = title;
        public int Capacity { get; } = capacity;
        public HashSet<Guid> AttendeeIds { get; } = [];
    }
}
