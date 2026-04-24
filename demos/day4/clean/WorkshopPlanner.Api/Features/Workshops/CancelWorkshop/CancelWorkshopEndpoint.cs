using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.CancelWorkshop;

namespace WorkshopPlanner.Api.Features.Workshops.CancelWorkshop;

public static class CancelWorkshopEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/cancel", async (int id, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new CancelWorkshopCommand(id), ct),
                TypedResults.Ok));
    }
}
