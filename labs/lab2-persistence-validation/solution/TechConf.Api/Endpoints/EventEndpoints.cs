using Microsoft.AspNetCore.Http.HttpResults;
using TechConf.Api.Models;
using TechConf.Api.Repositories;
using TechConf.Api.Validation;

namespace TechConf.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events").WithTags("Events");

        group.MapGet("/", GetAllEvents);
        group.MapGet("/{id:int}", GetEventById);
        group.MapGet("/{id:int}/sessions", GetEventSessions);
        group.MapPost("/", CreateEvent)
            .AddEndpointFilter<ValidationFilter<CreateEventRequest>>();
        group.MapPut("/{id:int}", UpdateEvent);
        group.MapDelete("/{id:int}", DeleteEvent);
    }

    private static async Task<Ok<List<EventDto>>> GetAllEvents(
        IEventRepository repository,
        string? city,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var events = await repository.GetAllAsync(city, page, pageSize, cancellationToken);
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

    private static async Task<Created<EventDto>> CreateEvent(
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
