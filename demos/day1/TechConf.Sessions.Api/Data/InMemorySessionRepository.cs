using TechConf.Sessions.Api.Models;

namespace TechConf.Sessions.Api.Data;

public sealed class InMemorySessionRepository : ISessionRepository
{
    //This implementation is not thread-safe and is only intended for demonstration purposes. In a production application, you would typically use a database or another persistent storage mechanism.    
    private readonly List<Session> _sessions =
    [
        new(1, "Minimal APIs in Practice", "Alice Hoffmann", "Web", new DateTime(2026, 4, 10, 9, 0, 0, DateTimeKind.Utc), 60, true),
        new(2, "Dependency Injection Essentials", "Bruno Keller", "Architecture", new DateTime(2026, 4, 10, 10, 30, 0, DateTimeKind.Utc), 45, false),
        new(3, "HTTP Status Codes that Matter", "Carla Meier", "HTTP", new DateTime(2026, 4, 10, 13, 0, 0, DateTimeKind.Utc), 45, true)
    ];
    private int _nextId = 4;

    public IReadOnlyList<Session> List()
    {
        return _sessions
            .OrderBy(session => session.StartsAt)
            .ThenBy(session => session.Title)
            .ToList();
    }

    public Session? GetById(int id)
    {
        return _sessions.FirstOrDefault(session => session.Id == id);
    }

    public int GetNextId()
    {
        return _nextId++;
    }

    public void Add(Session session)
    {
        _sessions.Add(session);
    }

    public bool Update(Session session)
    {
        var index = _sessions.FindIndex(existing => existing.Id == session.Id);

        if (index == -1)
        {
            return false;
        }

        _sessions[index] = session;
        return true;
    }

    public bool Delete(int id)
    {
        var session = _sessions.FirstOrDefault(existing => existing.Id == id);

        if (session is null)
        {
            return false;
        }

        _sessions.Remove(session);
        return true;
    }

    public bool ExistsWithTitle(string title, int? excludingId = null)
    {
        return _sessions.Any(session =>
            session.Id != excludingId &&
            session.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }
}
