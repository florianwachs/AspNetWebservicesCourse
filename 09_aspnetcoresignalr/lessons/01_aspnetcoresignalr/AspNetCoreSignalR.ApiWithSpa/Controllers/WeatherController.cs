using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreSignalR.ApiWithSpa.Hubs;
using AspNetCoreSignalR.ApiWithSpa.Models;
using AspNetCoreSignalR.ApiWithSpa.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AspNetCoreSignalR.ApiWithSpa.Controllers
{
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        private readonly WeatherServices _services;
        private readonly IHubContext<WeatherHub, IWeatherHub> _hub;

        public WeatherController(WeatherServices services, IHubContext<WeatherHub, IWeatherHub> hub)
        {
            _services = services;
            _hub = hub;
        }

        [HttpPost("forcasts")]
        public WeatherForecast AddForcast(WeatherForecast data)
        {
            // Add to Repository ...

            // Notify Users
            _hub.Clients.All.WeatherUpdated(data);

            return data;
        }

        [HttpGet("forcasts")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            return _services.GetForecasts();
        }

    }
}
