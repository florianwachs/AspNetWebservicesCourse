using MediatR;
using WorkshopPlanner.Application.Features.Workshops.GetWorkshops;

namespace WorkshopPlanner.Api.Features.Workshops.GetWorkshops;

public static class GetWorkshopsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
            TypedResults.Ok(await sender.Send(new GetWorkshopsQuery(), ct)));
    }
}
