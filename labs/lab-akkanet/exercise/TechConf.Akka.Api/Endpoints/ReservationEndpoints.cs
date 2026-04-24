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

    private static Task<IResult> GetAllSessions(ActorRegistry registry, CancellationToken cancellationToken)
    {
        // TODO Task 1 - ask the actor for ListSessions and return Results.Ok with the response payload.
        return Task.FromResult<IResult>(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    private static Task<IResult> GetSessionById(Guid id, ActorRegistry registry, CancellationToken cancellationToken)
    {
        // TODO Task 1 - ask the actor for GetSession(id) and map SessionNotFound to Results.NotFound.
        return Task.FromResult<IResult>(Results.StatusCode(StatusCodes.Status501NotImplemented));
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
            ErrorResponse error => Results.Json(error, statusCode: StatusCodes.Status501NotImplemented),
            _ => Results.Problem("Unexpected actor response while reserving a seat.")
        };
    }

    private static async Task<IResult> ReleaseSeatForSession(
        Guid id,
        ReleaseSeatRequest request,
        ActorRegistry registry,
        CancellationToken cancellationToken)
    {
        // TODO Task 6 - ask the actor for ReleaseSeat and map the possible responses:
        // SeatReleased -> 200 OK
        // SessionNotFound -> 404 Not Found
        // SeatNotReserved -> 409 Conflict
        // ErrorResponse -> 501 Not Implemented
        await AskSeatReservations(registry, new ReleaseSeat(id, request.AttendeeId), cancellationToken);
        return Results.StatusCode(StatusCodes.Status501NotImplemented);
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
