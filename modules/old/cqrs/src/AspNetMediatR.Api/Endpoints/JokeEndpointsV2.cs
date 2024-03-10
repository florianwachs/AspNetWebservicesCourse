using AspNetMediatR.Api.Queries;
using MediatR;

namespace AspNetMediatR.Api.Endpoints;

public static class JokeEndpointsV2
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/v2/jokes", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new JokesQuery());
            return Results.Ok(result);
        });
    }
}
