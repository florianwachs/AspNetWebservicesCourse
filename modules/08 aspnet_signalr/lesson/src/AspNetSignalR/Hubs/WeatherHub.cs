using AspNetSignalR.Models;
using Microsoft.AspNetCore.SignalR;

namespace AspNetSignalR.Hubs;

public class WeatherHub : Hub<IWeatherHub>
{
}

public interface IWeatherHub
{
    Task WeatherUpdated(WeatherForecast weather);
}