using TechConf.GraphQL.Data;
using TechConf.GraphQL.Models;

namespace TechConf.GraphQL.GraphQL;

// Input type for creating an event
public record CreateEventInput(
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int MaxAttendees);

public class Mutation
{
    // TODO: Task 4 — Implement CreateEvent
    // Create a new Event from the input:
    //   - Generate a new Guid for the Id
    //   - Set Status to EventStatus.Draft
    //   - Add to db.Events and SaveChangesAsync
    //   - Return the created event
    // Parameters: CreateEventInput input, [Service] AppDbContext db, CancellationToken ct
    public async Task<Event> CreateEvent(CreateEventInput input)
    {
        throw new NotImplementedException("Implement CreateEvent mutation");
    }
}
