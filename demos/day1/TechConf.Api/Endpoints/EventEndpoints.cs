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

        group.MapGet("/", GetAllEvents)
            .WithName("GetAllEvents")
            .WithSummary("List all events");

        group.MapGet("/{id:int}", GetEventById)
            .WithName("GetEventById")
            .WithSummary("Get one event by id");

        group.MapPost("/", CreateEvent)
            .WithName("CreateEvent")
            .WithSummary("Create a new event");

        group.MapPut("/{id:int}", UpdateEvent)
            .WithName("UpdateEvent")
            .WithSummary("Replace an existing event");

        group.MapDelete("/{id:int}", DeleteEvent)
            .WithName("DeleteEvent")
            .WithSummary("Delete an event");
    }

    // Query parameters like ?city=Munich or ?search=cloud are bound automatically.
    private static Ok<List<Event>> GetAllEvents(string? city, string? search)
    {
        var query = EventStore.Events.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(e => e.City.Equals(city, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e =>
                e.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (e.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        return TypedResults.Ok(query.OrderBy(e => e.Date).ToList());
    }

    // TypedResults + union return types make success and error responses explicit.
    private static Results<Ok<Event>, NotFound> GetEventById(int id)
    {
        var evt = EventStore.Events.FirstOrDefault(e => e.Id == id);

        return evt is not null
            ? TypedResults.Ok(evt)
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

        if (index == -1)
        {
            return TypedResults.NotFound();
        }

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

        if (evt is null)
        {
            return TypedResults.NotFound();
        }

        EventStore.Events.Remove(evt);
        return TypedResults.NoContent();
    }
}
