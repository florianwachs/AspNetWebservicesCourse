using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCoreTesting.Api.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreTesting.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching", "Sunny", "Rainy"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private WeatherMoodConverter MoodConverter { get; }

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherMoodConverter moodConverter)
        {
            _logger = logger;
            MoodConverter = moodConverter;
        }
        

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => {
                var result = new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-60, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                };

                result.Mood = MoodConverter.WeatherToMood(result);
                return result;
            })
            .ToArray();
        }
    }
}
