using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public interface ISessionManager
{
    /// <summary>Fired whenever a new session is created. Use to wire AI pipelines, logging, etc.</summary>
    event Action<SessionContext>? SessionCreated;

    SessionContext CreateSession(string title, string hostPin, string? sessionCode = null, string? description = null);
    SessionContext CreateSessionFromSeed(string seedJson, string hostPin, string? sessionCodeOverride = null);
    SessionContext? GetSession(string sessionCode);
    bool ValidateHostPin(string sessionCode, string pin);
    IReadOnlyList<SessionInfo> ListActiveSessions();
    Task LoadSlidesForSessionAsync(string sessionCode, string slidesPath);
    bool RemoveSession(string sessionCode);
}

public record SessionInfo(string SessionCode, string Title, string Description, SessionStatus Status, DateTimeOffset CreatedAt, int TopicCount);
