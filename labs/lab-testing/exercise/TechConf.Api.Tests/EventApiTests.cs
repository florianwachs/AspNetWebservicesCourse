using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechConf.Api.Data;
using TechConf.Api.Models;
using Xunit;

namespace TechConf.Api.Tests;

public class EventApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EventApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // TODO: Remove the existing DbContextOptions<AppDbContext> registration
                // Hint: Use services.SingleOrDefault(...) to find it, then services.Remove(...)

                // TODO: Add an in-memory database for testing
                // Hint: services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(...))
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetEvents_ReturnsEmptyList_WhenNoEvents()
    {
        // TODO: Send GET request to /api/events
        // TODO: Assert the response status is success (200)
        // TODO: Deserialize the response body as List<Event>
        // TODO: Assert the list is not null and is empty
        throw new NotImplementedException();
    }

    [Fact]
    public async Task PostEvent_CreatesEvent_ReturnsCreated()
    {
        // TODO: Create a new Event object with Title, Description, Date, Location
        // TODO: Send POST request to /api/events with the event as JSON
        // TODO: Assert the response status is 201 Created
        // TODO: Deserialize the response and assert the Title matches
        // TODO: Assert the Id is not Guid.Empty
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetEventById_ReturnsEvent_WhenExists()
    {
        // TODO: First create an event via POST /api/events
        // TODO: Extract the created event's Id from the response
        // TODO: Send GET request to /api/events/{id}
        // TODO: Assert the response is successful and the event data matches
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetEventById_ReturnsNotFound_WhenDoesNotExist()
    {
        // TODO: Send GET request to /api/events/{random-guid}
        // TODO: Assert the response status is 404 NotFound
        throw new NotImplementedException();
    }

    [Fact]
    public async Task DeleteEvent_ReturnsNoContent_WhenExists()
    {
        // TODO: Create an event via POST
        // TODO: Delete it via DELETE /api/events/{id}
        // TODO: Assert the delete response is 204 NoContent
        // TODO: Verify GET /api/events/{id} now returns 404
        throw new NotImplementedException();
    }
}
