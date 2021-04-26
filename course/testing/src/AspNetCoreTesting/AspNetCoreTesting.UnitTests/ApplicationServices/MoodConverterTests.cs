using AspNetCoreTesting.Api;
using AspNetCoreTesting.Api.ApplicationServices;
using System.Collections.Generic;
using Xunit;

namespace AspNetCoreTesting.UnitTests.ApplicationServices
{
    public class MoodConverterTests
    {

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void ShouldConvertForecastToCorrectMood(WeatherForecast forecast, string expectedEmoji)
        {
            var sut = new WeatherMoodConverter();
            Assert.Equal(expectedEmoji, sut.WeatherToMood(forecast));
        }

        public static IEnumerable<object[]> GetTestData => new[]
        {
            new object[]{new WeatherForecast { TemperatureC= -80}, "🥶" },
            new object[]{new WeatherForecast { TemperatureC= -5, Summary="Sunny"}, "😨" },
            new object[]{new WeatherForecast { TemperatureC= 45, Summary="Melting"}, "🥵" },
            new object[]{new WeatherForecast { TemperatureC= 12, Summary="Rainy"}, "😐" },
        };
    }
}
