using MediatR;
using WorkshopPlanner.Api.Common;

namespace WorkshopPlanner.Api.Features.Workshops.GetWorkshops;

public static class GetWorkshopsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new GetWorkshopsQuery(), ct),
                TypedResults.Ok));
    }
}
