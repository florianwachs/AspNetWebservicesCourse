using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.CancelRegistration;

namespace WorkshopPlanner.Api.Features.Workshops.CancelRegistration;

public static class CancelRegistrationEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}/registrations/{registrationId:int}", async (int id, int registrationId, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new CancelRegistrationCommand(id, registrationId), ct),
                TypedResults.Ok));
    }
}
