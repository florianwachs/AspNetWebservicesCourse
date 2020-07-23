using AspNetCoreTesting.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreTesting.IntegrationTests
{
    public class WeatherForecastTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient HttpClient { get; }

        public WeatherForecastTests(WebApplicationFactory<Startup> applicationFactory)
        {
            HttpClient = applicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async Task RetriveForecasts()
        {
            using var resultStream = await HttpClient.GetStreamAsync("/api/v1/weatherforecast");
            var sut = await JsonSerializer.DeserializeAsync<ExpectedForecast[]>(resultStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(sut);
            Assert.Equal(5, sut.Length);

            Assert.All(sut, s => Assert.NotEqual(DateTime.MinValue, s.Date));
        }

        public class ExpectedForecast
        {
            public DateTime Date { get; set; }
            public int TemperatureC { get; set; }
        }

    }
}
