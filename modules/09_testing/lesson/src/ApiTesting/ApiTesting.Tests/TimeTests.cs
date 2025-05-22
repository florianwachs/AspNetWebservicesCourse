using System.Net.Http.Json;
using ApiTesting.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTesting.Tests;

public class TimeTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task ReturnsCorrectTime()
    {
        var testTime = new DateTimeOffset(2023, 10, 1, 12, 0, 0, TimeSpan.Zero);
        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<TimeProvider>(_ => new SpecificTimeProvider(testTime));
            });
        }).CreateClient();

        var timestring = await client.GetFromJsonAsync<string>("/time");
        var time = DateTimeOffset.Parse(timestring);
        Assert.Equal(testTime, time);
    }
}