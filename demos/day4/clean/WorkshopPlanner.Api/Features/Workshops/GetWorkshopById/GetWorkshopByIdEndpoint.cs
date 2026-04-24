using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.GetWorkshopById;

namespace WorkshopPlanner.Api.Features.Workshops.GetWorkshopById;

public static class GetWorkshopByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new GetWorkshopByIdQuery(id), ct),
                TypedResults.Ok));
    }
}
