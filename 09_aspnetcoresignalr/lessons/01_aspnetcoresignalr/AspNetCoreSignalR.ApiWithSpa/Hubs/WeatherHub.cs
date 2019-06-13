using AspNetCoreSignalR.ApiWithSpa.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreSignalR.ApiWithSpa.Hubs
{
    public class WeatherHub : Hub<IWeatherHub>
    {

    }

    public interface IWeatherHub
    {
        Task WeatherUpdated(WeatherForecast weather);
    }
}
