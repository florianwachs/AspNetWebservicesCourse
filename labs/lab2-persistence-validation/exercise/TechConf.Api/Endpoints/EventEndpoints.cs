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

    // TODO: Task 4 - Refactor all methods below to use IEventRepository instead of direct data access

    private static async Task<IResult> GetAllEvents(
        IEventRepository repository,
        string? city,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        // TODO: Call repository.GetAllAsync(city, page, pageSize, cancellationToken)
        // and return the results with TypedResults.Ok(...)
        throw new NotImplementedException();
    }

    private static async Task<IResult> GetEventById(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        // TODO: Call repository.GetByIdAsync(id, cancellationToken)
        // and return the result with TypedResults.Ok(...)
        throw new NotImplementedException();
    }

    private static async Task<IResult> GetEventSessions(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        // TODO: Call repository.GetSessionsAsync(id, cancellationToken)
        // and return the result with TypedResults.Ok(...)
        throw new NotImplementedException();
    }

    private static async Task<IResult> CreateEvent(
        CreateEventRequest request,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        // TODO: Call repository.CreateAsync(request, cancellationToken)
        // and return TypedResults.Created(...) with the created event DTO
        throw new NotImplementedException();
    }

    private static async Task<IResult> UpdateEvent(
        int id,
        UpdateEventRequest request,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        // TODO: Call repository.UpdateAsync(id, request, cancellationToken)
        // and return TypedResults.NoContent()
        throw new NotImplementedException();
    }

    private static async Task<IResult> DeleteEvent(
        int id,
        IEventRepository repository,
        CancellationToken cancellationToken)
    {
        // TODO: Call repository.DeleteAsync(id, cancellationToken)
        // and return TypedResults.NoContent()
        throw new NotImplementedException();
    }
}
