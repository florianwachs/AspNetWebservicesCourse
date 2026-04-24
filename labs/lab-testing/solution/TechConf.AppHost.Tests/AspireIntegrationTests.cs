using System.Net;
using System.Net.Http.Json;
using Aspire.Hosting.Testing;
using TechConf.Api.Models;
using Xunit;

namespace TechConf.AppHost.Tests;

public class AspireIntegrationTests
{
    [Fact]
    public async Task ApiService_HealthCheck_ReturnsHealthy()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.TechConf_AppHost>();

        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("api");

        var response = await httpClient.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ApiService_ReturnsOk_ForEventsEndpoint()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.TechConf_AppHost>();

        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("api");

        var response = await httpClient.GetAsync("/api/events");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ApiService_CanCreateAndRetrieveEvent_WithPostgres()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.TechConf_AppHost>();

        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("api");

        var newEvent = new Event
        {
            Title = "Aspire Integration Test Event",
            Description = "Created during Aspire integration test with real Postgres",
            Date = new DateTime(2026, 9, 1),
            Location = "Frankfurt"
        };

        var postResponse = await httpClient.PostAsJsonAsync("/api/events", newEvent);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<Event>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Id);

        var getResponse = await httpClient.GetAsync($"/api/events/{created.Id}");
        getResponse.EnsureSuccessStatusCode();

        var retrieved = await getResponse.Content.ReadFromJsonAsync<Event>();
        Assert.NotNull(retrieved);
        Assert.Equal("Aspire Integration Test Event", retrieved.Title);
    }
}
