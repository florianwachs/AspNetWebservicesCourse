using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TechConf.Grpc.Server.Data;

namespace TechConf.Grpc.Server.Services;

public class SessionGrpcService : SessionService.SessionServiceBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<SessionGrpcService> _logger;

    public SessionGrpcService(AppDbContext db, ILogger<SessionGrpcService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public override async Task<GetSessionsResponse> GetSessions(
        GetSessionsRequest request, ServerCallContext context)
    {
        var query = _db.Sessions.AsNoTracking();

        if (!string.IsNullOrEmpty(request.EventId))
        {
            if (!Guid.TryParse(request.EventId, out var eventId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid event ID"));
            query = query.Where(s => s.EventId == eventId);
        }

        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;

        var total = await query.CountAsync(context.CancellationToken);
        var sessions = await query
            .OrderBy(s => s.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(context.CancellationToken);

        var response = new GetSessionsResponse { TotalCount = total };
        response.Sessions.AddRange(sessions.Select(MapToMessage));
        return response;
    }

    public override async Task<SessionMessage> GetSessionById(
        GetSessionByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid session ID format"));

        var session = await _db.Sessions.FindAsync(
            new object[] { id }, context.CancellationToken);

        if (session is null)
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Session '{request.Id}' not found"));

        return MapToMessage(session);
    }

    public override async Task<SessionMessage> CreateSession(
        CreateSessionRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Title is required"));

        var session = new Models.Session
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            StartTime = request.StartTime.ToDateTime(),
            EndTime = request.EndTime.ToDateTime(),
            Room = request.Room,
            EventId = Guid.Parse(request.EventId),
            SpeakerId = Guid.Parse(request.SpeakerId)
        };

        _db.Sessions.Add(session);
        await _db.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Created session {Id}: {Title}", session.Id, session.Title);
        return MapToMessage(session);
    }

    public override async Task StreamSessionUpdates(
        StreamSessionsRequest request,
        IServerStreamWriter<SessionMessage> responseStream,
        ServerCallContext context)
    {
        var eventId = Guid.Parse(request.EventId);

        while (!context.CancellationToken.IsCancellationRequested)
        {
            var sessions = await _db.Sessions
                .Where(s => s.EventId == eventId)
                .OrderBy(s => s.StartTime)
                .ToListAsync(context.CancellationToken);

            foreach (var session in sessions)
            {
                await responseStream.WriteAsync(MapToMessage(session));
            }

            await Task.Delay(TimeSpan.FromSeconds(3), context.CancellationToken);
        }
    }

    private static SessionMessage MapToMessage(Models.Session s) => new()
    {
        Id = s.Id.ToString(),
        Title = s.Title,
        Description = s.Description ?? "",
        StartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(s.StartTime, DateTimeKind.Utc)),
        EndTime = Timestamp.FromDateTime(DateTime.SpecifyKind(s.EndTime, DateTimeKind.Utc)),
        Room = s.Room,
        EventId = s.EventId.ToString(),
        SpeakerId = s.SpeakerId.ToString()
    };
}
