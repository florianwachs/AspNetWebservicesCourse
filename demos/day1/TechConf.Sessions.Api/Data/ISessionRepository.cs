using TechConf.Sessions.Api.Models;

namespace TechConf.Sessions.Api.Data;

public interface ISessionRepository
{
    IReadOnlyList<Session> List();

    Session? GetById(int id);

    int GetNextId();

    void Add(Session session);

    bool Update(Session session);

    bool Delete(int id);

    bool ExistsWithTitle(string title, int? excludingId = null);
}
