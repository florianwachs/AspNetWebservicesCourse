using MediatR;
using WorkshopPlanner.Api.Common;

namespace WorkshopPlanner.Api.Features.Workshops.AddSession;

public static class AddSessionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/sessions", async (int id, AddSessionBody body, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new AddSessionCommand(id, body.Title, body.SpeakerName, body.DurationMinutes), ct),
                response => TypedResults.Created($"/api/workshops/{id}", response)));
    }
}

public sealed record AddSessionBody(string Title, string SpeakerName, int DurationMinutes);
