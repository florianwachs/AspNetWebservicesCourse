using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TechConf.Grpc.Server.Data;
using TechConf.Grpc.Server.Models;

namespace TechConf.Grpc.Server.Services;

public class EventGrpcService : EventService.EventServiceBase
{
    private readonly AppDbContext _db;
    private readonly EventStreamBroadcaster _eventStreamBroadcaster;
    private readonly ILogger<EventGrpcService> _logger;

    public EventGrpcService(
        AppDbContext db,
        EventStreamBroadcaster eventStreamBroadcaster,
        ILogger<EventGrpcService> logger)
    {
        _db = db;
        _eventStreamBroadcaster = eventStreamBroadcaster;
        _logger = logger;
    }

    public override async Task<GetEventsResponse> GetEvents(
        GetEventsRequest request, ServerCallContext context)
    {
        var query = BuildEventQuery(request.Search);
        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;

        var totalCount = await query.CountAsync(context.CancellationToken);
        var events = await query
            .OrderBy(ev => ev.StartDate)
            .ThenBy(ev => ev.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(context.CancellationToken);

        var response = new GetEventsResponse
        {
            TotalCount = totalCount
        };

        response.Events.AddRange(events.Select(MapToMessage));

        return response;
    }

    public override async Task<EventMessage> GetEventById(
        GetEventByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var eventId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid event ID format."));
        }

        var ev = await _db.Events.FindAsync([eventId], context.CancellationToken);

        if (ev is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Event '{request.Id}' was not found."));
        }

        return MapToMessage(ev);
    }

    public override async Task<EventMessage> CreateEvent(
        CreateEventRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Title is required."));
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Location is required."));
        }

        var startDate = request.StartDate.ToDateTime();
        var endDate = request.EndDate.ToDateTime();

        if (endDate <= startDate)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "EndDate must be after StartDate."));
        }

        var ev = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            StartDate = startDate,
            EndDate = endDate,
            Location = request.Location.Trim(),
            MaxAttendees = request.MaxAttendees,
            Status = Models.EventStatus.Draft
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync(context.CancellationToken);

        var message = MapToMessage(ev);
        _eventStreamBroadcaster.Publish(message);

        _logger.LogInformation("Created event {EventId}: {EventTitle}", ev.Id, ev.Title);

        return message;
    }

    public override async Task<Empty> DeleteEvent(
        DeleteEventRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var eventId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid event ID format."));
        }

        var ev = await _db.Events.FindAsync([eventId], context.CancellationToken);

        if (ev is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Event '{request.Id}' was not found."));
        }

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync(context.CancellationToken);

        return new Empty();
    }

    public override async Task StreamEvents(
        StreamEventsRequest request,
        IServerStreamWriter<EventMessage> responseStream,
        ServerCallContext context)
    {
        var reader = _eventStreamBroadcaster.Subscribe(out var subscriptionId);
        var sentEventIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var existingEvents = await BuildEventQuery(request.Search)
                .OrderBy(ev => ev.StartDate)
                .ThenBy(ev => ev.Title)
                .ToListAsync(context.CancellationToken);

            foreach (var ev in existingEvents)
            {
                var message = MapToMessage(ev);
                sentEventIds.Add(message.Id);
                await responseStream.WriteAsync(message, context.CancellationToken);
            }

            await foreach (var message in reader.ReadAllAsync(context.CancellationToken))
            {
                if (!sentEventIds.Add(message.Id) || !MatchesSearch(message, request.Search))
                {
                    continue;
                }

                await responseStream.WriteAsync(message, context.CancellationToken);
            }
        }
        catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("Event stream cancelled for client {Peer}", context.Peer);
        }
        finally
        {
            _eventStreamBroadcaster.Unsubscribe(subscriptionId);
        }
    }

    // Helper: Map domain Event to Protobuf EventMessage
    private static EventMessage MapToMessage(Event ev) => new()
    {
        Id = ev.Id.ToString(),
        Title = ev.Title,
        Description = ev.Description ?? "",
        StartDate = Timestamp.FromDateTime(DateTime.SpecifyKind(ev.StartDate, DateTimeKind.Utc)),
        EndDate = Timestamp.FromDateTime(DateTime.SpecifyKind(ev.EndDate, DateTimeKind.Utc)),
        Location = ev.Location,
        MaxAttendees = ev.MaxAttendees,
        Status = ev.Status switch
        {
            Models.EventStatus.Draft => EventStatusEnum.EventStatusDraft,
            Models.EventStatus.Published => EventStatusEnum.EventStatusPublished,
            Models.EventStatus.Cancelled => EventStatusEnum.EventStatusCancelled,
            _ => EventStatusEnum.EventStatusUnspecified
        }
    };

    private IQueryable<Event> BuildEventQuery(string? search)
    {
        var query = _db.Events.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(ev => ev.Title.Contains(search));
        }

        return query;
    }

    private static bool MatchesSearch(EventMessage message, string? search) =>
        string.IsNullOrWhiteSpace(search)
        || message.Title.Contains(search, StringComparison.OrdinalIgnoreCase);
}
