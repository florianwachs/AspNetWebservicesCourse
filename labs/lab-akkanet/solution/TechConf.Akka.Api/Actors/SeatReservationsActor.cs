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

        var validation = await _reservationValidation.Ask<ReservationValidationResult>(
            new ValidateSeatReservation(ToSnapshot(session), message.AttendeeId),
            ValidationTimeout);

        switch (validation)
        {
            case ReservationAccepted:
                session.AttendeeIds.Add(message.AttendeeId);
                replyTo.Tell(new SeatReserved(ToDetails(session)));
                break;
            case ReservationRejectedAlreadyReserved alreadyReserved:
                replyTo.Tell(new SeatAlreadyReserved(alreadyReserved.SessionId, alreadyReserved.AttendeeId));
                break;
            case ReservationRejectedFull full:
                replyTo.Tell(new SessionFull(full.SessionId, full.Title));
                break;
            default:
                throw new InvalidOperationException($"Unexpected validation response '{validation.GetType().Name}'.");
        }
    }

    private async Task HandleReleaseSeatAsync(ReleaseSeat message)
    {
        var replyTo = Sender;

        if (!_sessions.TryGetValue(message.SessionId, out var session))
        {
            replyTo.Tell(new SessionNotFound(message.SessionId));
            return;
        }

        var validation = await _reservationValidation.Ask<ReleaseValidationResult>(
            new ValidateSeatRelease(ToSnapshot(session), message.AttendeeId),
            ValidationTimeout);

        switch (validation)
        {
            case ReleaseAccepted:
                session.AttendeeIds.Remove(message.AttendeeId);
                replyTo.Tell(new SeatReleased(ToDetails(session)));
                break;
            case ReleaseRejectedNotReserved notReserved:
                replyTo.Tell(new SeatNotReserved(notReserved.SessionId, notReserved.AttendeeId));
                break;
            default:
                throw new InvalidOperationException($"Unexpected validation response '{validation.GetType().Name}'.");
        }
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
