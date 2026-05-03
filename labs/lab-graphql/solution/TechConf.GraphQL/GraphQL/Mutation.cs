using TechConf.GraphQL.Data;
using TechConf.GraphQL.Models;

namespace TechConf.GraphQL.GraphQL;

public record CreateEventInput(
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int MaxAttendees);

public class Mutation
{
    public async Task<Event> CreateEvent(
        CreateEventInput input,
        [Service] AppDbContext db,
        CancellationToken ct)
    {
        var ev = new Event
        {
            Id = Guid.NewGuid(),
            Title = input.Title,
            Description = input.Description,
            StartDate = input.StartDate,
            EndDate = input.EndDate,
            Location = input.Location,
            MaxAttendees = input.MaxAttendees,
            Status = EventStatus.Draft
        };

        db.Events.Add(ev);
        await db.SaveChangesAsync(ct);
        return ev;
    }
}
