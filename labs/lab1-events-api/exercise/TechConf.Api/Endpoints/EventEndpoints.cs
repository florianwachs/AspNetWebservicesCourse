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
        group.MapPost("/", CreateEvent);
        group.MapPut("/{id:int}", UpdateEvent);
        group.MapDelete("/{id:int}", DeleteEvent);
    }

    // TODO: Task 1 - Return all events from EventStore.Events
    private static IResult GetAllEvents()
    {
        throw new NotImplementedException("Implement this method to return all events");
    }

    // TODO: Task 2 - Find event by ID, return 404 if not found
    private static IResult GetEventById(int id)
    {
        throw new NotImplementedException("Implement this method to return an event by ID");
    }

    // TODO: Task 3 - Create a new event, return 201 Created with location header
    private static IResult CreateEvent(CreateEventRequest request)
    {
        throw new NotImplementedException("Implement this method to create a new event");
    }

    // TODO: Task 4 - Update an existing event, return 404 if not found, 204 on success
    private static IResult UpdateEvent(int id, UpdateEventRequest request)
    {
        throw new NotImplementedException("Implement this method to update an event");
    }

    // TODO: Task 5 - Delete an event, return 404 if not found, 204 on success
    private static IResult DeleteEvent(int id)
    {
        throw new NotImplementedException("Implement this method to delete an event");
    }
}
