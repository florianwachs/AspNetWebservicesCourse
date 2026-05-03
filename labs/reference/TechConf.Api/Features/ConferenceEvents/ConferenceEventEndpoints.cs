using Microsoft.EntityFrameworkCore;
using TechConf.Api.Data;

namespace TechConf.Api.Features.ConferenceEvents;

public static class ConferenceEventEndpoints
{
    public static void MapConferenceEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events").WithTags("Events");

        group.MapGet("/", async (AppDbContext db, CancellationToken cancellationToken) =>
        {
            var events = await db.ConferenceEvents
                .AsNoTracking()
                .OrderBy(x => x.StartDate)
                .Select(x => new ConferenceEventListItem(
                    x.Id,
                    x.Slug,
                    x.Name,
                    x.City,
                    x.Location,
                    x.StartDate,
                    x.EndDate,
                    x.ProposalDeadlineUtc))
                .ToListAsync(cancellationToken);

            return TypedResults.Ok(events);
        })
        .WithSummary("List public conference events")
        .Produces<IReadOnlyList<ConferenceEventListItem>>(StatusCodes.Status200OK);
    }
}

public sealed record ConferenceEventListItem(
    int Id,
    string Slug,
    string Name,
    string City,
    string Location,
    DateOnly StartDate,
    DateOnly EndDate,
    DateTimeOffset ProposalDeadlineUtc);
