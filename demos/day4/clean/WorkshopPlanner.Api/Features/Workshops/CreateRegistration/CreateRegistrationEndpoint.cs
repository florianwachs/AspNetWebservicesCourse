using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.CreateRegistration;

namespace WorkshopPlanner.Api.Features.Workshops.CreateRegistration;

public static class CreateRegistrationEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/registrations", async (int id, CreateRegistrationBody body, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new CreateRegistrationCommand(id, body.AttendeeName, body.AttendeeEmail), ct),
                response => TypedResults.Created($"/api/workshops/{id}/registrations/{response.RegistrationId}", response)));
    }
}

public sealed record CreateRegistrationBody(string AttendeeName, string AttendeeEmail);
