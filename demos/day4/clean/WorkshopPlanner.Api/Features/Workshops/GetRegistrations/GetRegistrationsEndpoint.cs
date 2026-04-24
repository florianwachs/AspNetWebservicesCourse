using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.GetRegistrations;

namespace WorkshopPlanner.Api.Features.Workshops.GetRegistrations;

public static class GetRegistrationsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}/registrations", async (int id, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new GetRegistrationsQuery(id), ct),
                TypedResults.Ok));
    }
}
