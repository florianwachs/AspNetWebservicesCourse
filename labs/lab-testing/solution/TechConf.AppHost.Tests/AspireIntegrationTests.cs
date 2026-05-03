using System.Net;
using System.Net.Http.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
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
        await app.ResourceNotifications.WaitForResourceAsync(
            "api", KnownResourceStates.Running);

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
        await app.ResourceNotifications.WaitForResourceAsync(
            "api", KnownResourceStates.Running);

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
        await app.ResourceNotifications.WaitForResourceAsync(
            "api", KnownResourceStates.Running);

        var httpClient = app.CreateHttpClient("api");

        var newEvent = new CreateEventRequest
        {
            Title = "Aspire Integration Test Event",
            Description = "Created during Aspire integration test with real Postgres",
            Date = new DateTime(2026, 9, 1),
            Location = "Frankfurt"
        };

        var postResponse = await httpClient.PostAsJsonAsync("/api/events", newEvent);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<EventResponse>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Id);

        var getResponse = await httpClient.GetAsync($"/api/events/{created.Id}");
        getResponse.EnsureSuccessStatusCode();

        var retrieved = await getResponse.Content.ReadFromJsonAsync<EventResponse>();
        Assert.NotNull(retrieved);
        Assert.Equal("Aspire Integration Test Event", retrieved.Title);
    }

    [Fact]
    public async Task WebResource_ReceivesApiServiceDiscoveryConfiguration()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.TechConf_AppHost>();

        var web = Assert.IsAssignableFrom<IResourceWithEnvironment>(
            appHost.Resources.Single(r => r.Name == "web"));

        await using var app = await appHost.BuildAsync();

        var context = new DistributedApplicationExecutionContext(
            new DistributedApplicationExecutionContextOptions(
                DistributedApplicationOperation.Run)
            {
                ServiceProvider = app.Services
            });

        var configuration = await ExecutionConfigurationBuilder
            .Create(web)
            .WithEnvironmentVariablesConfig()
            .BuildAsync(context);

        var envVars = configuration.EnvironmentVariables;

        Assert.Contains(envVars, pair =>
            (pair.Key == "services__api__http__0" || pair.Key == "services__api__https__0")
            && pair.Value.Contains("{api.bindings."));
    }
}

public sealed class CreateEventRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
}

public sealed class EventResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
}
