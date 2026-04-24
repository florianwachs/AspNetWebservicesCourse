using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.CheckInRegistration;

namespace WorkshopPlanner.Api.Features.Workshops.CheckInRegistration;

public static class CheckInRegistrationEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/registrations/{registrationId:int}/check-in", async (int id, int registrationId, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new CheckInRegistrationCommand(id, registrationId), ct),
                TypedResults.Ok));
    }
}
