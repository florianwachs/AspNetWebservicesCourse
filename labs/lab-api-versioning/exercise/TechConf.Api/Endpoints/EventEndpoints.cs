using Microsoft.AspNetCore.Http.HttpResults;
using TechConf.Api.Data;
using TechConf.Api.Models;
using EventResponseV1 = TechConf.Api.Models.V1.EventResponse;
using EventResponseV2 = TechConf.Api.Models.V2.EventResponse;
using VenueResponseV2 = TechConf.Api.Models.V2.VenueResponse;

namespace TechConf.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events")
            .WithTags("Events");

        group.MapGet("/", GetAllEvents)
            .WithName("GetEvents")
            .WithSummary("List all events");

        group.MapGet("/{id:int}", GetEventById)
            .WithName("GetEventById")
            .WithSummary("Get an event by ID");
    }

    // TODO: Task 2 - Replace the unversioned group with a versioned API:
    // var events = app.NewVersionedApi("Events")
    //     .MapGroup("/api/v{version:apiVersion}/events")
    //     .HasDeprecatedApiVersion(1.0)
    //     .HasApiVersion(2.0)
    //     .ReportApiVersions()
    //     .WithTags("Events");
    //
    // Then map the V1 handlers with .MapToApiVersion(1.0)
    // and the V2 handlers with .MapToApiVersion(2.0).
    public static void MapVersionedEventEndpoints(this IEndpointRouteBuilder app)
    {
        throw new NotImplementedException("Convert the current unversioned endpoints to versioned route groups.");
    }

    private static Ok<List<EventResponseV1>> GetAllEvents()
        => TypedResults.Ok(EventStore.Events.Select(ToV1).ToList());

    private static Results<Ok<EventResponseV1>, NotFound> GetEventById(int id)
    {
        var evt = EventStore.Events.FirstOrDefault(e => e.Id == id);

        return evt is not null
            ? TypedResults.Ok(ToV1(evt))
            : TypedResults.NotFound();
    }

    // TODO: Task 3 - Map these handlers to api/v2/events and api/v2/events/{id}.
    private static Ok<List<EventResponseV2>> GetAllEventsV2()
    {
        throw new NotImplementedException("Return the richer V2 contract.");
    }

    private static Results<Ok<EventResponseV2>, NotFound> GetEventByIdV2(int id)
    {
        throw new NotImplementedException("Return the richer V2 contract by ID.");
    }

    private static EventResponseV1 ToV1(Event evt)
        => new(evt.Id, evt.Title, evt.StartDate, $"{evt.Venue.Name}, {evt.Venue.City}");

    // TODO: Task 3 - Project the internal model to the V2 DTO.
    private static EventResponseV2 ToV2(Event evt)
    {
        throw new NotImplementedException(
            "Map description, end date, nested venue, registered count, and status to the V2 response.");
    }
}
