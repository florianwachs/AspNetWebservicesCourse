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
        // TODO: Create a DistributedApplicationTestingBuilder for TechConf_AppHost.
        // This starts the distributed app through the AppHost instead of hosting the API directly.
        // Hint: var appHost = await DistributedApplicationTestingBuilder
        //           .CreateAsync<Projects.TechConf_AppHost>();

        // TODO: Build and start the AppHost-driven test application.
        // Hint: await using var app = await appHost.BuildAsync();
        //       await app.StartAsync();
        //       await app.ResourceNotifications.WaitForResourceAsync(
        //           "api", KnownResourceStates.Running);

        // TODO: Create an HttpClient for the named "api" resource.
        // Hint: var httpClient = app.CreateHttpClient("api");

        // TODO: Send GET request to /health and assert 200 OK
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ApiService_ReturnsOk_ForEventsEndpoint()
    {
        // TODO: Start the AppHost-driven Aspire test app (same pattern as above).
        // TODO: Wait for the "api" resource to be running.
        // TODO: Create an HttpClient for the named "api" resource.
        // TODO: Send GET request to /api/events
        // TODO: Assert the response status is 200 OK
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ApiService_CanCreateAndRetrieveEvent_WithPostgres()
    {
        // TODO: Start the AppHost-driven Aspire test app.
        // TODO: Wait for the "api" resource to be running.
        // TODO: Create an HttpClient for the named "api" resource.

        // TODO: Create and POST a new Event to /api/events
        // TODO: Assert 201 Created and deserialize the response

        // TODO: GET the event by its Id from /api/events/{id}
        // TODO: Assert the retrieved event matches the created one
        // This verifies that the AppHost-wired API really persists data to Postgres.
        throw new NotImplementedException();
    }

    [Fact]
    public async Task WebResource_ReceivesApiServiceDiscoveryConfiguration()
    {
        // TODO: Create a DistributedApplicationTestingBuilder for TechConf_AppHost.
        // TODO: Find the "web" resource from appHost.Resources and cast it to IResourceWithEnvironment.
        // Hint: var web = Assert.IsAssignableFrom<IResourceWithEnvironment>(
        //           appHost.Resources.Single(r => r.Name == "web"));

        // TODO: Build the distributed application so you can provide app.Services to the
        // execution context used for configuration resolution.
        // Hint: await using var app = await appHost.BuildAsync();

        // TODO: Read the environment variables that would be resolved for the web resource at run time.
        // Hint: var context = new DistributedApplicationExecutionContext(
        //           new DistributedApplicationExecutionContextOptions(
        //               DistributedApplicationOperation.Run)
        //           {
        //               ServiceProvider = app.Services
        //           });
        //
        // Hint: var configuration = await ExecutionConfigurationBuilder
        //           .Create(web)
        //           .WithEnvironmentVariablesConfig()
        //           .BuildAsync(context);
        //       var envVars = configuration.EnvironmentVariables;

        // TODO: Assert the web resource receives Aspire service discovery information for the API.
        // Check for either services__api__http__0 or services__api__https__0.
        throw new NotImplementedException();
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
