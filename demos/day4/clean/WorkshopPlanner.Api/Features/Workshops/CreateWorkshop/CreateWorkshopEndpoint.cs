using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.CreateWorkshop;

namespace WorkshopPlanner.Api.Features.Workshops.CreateWorkshop;

public static class CreateWorkshopEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateWorkshopCommand command, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(command, ct),
                response => TypedResults.Created($"/api/workshops/{response.Id}", response)));
    }
}
