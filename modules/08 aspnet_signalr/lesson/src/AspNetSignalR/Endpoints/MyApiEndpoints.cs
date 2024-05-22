using AspNetSignalR.Hubs;
using AspNetSignalR.Models;
using AspNetSignalR.Services;
using Microsoft.AspNetCore.SignalR;

namespace AspNetSignalR.Endpoints;

public static class MyApiEndpoints
{
    public static void MapMyApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/forecasts", (WeatherServices services) => 
                Results.Ok(services.GetForecasts()))
            .WithName("GetWeatherForecast")
            .WithOpenApi();

        app.MapPost("/api/v1/forecasts",
                (WeatherForecast data, IHubContext<WeatherHub, IWeatherHub> hub) =>
                {
                    // Add to Repository ...

                    // Notify Users
                    hub.Clients.All.WeatherUpdated(data);

                    return Results.Ok(data);
                })
            .WithName("PostWeatherForecast")
            .WithOpenApi();
    }
}