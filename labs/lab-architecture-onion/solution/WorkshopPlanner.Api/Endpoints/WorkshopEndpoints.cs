using WorkshopPlanner.Api.Contracts;
using WorkshopPlanner.Application.Workshops.AddSession;
using WorkshopPlanner.Application.Workshops.CreateWorkshop;
using WorkshopPlanner.Application.Workshops.GetWorkshopById;
using WorkshopPlanner.Application.Workshops.GetWorkshops;
using WorkshopPlanner.Application.Workshops.PublishWorkshop;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Api.Endpoints;

public static class WorkshopEndpoints
{
    public static void MapWorkshopEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/workshops").WithTags("Workshops");

        group.MapGet("/", async (GetWorkshopsHandler handler, CancellationToken ct) =>
            TypedResults.Ok(await handler.Handle(ct)));

        group.MapGet("/{id:int}", async (int id, GetWorkshopByIdHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetWorkshopByIdQuery(id), ct);
            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : ToFailureResult(result.Error!);
        });

        group.MapPost("/", async (CreateWorkshopCommand command, CreateWorkshopHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(command, ct);
            return result.IsSuccess
                ? TypedResults.Created($"/api/workshops/{result.Value!.Id}", result.Value)
                : ToFailureResult(result.Error!);
        });

        group.MapPost("/{id:int}/sessions", async (int id, AddSessionRequest request, AddSessionHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new AddSessionCommand(id, request.Title, request.SpeakerName, request.DurationMinutes), ct);
            return result.IsSuccess
                ? TypedResults.Created($"/api/workshops/{id}", result.Value)
                : ToFailureResult(result.Error!);
        });

        group.MapPost("/{id:int}/publish", async (int id, PublishWorkshopHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new PublishWorkshopCommand(id), ct);
            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : ToFailureResult(result.Error!);
        });
    }

    private static IResult ToFailureResult(Error error) =>
        error.Type switch
        {
            ErrorType.Validation => Results.BadRequest(new ErrorResponse(error.Message)),
            ErrorType.Conflict => Results.Conflict(new ErrorResponse(error.Message)),
            ErrorType.NotFound => Results.NotFound(new ErrorResponse(error.Message)),
            _ => Results.BadRequest(new ErrorResponse(error.Message))
        };
}
