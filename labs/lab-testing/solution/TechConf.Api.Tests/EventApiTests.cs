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
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor is not null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}"));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetEvents_ReturnsEmptyList_WhenNoEvents()
    {
        var response = await _client.GetAsync("/api/events");

        response.EnsureSuccessStatusCode();
        var events = await response.Content.ReadFromJsonAsync<List<Event>>();
        Assert.NotNull(events);
        Assert.Empty(events);
    }

    [Fact]
    public async Task PostEvent_CreatesEvent_ReturnsCreated()
    {
        var newEvent = new Event
        {
            Title = "Unit Test Conference",
            Description = "Testing with WebApplicationFactory",
            Date = new DateTime(2026, 6, 15),
            Location = "Munich"
        };

        var response = await _client.PostAsJsonAsync("/api/events", newEvent);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Event>();
        Assert.NotNull(created);
        Assert.Equal("Unit Test Conference", created.Title);
        Assert.NotEqual(Guid.Empty, created.Id);
    }

    [Fact]
    public async Task GetEventById_ReturnsEvent_WhenExists()
    {
        var newEvent = new Event
        {
            Title = "Findable Event",
            Description = "Should be found by ID",
            Date = new DateTime(2026, 7, 1),
            Location = "Berlin"
        };
        var postResponse = await _client.PostAsJsonAsync("/api/events", newEvent);
        var created = await postResponse.Content.ReadFromJsonAsync<Event>();

        var response = await _client.GetAsync($"/api/events/{created!.Id}");

        response.EnsureSuccessStatusCode();
        var found = await response.Content.ReadFromJsonAsync<Event>();
        Assert.NotNull(found);
        Assert.Equal("Findable Event", found.Title);
    }

    [Fact]
    public async Task GetEventById_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await _client.GetAsync($"/api/events/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteEvent_ReturnsNoContent_WhenExists()
    {
        var newEvent = new Event
        {
            Title = "Deletable Event",
            Description = "Will be deleted",
            Date = new DateTime(2026, 8, 1),
            Location = "Hamburg"
        };
        var postResponse = await _client.PostAsJsonAsync("/api/events", newEvent);
        var created = await postResponse.Content.ReadFromJsonAsync<Event>();

        var deleteResponse = await _client.DeleteAsync($"/api/events/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/events/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
