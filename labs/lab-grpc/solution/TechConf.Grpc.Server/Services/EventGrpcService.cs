using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TechConf.Grpc.Server.Data;
using TechConf.Grpc.Server.Models;

namespace TechConf.Grpc.Server.Services;

public class EventGrpcService : EventService.EventServiceBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<EventGrpcService> _logger;

    public EventGrpcService(AppDbContext db, ILogger<EventGrpcService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public override async Task<GetEventsResponse> GetEvents(
        GetEventsRequest request, ServerCallContext context)
    {
        var query = _db.Events.AsNoTracking();

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(e => e.Title.Contains(request.Search));

        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;

        var total = await query.CountAsync(context.CancellationToken);
        var events = await query
            .OrderBy(e => e.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(context.CancellationToken);

        var response = new GetEventsResponse { TotalCount = total };
        response.Events.AddRange(events.Select(MapToMessage));
        return response;
    }

    public override async Task<EventMessage> GetEventById(
        GetEventByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid event ID format"));

        var ev = await _db.Events.FindAsync(
            new object[] { id }, context.CancellationToken);

        if (ev is null)
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Event '{request.Id}' not found"));

        return MapToMessage(ev);
    }

    public override async Task<EventMessage> CreateEvent(
        CreateEventRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Title is required"));

        var ev = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate.ToDateTime(),
            EndDate = request.EndDate.ToDateTime(),
            Location = request.Location,
            MaxAttendees = request.MaxAttendees,
            Status = Models.EventStatus.Draft
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Created event {Id}: {Title}", ev.Id, ev.Title);
        return MapToMessage(ev);
    }

    public override async Task<Empty> DeleteEvent(
        DeleteEventRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid event ID format"));

        var ev = await _db.Events.FindAsync(
            new object[] { id }, context.CancellationToken)
            ?? throw new RpcException(new Status(StatusCode.NotFound,
                $"Event '{request.Id}' not found"));

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync(context.CancellationToken);
        return new Empty();
    }

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
}
