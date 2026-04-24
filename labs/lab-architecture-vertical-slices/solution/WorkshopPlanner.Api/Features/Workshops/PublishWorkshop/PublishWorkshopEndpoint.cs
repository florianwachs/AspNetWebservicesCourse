using MediatR;
using WorkshopPlanner.Api.Common;

namespace WorkshopPlanner.Api.Features.Workshops.PublishWorkshop;

public static class PublishWorkshopEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/publish", async (int id, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new PublishWorkshopCommand(id), ct),
                TypedResults.Ok));
    }
}
