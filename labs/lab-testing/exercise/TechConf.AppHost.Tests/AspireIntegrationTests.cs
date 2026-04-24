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
        // TODO: Create a DistributedApplicationTestingBuilder for TechConf_AppHost.
        // This starts the distributed app through the AppHost instead of hosting the API directly.
        // Hint: var appHost = await DistributedApplicationTestingBuilder
        //           .CreateAsync<Projects.TechConf_AppHost>();

        // TODO: Build and start the AppHost-driven test application.
        // Hint: await using var app = await appHost.BuildAsync();
        //       await app.StartAsync();

        // TODO: Create an HttpClient for the named "api" resource.
        // Hint: var httpClient = app.CreateHttpClient("api");

        // TODO: Send GET request to /health and assert 200 OK
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ApiService_ReturnsOk_ForEventsEndpoint()
    {
        // TODO: Start the AppHost-driven Aspire test app (same pattern as above).
        // TODO: Create an HttpClient for the named "api" resource.
        // TODO: Send GET request to /api/events
        // TODO: Assert the response status is 200 OK
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ApiService_CanCreateAndRetrieveEvent_WithPostgres()
    {
        // TODO: Start the AppHost-driven Aspire test app.
        // TODO: Create an HttpClient for the named "api" resource.

        // TODO: Create and POST a new Event to /api/events
        // TODO: Assert 201 Created and deserialize the response

        // TODO: GET the event by its Id from /api/events/{id}
        // TODO: Assert the retrieved event matches the created one
        // This verifies that the AppHost-wired API really persists data to Postgres.
        throw new NotImplementedException();
    }
}
