using TechConf.Sessions.Api.Models;

namespace TechConf.Sessions.Api.Data;

public sealed class InMemorySessionRepository : ISessionRepository
{
    private readonly object _syncRoot = new();
    private readonly List<Session> _sessions =
    [
        new(1, "Minimal APIs in Practice", "Alice Hoffmann", "Web", new DateTime(2026, 4, 10, 9, 0, 0, DateTimeKind.Utc), 60, true),
        new(2, "Dependency Injection Essentials", "Bruno Keller", "Architecture", new DateTime(2026, 4, 10, 10, 30, 0, DateTimeKind.Utc), 45, false),
        new(3, "HTTP Status Codes that Matter", "Carla Meier", "HTTP", new DateTime(2026, 4, 10, 13, 0, 0, DateTimeKind.Utc), 45, true)
    ];
    private int _nextId = 4;

    public IReadOnlyList<Session> List()
    {
        lock (_syncRoot)
        {
            return _sessions
                .OrderBy(session => session.StartsAt)
                .ThenBy(session => session.Title)
                .ToList();
        }
    }

    public Session? GetById(int id)
    {
        lock (_syncRoot)
        {
            return _sessions.FirstOrDefault(session => session.Id == id);
        }
    }

    public int GetNextId()
    {
        lock (_syncRoot)
        {
            return _nextId++;
        }
    }

    public void Add(Session session)
    {
        lock (_syncRoot)
        {
            _sessions.Add(session);
        }
    }

    public bool Update(Session session)
    {
        lock (_syncRoot)
        {
            var index = _sessions.FindIndex(existing => existing.Id == session.Id);

            if (index == -1)
            {
                return false;
            }

            _sessions[index] = session;
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_syncRoot)
        {
            var session = _sessions.FirstOrDefault(existing => existing.Id == id);

            if (session is null)
            {
                return false;
            }

            _sessions.Remove(session);
            return true;
        }
    }

    public bool ExistsWithTitle(string title, int? excludingId = null)
    {
        lock (_syncRoot)
        {
            return _sessions.Any(session =>
                session.Id != excludingId &&
                session.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }
    }
}
