using System.Net.Http.Json;
using ApiTesting.Tests.Infrastructure;

namespace ApiTesting.Tests;

public class WeatherApiV1Tests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task ReturnsWeatherForecast()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/weatherforecast");
        response.EnsureSuccessStatusCode();
        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        Assert.NotNull(forecasts);
        Assert.All(forecasts, item =>
        {
            Assert.NotNull(item);
            Assert.NotEqual(default, item.Date);
            Assert.InRange(item.TemperatureC, -20, 55);
            Assert.NotNull(item.Summary);
        });
    }
}