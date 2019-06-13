using AspNetCoreSignalR.ApiWithSpa.Hubs;
using AspNetCoreSignalR.ApiWithSpa.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreSignalR.ApiWithSpa.Services
{
    public class WeatherServices
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static Random _rng = new Random();
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(5);
        private readonly IHubContext<WeatherHub, IWeatherHub> _hub;
        private readonly Timer _timer;

        public WeatherServices(IHubContext<WeatherHub, IWeatherHub> hub)
        {
            _hub = hub;
            _timer = new Timer(UpdateWeather, null, _updateInterval, _updateInterval);
        }

        private void UpdateWeather(object state)
        {
            _hub.Clients.All.WeatherUpdated(CreateForcast(1));
        }

        public IEnumerable<WeatherForecast> GetForecasts()
        {
            return Enumerable.Range(1, 5).Select(CreateForcast);
        }

        private static WeatherForecast CreateForcast(int day)
        {
            return new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(day).ToString("d"),
                TemperatureC = _rng.Next(-20, 55),
                Summary = Summaries[_rng.Next(Summaries.Length)]
            };
        }
    }
}
