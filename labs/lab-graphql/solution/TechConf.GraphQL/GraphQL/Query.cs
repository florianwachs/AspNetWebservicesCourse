using TechConf.GraphQL.Data;
using TechConf.GraphQL.Models;

namespace TechConf.GraphQL.GraphQL;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Event> GetEvents([Service] AppDbContext db)
        => db.Events;

    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<Event> GetEventById([Service] AppDbContext db, Guid id)
        => db.Events.Where(e => e.Id == id);

    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Session> GetSessions([Service] AppDbContext db)
        => db.Sessions;
}
