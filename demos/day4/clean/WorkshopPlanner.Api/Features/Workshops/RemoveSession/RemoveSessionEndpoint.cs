using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.RemoveSession;

namespace WorkshopPlanner.Api.Features.Workshops.RemoveSession;

public static class RemoveSessionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}/sessions/{sessionId:int}", async (int id, int sessionId, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new RemoveSessionCommand(id, sessionId), ct),
                TypedResults.Ok));
    }
}
