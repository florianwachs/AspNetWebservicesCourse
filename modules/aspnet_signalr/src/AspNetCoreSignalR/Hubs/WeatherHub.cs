using AspNetCoreSignalR.Models;
using Microsoft.AspNetCore.SignalR;

namespace AspNetCoreSignalR.Hubs;

public class WeatherHub : Hub<IWeatherHub>
{
}

public interface IWeatherHub
{
    Task WeatherUpdated(WeatherForecast weather);
}