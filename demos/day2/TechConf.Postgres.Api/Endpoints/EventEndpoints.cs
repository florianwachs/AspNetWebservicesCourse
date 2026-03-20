using Microsoft.AspNetCore.Http.HttpResults;
using TechConf.Api.Models;
using TechConf.Api.Repositories;
using TechConf.Api.Validation;

namespace TechConf.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events")
            .WithTags("Events");

        group.MapGet("/", GetAllEvents)
            .Produces<List<EventListItemDto>>(StatusCodes.Status200OK)
            .WithSummary("List all events");

        group.MapGet("/{id:int}", GetEventById)
            .Produces<EventDetailDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get one event including sessions and speakers");

        group.MapGet("/{id:int}/sessions", GetEventSessions)
            .Produces<List<SessionDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get sessions for one event");

        group.MapPost("/", CreateEvent)
            .AddEndpointFilter<ValidationFilter<CreateEventRequest>>()
            .Produces<EventListItemDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .WithSummary("Create a new event");

        group.MapPut("/{id:int}", UpdateEvent)
            .AddEndpointFilter<ValidationFilter<UpdateEventRequest>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithSummary("Update an existing event");

        group.MapDelete("/{id:int}", DeleteEvent)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete an event");
    }

    private static async Task<Ok<List<EventListItemDto>>> GetAllEvents(
        string? city,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        var events = await repository.GetAllAsync(city, cancellationToken);
        return TypedResults.Ok(events);
    }

    private static async Task<Ok<EventDetailDto>> GetEventById(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        var evt = await repository.GetByIdAsync(id, cancellationToken);
        return TypedResults.Ok(evt);
    }

    private static async Task<Ok<List<SessionDto>>> GetEventSessions(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        var sessions = await repository.GetSessionsAsync(id, cancellationToken);
        return TypedResults.Ok(sessions);
    }

    private static async Task<Created<EventListItemDto>> CreateEvent(
        CreateEventRequest request,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        var evt = await repository.CreateAsync(request, cancellationToken);
        return TypedResults.Created($"/api/events/{evt.Id}", evt);
    }

    private static async Task<NoContent> UpdateEvent(
        int id,
        UpdateEventRequest request,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        await repository.UpdateAsync(id, request, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteEvent(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        await repository.DeleteAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }
}
