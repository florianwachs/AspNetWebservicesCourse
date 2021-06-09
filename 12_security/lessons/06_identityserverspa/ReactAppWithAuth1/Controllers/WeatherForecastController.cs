using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReactAppWithAuth1.Models;

namespace ReactAppWithAuth1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly List<string> Summaries = new()
        {
            "Freezing",
            "Bracing"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = AppPolicies.CanReadWeather)]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Count)]
            })
            .ToArray();
        }

        [HttpPost]
        [Authorize(Policy = AppPolicies.CanAddWeather)]
        public ActionResult AddSummary(string summary)
        {
            if (string.IsNullOrEmpty(summary))
            {
                return BadRequest();
            }

            Summaries.Add(summary);
            return Ok();
        }
    }
}
