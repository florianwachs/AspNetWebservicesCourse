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

    // TODO: Task 4 - Refactor all methods below to use IEventRepository and map entities to DTOs here

    private static async Task<IResult> GetAllEvents(
        IEventRepository repository,
        string? city,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        // TODO: Call repository.GetAll(city, page, pageSize, cancellationToken),
        // map each Event to EventDto, and return the results with TypedResults.Ok(...)
        throw new NotImplementedException();
    }

    private static async Task<IResult> GetEventById(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        // TODO: Call repository.GetById(id, cancellationToken),
        // map the returned Event to EventDetailDto, and return it with TypedResults.Ok(...)
        throw new NotImplementedException();
    }

    private static async Task<IResult> GetEventSessions(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        // TODO: Call repository.GetSessions(id, cancellationToken),
        // map each Session to SessionDto, and return the result with TypedResults.Ok(...)
        throw new NotImplementedException();
    }

    private static async Task<IResult> CreateEvent(
        CreateEventRequest request,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        // TODO: Call repository.Create(request, cancellationToken),
        // map the created Event to EventDto, and return TypedResults.Created(...)
        throw new NotImplementedException();
    }

    private static async Task<IResult> UpdateEvent(
        int id,
        UpdateEventRequest request,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        // TODO: Call repository.Update(id, request, cancellationToken)
        // and return TypedResults.NoContent()
        throw new NotImplementedException();
    }

    private static async Task<IResult> DeleteEvent(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        // TODO: Call repository.Delete(id, cancellationToken)
        // and return TypedResults.NoContent()
        throw new NotImplementedException();
    }
}
