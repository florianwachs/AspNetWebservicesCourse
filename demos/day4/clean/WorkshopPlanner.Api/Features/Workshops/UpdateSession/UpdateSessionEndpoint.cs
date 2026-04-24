using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Application.Features.Workshops.UpdateSession;

namespace WorkshopPlanner.Api.Features.Workshops.UpdateSession;

public static class UpdateSessionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:int}/sessions/{sessionId:int}", async (int id, int sessionId, UpdateSessionBody body, ISender sender, CancellationToken ct) =>
            HttpResultMapper.FromResult(
                await sender.Send(new UpdateSessionCommand(id, sessionId, body.Title, body.SpeakerName, body.DurationMinutes), ct),
                TypedResults.Ok));
    }
}

public sealed record UpdateSessionBody(string Title, string SpeakerName, int DurationMinutes);
