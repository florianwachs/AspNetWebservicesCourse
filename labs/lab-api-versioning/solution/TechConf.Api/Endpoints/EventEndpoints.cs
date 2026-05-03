using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using TechConf.Api.Data;
using TechConf.Api.Models;
using EventResponseV1 = TechConf.Api.Models.V1.EventResponse;
using EventResponseV2 = TechConf.Api.Models.V2.EventResponse;
using VenueResponseV2 = TechConf.Api.Models.V2.VenueResponse;

namespace TechConf.Api.Endpoints;

public static class EventEndpoints
{
    public static void MapVersionedEventEndpoints(this IEndpointRouteBuilder app)
    {
        var events = app.NewVersionedApi("Events")
            .MapGroup("/api/v{version:apiVersion}/events")
            .HasDeprecatedApiVersion(1.0)
            .HasApiVersion(2.0)
            .ReportApiVersions()
            .WithTags("Events");

        events.MapGet("/", GetAllEventsV1)
            .WithName("GetEventsV1")
            .WithSummary("List all events")
            .WithDescription("Returns the flat V1 event representation.")
            .Produces<List<EventResponseV1>>(StatusCodes.Status200OK)
            .MapToApiVersion(1.0);

        events.MapGet("/{id:int}", GetEventByIdV1)
            .WithName("GetEventByIdV1")
            .WithSummary("Get an event by ID")
            .WithDescription("Returns a single event using the flat V1 contract.")
            .Produces<EventResponseV1>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(1.0);

        events.MapGet("/", GetAllEventsV2)
            .WithName("GetEventsV2")
            .WithSummary("List all events")
            .WithDescription("Returns the richer V2 event representation.")
            .Produces<List<EventResponseV2>>(StatusCodes.Status200OK)
            .MapToApiVersion(2.0);

        events.MapGet("/{id:int}", GetEventByIdV2)
            .WithName("GetEventByIdV2")
            .WithSummary("Get an event by ID")
            .WithDescription("Returns a single event using the richer V2 contract.")
            .Produces<EventResponseV2>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(2.0);
    }

    private static Ok<List<EventResponseV1>> GetAllEventsV1()
        => TypedResults.Ok(EventStore.Events.Select(ToV1).ToList());

    private static Results<Ok<EventResponseV1>, NotFound> GetEventByIdV1(int id)
    {
        var evt = EventStore.Events.FirstOrDefault(e => e.Id == id);

        return evt is not null
            ? TypedResults.Ok(ToV1(evt))
            : TypedResults.NotFound();
    }

    private static Ok<List<EventResponseV2>> GetAllEventsV2()
        => TypedResults.Ok(EventStore.Events.Select(ToV2).ToList());

    private static Results<Ok<EventResponseV2>, NotFound> GetEventByIdV2(int id)
    {
        var evt = EventStore.Events.FirstOrDefault(e => e.Id == id);

        return evt is not null
            ? TypedResults.Ok(ToV2(evt))
            : TypedResults.NotFound();
    }

    private static EventResponseV1 ToV1(Event evt)
        => new(evt.Id, evt.Title, evt.StartDate, $"{evt.Venue.Name}, {evt.Venue.City}");

    private static EventResponseV2 ToV2(Event evt)
        => new(
            evt.Id,
            evt.Title,
            evt.Description,
            evt.StartDate,
            evt.EndDate,
            new VenueResponseV2(evt.Venue.Name, evt.Venue.Address, evt.Venue.City, evt.Venue.Capacity),
            evt.RegisteredCount,
            evt.Status.ToString());
}
