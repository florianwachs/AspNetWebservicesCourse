using TechConf.GraphQL.Data;
using TechConf.GraphQL.Models;

namespace TechConf.GraphQL.GraphQL;

public class Query
{
    // TODO: Task 2 — Implement GetEvents
    // Return all events from the database as IQueryable<Event>.
    // Add these attributes:
    //   [UseProjection]
    //   [UseFiltering]
    //   [UseSorting]
    // The method receives AppDbContext via [Service] parameter injection.
    public IQueryable<Event> GetEvents()
    {
        throw new NotImplementedException("Implement GetEvents with filtering, sorting, and projections");
    }

    // TODO: Task 3 — Implement GetEventById
    // Return a single event by its Guid id.
    // Add [UseFirstOrDefault] to return the first match or null.
    // Add [UseProjection] for field selection.
    // Filter: db.Events.Where(e => e.Id == id)
    public IQueryable<Event> GetEventById(Guid id)
    {
        throw new NotImplementedException("Implement GetEventById with UseFirstOrDefault");
    }

    // TODO: Task 6 — Implement GetSessions with pagination
    // Return all sessions from the database as IQueryable<Session>.
    // Add these attributes:
    //   [UsePaging(IncludeTotalCount = true)]
    //   [UseProjection]
    //   [UseFiltering]
    //   [UseSorting]
    public IQueryable<Session> GetSessions()
    {
        throw new NotImplementedException("Implement GetSessions with cursor-based pagination");
    }
}
