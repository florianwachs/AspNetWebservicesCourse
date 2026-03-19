using Microsoft.AspNetCore.Http.HttpResults;
using TechConf.Api.Data;
using TechConf.Api.Models;

namespace TechConf.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events")
            .WithTags("Events");

        group.MapGet("/", GetAllEvents);
        group.MapGet("/{id:int}", GetEventById);
        group.MapGet("/{id:int}/summary", GetEventSummary);
        group.MapPost("/", CreateEvent);
        group.MapPut("/{id:int}", UpdateEvent);
        group.MapDelete("/{id:int}", DeleteEvent);
    }

    private static Ok<List<Event>> GetAllEvents(string? city, DateTime? from, string? name)
    {
        var query = EventStore.Events.AsEnumerable();

        if (city is not null)
            query = query.Where(e => e.City.Equals(city, StringComparison.OrdinalIgnoreCase));

        if (from is not null)
            query = query.Where(e => e.Date >= from);

        if (name is not null)
            query = query.Where(e => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        return TypedResults.Ok(query.ToList());
    }

    private static Results<Ok<Event>, NotFound> GetEventById(int id)
    {
        var evt = EventStore.Events.FirstOrDefault(e => e.Id == id);
        return evt is not null
            ? TypedResults.Ok(evt)
            : TypedResults.NotFound();
    }

    private static Results<Ok<EventSummary>, NotFound> GetEventSummary(int id)
    {
        var evt = EventStore.Events.FirstOrDefault(e => e.Id == id);
        return evt is not null
            ? TypedResults.Ok(new EventSummary(evt.Name, evt.Date, evt.City))
            : TypedResults.NotFound();
    }

    private static Created<Event> CreateEvent(CreateEventRequest request)
    {
        var evt = new Event(
            EventStore.GetNextId(),
            request.Name,
            request.Date,
            request.City,
            request.Description);

        EventStore.Events.Add(evt);

        return TypedResults.Created($"/api/events/{evt.Id}", evt);
    }

    private static Results<NoContent, NotFound> UpdateEvent(int id, UpdateEventRequest request)
    {
        var index = EventStore.Events.FindIndex(e => e.Id == id);
        if (index == -1) return TypedResults.NotFound();

        EventStore.Events[index] = EventStore.Events[index] with
        {
            Name = request.Name,
            Date = request.Date,
            City = request.City,
            Description = request.Description
        };

        return TypedResults.NoContent();
    }

    private static Results<NoContent, NotFound> DeleteEvent(int id)
    {
        var evt = EventStore.Events.FirstOrDefault(e => e.Id == id);
        if (evt is null) return TypedResults.NotFound();

        EventStore.Events.Remove(evt);
        return TypedResults.NoContent();
    }
}
