using Akka.Actor;
using Akka.Hosting;
using akkasample.App.Actors;
using akkasample.Domain;
using Microsoft.AspNetCore.Mvc;

namespace akkasample.App.Endpoints;

public static class CounterEndpoints
{
    public static void MapCounter(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/counter");

        group.MapGet("{counterId}", async (string counterId, IRequiredActor<CounterActor> counterActor) =>
        {
            var counter = await counterActor.ActorRef.Ask<Counter>(new FetchCounter(counterId), TimeSpan.FromSeconds(5));
            return counter;
        });
        
        group.MapPost("{counterId}", async (string counterId,[FromBody] int increment, IRequiredActor<CounterActor> counterActor) =>
        {
            var result = await counterActor.ActorRef.Ask<CounterCommandResponse>(new IncrementCounterCommand(counterId, increment), TimeSpan.FromSeconds(5));
            if (!result.IsSuccess)
            {
                return Results.BadRequest();
            }

            return Results.Ok(result.Event);
        });
        
        group.MapPut("{counterId}", async (string counterId,[FromBody] int counterValue, IRequiredActor<CounterActor> counterActor) =>
        {
            var result = await counterActor.ActorRef.Ask<CounterCommandResponse>(new SetCounterCommand(counterId, counterValue), TimeSpan.FromSeconds(5));
            if (!result.IsSuccess)
            {
                return Results.BadRequest();
            }
        
            return Results.Ok(result.Event);
        });
    }
}