using Akka.Actor;
using Akka.Hosting;
using TechConf.Akka.Api.Actors;
using TechConf.Akka.Api.Models;

namespace TechConf.Akka.Api.Endpoints;

public static class ReservationEndpoints
{
    public static IEndpointRouteBuilder MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sessions")
            .WithTags("Sessions");

        group.MapGet("/", GetAllSessions);
        group.MapGet("/{id:guid}", GetSessionById);
        group.MapPost("/{id:guid}/reserve", ReserveSeatForSession);
        group.MapPost("/{id:guid}/release", ReleaseSeatForSession);

        return app;
    }

    private static async Task<IResult> GetAllSessions(ActorRegistry registry, CancellationToken cancellationToken)
    {
        var response = await AskSeatReservations(registry, new ListSessions(), cancellationToken);

        return response switch
        {
            SessionListResponse sessions => Results.Ok(sessions),
            _ => Results.Problem("Unexpected actor response while listing sessions.")
        };
    }

    private static async Task<IResult> GetSessionById(
        Guid id,
        ActorRegistry registry,
        CancellationToken cancellationToken)
    {
        var response = await AskSeatReservations(registry, new GetSession(id), cancellationToken);

        return response switch
        {
            SessionDetails session => Results.Ok(session),
            SessionNotFound notFound => Results.NotFound(new ErrorResponse($"Session '{notFound.SessionId}' was not found.")),
            _ => Results.Problem("Unexpected actor response while loading the session.")
        };
    }

    private static async Task<IResult> ReserveSeatForSession(
        Guid id,
        ReserveSeatRequest request,
        ActorRegistry registry,
        CancellationToken cancellationToken)
    {
        var response = await AskSeatReservations(registry, new ReserveSeat(id, request.AttendeeId), cancellationToken);

        return response switch
        {
            SeatReserved reserved => Results.Ok(reserved.Session),
            SessionNotFound notFound => Results.NotFound(new ErrorResponse($"Session '{notFound.SessionId}' was not found.")),
            SessionFull full => Results.Conflict(new ErrorResponse($"Session '{full.Title}' is already full.")),
            SeatAlreadyReserved => Results.Conflict(new ErrorResponse("This attendee already holds a reservation.")),
            _ => Results.Problem("Unexpected actor response while reserving a seat.")
        };
    }

    private static async Task<IResult> ReleaseSeatForSession(
        Guid id,
        ReleaseSeatRequest request,
        ActorRegistry registry,
        CancellationToken cancellationToken)
    {
        var response = await AskSeatReservations(registry, new ReleaseSeat(id, request.AttendeeId), cancellationToken);

        return response switch
        {
            SeatReleased released => Results.Ok(released.Session),
            SessionNotFound notFound => Results.NotFound(new ErrorResponse($"Session '{notFound.SessionId}' was not found.")),
            SeatNotReserved => Results.Conflict(new ErrorResponse("This attendee does not currently hold a reservation.")),
            _ => Results.Problem("Unexpected actor response while releasing a seat.")
        };
    }

    private static async Task<object> AskSeatReservations(
        ActorRegistry registry,
        object message,
        CancellationToken cancellationToken)
    {
        var actor = registry.Get<SeatReservationsActorKey>();
        return await actor.Ask<object>(message, cancellationToken).ConfigureAwait(false);
    }
}
