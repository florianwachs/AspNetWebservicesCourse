using System.Net.Http.Json;
using TechConf.Microservices.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient("events", client => client.BaseAddress = new Uri("http://events"));
builder.Services.AddHttpClient("sessions", client => client.BaseAddress = new Uri("http://sessions"));
builder.Services.AddHttpClient("registrations", client => client.BaseAddress = new Uri("http://registrations"));
builder.Services.AddHttpClient("notifications", client => client.BaseAddress = new Uri("http://notifications"));
builder.Services.AddHttpClient("recommendations", client => client.BaseAddress = new Uri("http://recommendations"));

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Redirect("/api/dashboard"));

var api = app.MapGroup("/api");

api.MapGet("/events", async (IHttpClientFactory clients, CancellationToken cancellationToken) =>
{
    var events = await ReadAsync<IReadOnlyList<EventSummary>>(
        clients.CreateClient("events"),
        "/events",
        cancellationToken);

    return Results.Ok(events);
});

api.MapGet("/events/{eventId:guid}/sessions", async (
    Guid eventId,
    IHttpClientFactory clients,
    CancellationToken cancellationToken) =>
{
    var sessions = await ReadAsync<IReadOnlyList<SessionSummary>>(
        clients.CreateClient("sessions"),
        $"/events/{eventId}/sessions",
        cancellationToken);

    return Results.Ok(sessions);
});

api.MapGet("/notifications/recent", async (IHttpClientFactory clients, CancellationToken cancellationToken) =>
{
    var notifications = await ReadAsync<IReadOnlyList<NotificationSummary>>(
        clients.CreateClient("notifications"),
        "/notifications/recent",
        cancellationToken);

    return Results.Ok(notifications);
});

api.MapGet("/recommendations", async (
    Guid? eventId,
    IHttpClientFactory clients,
    CancellationToken cancellationToken) =>
{
    var path = eventId.HasValue
        ? $"/recommendations?eventId={Uri.EscapeDataString(eventId.Value.ToString())}"
        : "/recommendations";

    var recommendations = await ReadAsync<IReadOnlyList<RecommendationSummary>>(
        clients.CreateClient("recommendations"),
        path,
        cancellationToken);

    return Results.Ok(recommendations);
});

api.MapPost("/registrations", async (
    RegistrationRequest request,
    IHttpClientFactory clients,
    CancellationToken cancellationToken) =>
{
    var sessions = clients.CreateClient("sessions");
    var registrations = clients.CreateClient("registrations");

    var sessionResponse = await sessions.GetAsync($"/sessions/{request.SessionId}", cancellationToken);

    if (sessionResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        return Results.BadRequest(new { error = "The selected session does not exist." });
    }

    sessionResponse.EnsureSuccessStatusCode();
    var session = await sessionResponse.Content.ReadFromJsonAsync<SessionSummary>(cancellationToken);

    if (session is null || session.EventId != request.EventId)
    {
        return Results.BadRequest(new { error = "The selected session does not belong to the selected event." });
    }

    var registrationResponse = await registrations.PostAsJsonAsync("/registrations", request, cancellationToken);
    var body = await registrationResponse.Content.ReadAsStringAsync(cancellationToken);

    return Results.Content(body, "application/json", statusCode: (int)registrationResponse.StatusCode);
});

api.MapGet("/dashboard", async (IHttpClientFactory clients, CancellationToken cancellationToken) =>
{
    var eventCountTask = ReadAsync<CountResponse>(clients.CreateClient("events"), "/events/count", cancellationToken);
    var sessionCountTask = ReadAsync<CountResponse>(clients.CreateClient("sessions"), "/sessions/count", cancellationToken);
    var registrationCountTask = ReadAsync<CountResponse>(clients.CreateClient("registrations"), "/registrations/count", cancellationToken);
    var notificationCountTask = ReadAsync<CountResponse>(clients.CreateClient("notifications"), "/notifications/count", cancellationToken);
    var recommendationCountTask = ReadAsync<CountResponse>(clients.CreateClient("recommendations"), "/recommendations/count", cancellationToken);

    await Task.WhenAll(eventCountTask, sessionCountTask, registrationCountTask, notificationCountTask, recommendationCountTask);

    var dashboard = new DashboardResponse(
        eventCountTask.Result.Count,
        sessionCountTask.Result.Count,
        registrationCountTask.Result.Count,
        notificationCountTask.Result.Count,
        recommendationCountTask.Result.Count,
        [
            new ServiceSnapshot("Gateway / BFF", "Frontend API composition", "ASP.NET Core", "none", "HTTP to internal services", "Healthy"),
            new ServiceSnapshot("Events", "Event catalog", "ASP.NET Core + EF Core", "eventsdb", "HTTP", "Healthy"),
            new ServiceSnapshot("Sessions", "Schedule and seat inventory", "ASP.NET Core + EF Core", "sessionsdb", "HTTP", "Healthy"),
            new ServiceSnapshot("Registrations", "Attendee registration", "ASP.NET Core + EF Core", "registrationsdb", "HTTP + RabbitMQ publish", "Healthy"),
            new ServiceSnapshot("Notifications", "Registration confirmations", "Worker + Minimal API", "notificationsdb", "RabbitMQ consume + HTTP", "Healthy"),
            new ServiceSnapshot("Recommendations", "Session recommendations", "Node.js", "none", "HTTP", "Healthy")
        ]);

    return Results.Ok(dashboard);
});

app.Run();

static async Task<T> ReadAsync<T>(HttpClient client, string path, CancellationToken cancellationToken)
{
    var response = await client.GetAsync(path, cancellationToken);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
        ?? throw new InvalidOperationException($"Empty response from {path}.");
}
