using Microsoft.AspNetCore.Mvc;
using System;

namespace AspNetOpenApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Ermittelt die tatsächlichen Wetterdaten, ehrlich
        /// </summary>        
        /// <returns>Wetterdaten für NOW</returns>
        /// <response code="200">Die Wetterdaten</response>
        [HttpGet(Name = "GetWeatherForecast")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WeatherForecast>))]
        public ActionResult<IEnumerable<WeatherForecast>> Get()
        {
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            return Ok(result);
        }

        /// <summary>
        /// Ermittelt einen spezifischen Forecast
        /// </summary>     
        /// <param name="id">Id des Wettereintrags</param>
        /// <returns>Wetterdaten by Id</returns>
        /// <response code="200">Die Wetterdaten</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WeatherForecast))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
        public ActionResult<WeatherForecast> GetById(string id)
        {
            var result = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };

            return Ok(result);
        }
    }
}