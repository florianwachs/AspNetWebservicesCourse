using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.UpdateWorkshop;

namespace WorkshopPlanner.Api.Features.Workshops.UpdateWorkshop;

public static class UpdateWorkshopEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:int}", async (int id, UpdateWorkshopBody body, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new UpdateWorkshopCommand(id, body.Title, body.City, body.MaxAttendees), ct),
                TypedResults.Ok));
    }
}

public sealed record UpdateWorkshopBody(string Title, string City, int MaxAttendees);
