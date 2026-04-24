using Akka.Actor;

namespace TechConf.Akka.Api.Actors;

public sealed record SessionReservationSnapshot(Guid SessionId, string Title, int Capacity, IReadOnlyCollection<Guid> AttendeeIds);

public sealed record ValidateSeatReservation(SessionReservationSnapshot Session, Guid AttendeeId);

public sealed record ValidateSeatRelease(SessionReservationSnapshot Session, Guid AttendeeId);

public abstract record ReservationValidationResult;

public sealed record ReservationAccepted : ReservationValidationResult;

public sealed record ReservationRejectedAlreadyReserved(Guid SessionId, Guid AttendeeId) : ReservationValidationResult;

public sealed record ReservationRejectedFull(Guid SessionId, string Title) : ReservationValidationResult;

public abstract record ReleaseValidationResult;

public sealed record ReleaseAccepted : ReleaseValidationResult;

public sealed record ReleaseRejectedNotReserved(Guid SessionId, Guid AttendeeId) : ReleaseValidationResult;

public sealed class ReservationValidationActor : ReceiveActor
{
    public ReservationValidationActor()
    {
        Receive<ValidateSeatReservation>(HandleValidateSeatReservation);
        Receive<ValidateSeatRelease>(HandleValidateSeatRelease);
    }

    public static Props Create() =>
        Props.Create(() => new ReservationValidationActor());

    private void HandleValidateSeatReservation(ValidateSeatReservation message)
    {
        if (message.Session.AttendeeIds.Contains(message.AttendeeId))
        {
            Sender.Tell(new ReservationRejectedAlreadyReserved(message.Session.SessionId, message.AttendeeId));
            return;
        }

        if (message.Session.AttendeeIds.Count >= message.Session.Capacity)
        {
            Sender.Tell(new ReservationRejectedFull(message.Session.SessionId, message.Session.Title));
            return;
        }

        Sender.Tell(new ReservationAccepted());
    }

    private void HandleValidateSeatRelease(ValidateSeatRelease message)
    {
        if (!message.Session.AttendeeIds.Contains(message.AttendeeId))
        {
            Sender.Tell(new ReleaseRejectedNotReserved(message.Session.SessionId, message.AttendeeId));
            return;
        }

        Sender.Tell(new ReleaseAccepted());
    }
}
