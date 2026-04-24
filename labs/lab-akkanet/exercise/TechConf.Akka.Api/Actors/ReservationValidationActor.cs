using Akka.Actor;
using TechConf.Akka.Api.Models;

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
        // TODO Task 2 - validate reserve requests in the second actor.
        // Return:
        // 1. ReservationRejectedAlreadyReserved when the attendee already has a seat
        // 2. ReservationRejectedFull when capacity is exhausted
        // 3. ReservationAccepted when the reservation is allowed
        Sender.Tell(new ErrorResponse("TODO: Implement reserve validation in ReservationValidationActor."));
    }

    private void HandleValidateSeatRelease(ValidateSeatRelease message)
    {
        // TODO Task 5 - validate release requests in the second actor.
        // Return:
        // 1. ReleaseRejectedNotReserved when the attendee has no seat
        // 2. ReleaseAccepted when the release is allowed
        Sender.Tell(new ErrorResponse("TODO: Implement release validation in ReservationValidationActor."));
    }
}
