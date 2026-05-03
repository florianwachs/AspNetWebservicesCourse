using System.Collections.Concurrent;
using System.Text.Json;
using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public class SessionManager : ISessionManager
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly ConcurrentDictionary<string, SessionContext> _sessions = new(StringComparer.OrdinalIgnoreCase);

    public event Action<SessionContext>? SessionCreated;

    public SessionContext CreateSession(string title, string hostPin, string? sessionCode = null, string? description = null)
    {
        var code = string.IsNullOrWhiteSpace(sessionCode)
            ? GenerateSessionCode()
            : NormalizeCode(sessionCode);

        if (_sessions.ContainsKey(code))
            throw new InvalidOperationException($"Session with code '{code}' already exists.");

        var session = new ConferenceSession
        {
            Id = Guid.NewGuid().ToString(),
            SessionCode = code,
            Title = title,
            Description = description ?? "",
            HostPin = hostPin,
            Status = SessionStatus.Setup,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var context = new SessionContext(session);
        if (!_sessions.TryAdd(code, context))
            throw new InvalidOperationException($"Failed to create session '{code}'.");

        SessionCreated?.Invoke(context);
        return context;
    }

    public SessionContext CreateSessionFromSeed(string seedJson, string hostPin, string? sessionCodeOverride = null)
    {
        var seedData = JsonSerializer.Deserialize<SeedSessionData>(seedJson, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize seed data.");

        var code = sessionCodeOverride is not null ? NormalizeCode(sessionCodeOverride) : NormalizeCode(seedData.SessionId);

        if (_sessions.ContainsKey(code))
            throw new InvalidOperationException($"Session with code '{code}' already exists.");

        var session = new ConferenceSession
        {
            Id = Guid.NewGuid().ToString(),
            SessionCode = code,
            Title = seedData.Title,
            Description = seedData.Description,
            HostPin = hostPin,
            Status = SessionStatus.Setup,
            CreatedAt = DateTimeOffset.UtcNow,
            Topics = seedData.Topics.Select(t => new SessionTopic
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Order = t.Order,
                Status = TopicStatus.Upcoming,
                TalkingPoints = t.TalkingPoints ?? [],
                SuggestedPolls = t.SuggestedPolls?.Select(sp => new SuggestedPoll
                {
                    Question = sp.Question,
                    Options = sp.Options ?? []
                }).ToList() ?? []
            }).ToList()
        };

        var context = new SessionContext(session);
        if (!_sessions.TryAdd(code, context))
            throw new InvalidOperationException($"Failed to create session '{code}'.");

        SessionCreated?.Invoke(context);
        return context;
    }

    public SessionContext? GetSession(string sessionCode)
    {
        _sessions.TryGetValue(NormalizeCode(sessionCode), out var context);
        return context;
    }

    public bool ValidateHostPin(string sessionCode, string pin)
    {
        if (!_sessions.TryGetValue(NormalizeCode(sessionCode), out var context))
            return false;
        return string.Equals(context.Session.HostPin, pin, StringComparison.Ordinal);
    }

    public IReadOnlyList<SessionInfo> ListActiveSessions()
    {
        return _sessions.Values
            .Where(c => c.Session.Status != SessionStatus.Completed)
            .Select(c => new SessionInfo(
                c.Session.SessionCode,
                c.Session.Title,
                c.Session.Description,
                c.Session.Status,
                c.Session.CreatedAt,
                c.Session.Topics.Count))
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }

    public async Task LoadSlidesForSessionAsync(string sessionCode, string slidesPath)
    {
        var context = GetSession(sessionCode)
            ?? throw new ArgumentException($"Session '{sessionCode}' not found.");
        var slides = await SlideMarkdownParser.ParseFileAsync(slidesPath);
        context.LoadSlides(slides);
    }

    public bool RemoveSession(string sessionCode)
        => _sessions.TryRemove(NormalizeCode(sessionCode), out _);

    private static string GenerateSessionCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = Random.Shared;
        return new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    // Internal DTOs for seed JSON deserialization
    internal sealed class SeedSessionData
    {
        public string SessionId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public List<SeedTopic> Topics { get; set; } = [];
    }

    internal sealed class SeedTopic
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int Order { get; set; }
        public List<string>? TalkingPoints { get; set; }
        public List<SeedSuggestedPoll>? SuggestedPolls { get; set; }
    }

    internal sealed class SeedSuggestedPoll
    {
        public string Question { get; set; } = "";
        public List<string>? Options { get; set; }
    }
}
