using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReactAppWithAuth1.Data;
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IAuthorizationService authorizationService)
        {
            _logger = logger;
            AuthorizationService = authorizationService;
        }

        public IAuthorizationService AuthorizationService { get; }

        [HttpGet]
        [Authorize(Policy = AppPolicies.CanReadWeather)]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var canReadTemp = await AuthorizationService.AuthorizeAsync(User, AppPolicies.CanReadTemp);
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index =>
            {
                var tempC = rng.Next(-20, 55);
                var tempF = 32 + (int)(tempC / 0.5556);

                return new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = canReadTemp.Succeeded? tempC.ToString() : "[YOU HAVE TO PAY]",
                    TemperatureF = canReadTemp.Succeeded ? tempF.ToString() : "[YOU HAVE TO PAY]",
                    Summary = Summaries[rng.Next(Summaries.Count)]
                };
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
