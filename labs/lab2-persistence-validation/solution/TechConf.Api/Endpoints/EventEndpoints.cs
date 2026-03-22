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
        var events = await repository.GetAll(city, page, pageSize, cancellationToken);
        return TypedResults.Ok(events.Select(MapEvent).ToList());
    }

    private static async Task<Ok<EventDetailDto>> GetEventById(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        var evt = await repository.GetById(id, cancellationToken);
        return TypedResults.Ok(MapEventDetail(evt));
    }

    private static async Task<Ok<List<SessionDto>>> GetEventSessions(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        var sessions = await repository.GetSessions(id, cancellationToken);
        return TypedResults.Ok(sessions.Select(MapSession).ToList());
    }

    private static async Task<Created<EventDto>> CreateEvent(
        CreateEventRequest request,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        var evt = await repository.Create(request, cancellationToken);
        return TypedResults.Created($"/api/events/{evt.Id}", MapEvent(evt));
    }

    private static async Task<NoContent> UpdateEvent(
        int id,
        UpdateEventRequest request,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        await repository.Update(id, request, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteEvent(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        await repository.Delete(id, cancellationToken);
        return TypedResults.NoContent();
    }

    private static EventDto MapEvent(Event evt) =>
        new(
            evt.Id,
            evt.Name,
            evt.Date,
            evt.City,
            evt.Description,
            evt.Sessions.Count);

    private static EventDetailDto MapEventDetail(Event evt) =>
        new(
            evt.Id,
            evt.Name,
            evt.Date,
            evt.City,
            evt.Description,
            evt.Sessions
                .OrderBy(s => s.Title)
                .Select(MapSession)
                .ToList());

    private static SessionDto MapSession(Session session) =>
        new(
            session.Id,
            session.Title,
            session.Duration,
            session.Speakers
                .OrderBy(sp => sp.Name)
                .Select(sp => sp.Name)
                .ToList());
}
