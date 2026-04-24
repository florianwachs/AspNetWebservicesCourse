using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.GetSessions;

namespace WorkshopPlanner.Api.Features.Workshops.GetSessions;

public static class GetSessionsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}/sessions", async (int id, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new GetSessionsQuery(id), ct),
                TypedResults.Ok));
    }
}
