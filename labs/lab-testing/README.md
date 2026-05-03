# Lab – Testing  🧪

## Learning Objectives

| # | Objective |
|---|---|
| 1 | Run **closed-box distributed-app integration tests** with `Aspire.Hosting.Testing` and the AppHost |
| 2 | Author end-to-end (E2E) tests for a **React + Minimal API** application using **Playwright** on top of the same Aspire test harness |
| 3 | Use `WebApplicationFactory` as an **optional focused open-box** technique when you need single-service overrides |

## Scenario

You are building **TechConf**, a small conference-management platform.
The solution consists of:

| Project | Purpose |
|---|---|
| `TechConf.Api` | Minimal API with Events CRUD, backed by PostgreSQL (EF Core) |
| `TechConf.AppHost` | .NET Aspire orchestrator (PostgreSQL + API + React frontend) |
| `TechConf.ServiceDefaults` | Shared Aspire defaults (OpenTelemetry, health checks, resilience) |
| `TechConf.Web` | React / Vite frontend that lists and creates events |
| `TechConf.AppHost.Tests` | **Task 1** – closed-box distributed-app tests with Aspire testing |
| `TechConf.E2E.Tests` | **Task 2** – Playwright E2E tests on top of the same Aspire harness |
| `TechConf.Api.Tests` | **Optional Task 3** – API-only open-box tests with `WebApplicationFactory` |

## Prerequisites Setup

```bash
# Ensure Docker is running (needed for Aspire-managed infrastructure resources)
docker info

# Install Playwright browsers (needed for E2E tests)
# After building TechConf.E2E.Tests, run from the exercise directory:
cd exercise
pwsh TechConf.E2E.Tests/bin/Debug/net10.0/playwright.ps1 install
```

---

## Task 1 – Closed-box Integration Tests with Aspire Testing

**Project:** `TechConf.AppHost.Tests`

Use `DistributedApplicationTestingBuilder` to start the **full distributed application through the AppHost**.

This lab follows Aspire's **closed-box integration testing** model:

- your test references the **AppHost**, not the API project,
- the AppHost starts the API, PostgreSQL, and other resources,
- and your test interacts with named resources from the outside.

Because the AppHost orchestrates the real PostgreSQL resource, this gives you realistic persistence behavior **without** manual Testcontainers setup in the test project.

### What to implement

| # | Step | Hint |
|---|---|---|
| 1 | `ApiService_HealthCheck_ReturnsHealthy` | Start the AppHost, wait for `"api"`, then `GET /health` → 200 |
| 2 | `ApiService_ReturnsOk_ForEventsEndpoint` | `GET /api/events` → 200 |
| 3 | `ApiService_CanCreateAndRetrieveEvent_WithPostgres` | POST then GET to verify data is persisted in real Postgres |
| 4 | `WebResource_ReceivesApiServiceDiscoveryConfiguration` | Inspect the `"web"` resource run-time environment variables for `services__api__*` |

### Key APIs

```csharp
// Create and start the AppHost-driven distributed app
var appHost = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.TechConf_AppHost>();

await using var app = await appHost.BuildAsync();
await app.StartAsync();

// Wait for a named Aspire resource to be ready before calling it
await app.ResourceNotifications.WaitForResourceAsync(
    "api", KnownResourceStates.Running);

// Get an HttpClient for a named Aspire resource
var httpClient = app.CreateHttpClient("api");
```

---

## Task 2 – End-to-End Tests with Playwright on the Aspire Harness

**Project:** `TechConf.E2E.Tests`

Build on the same Aspire testing harness from Task 2, then add **Playwright** to automate a real browser for true end-to-end tests.

### What to implement

| # | Step | Hint |
|---|---|---|
| 1 | Set up `IAsyncLifetime` to start the AppHost-driven Aspire test app + launch a Playwright browser | Wait for the `"web"` resource with `_app.ResourceNotifications` |
| 2 | `HomePage_ShowsTitle` | Navigate to the React app, assert `<h1>TechConf Events</h1>` |
| 3 | `CanCreateEvent_AndSeeItInList` | Fill form fields (`data-testid` selectors), submit, assert the event appears |

### Key APIs

```csharp
// Start Aspire and wait for the web frontend
await _app.StartAsync();
await _app.ResourceNotifications.WaitForResourceAsync(
    "web", KnownResourceStates.Running);

// Launch Playwright
_playwright = await Playwright.CreateAsync();
_browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });

// Get the web app URL
var webUrl = _app.GetEndpoint("web", "http").ToString();
var page = await _browser.NewPageAsync();
await page.GotoAsync(webUrl);
```

> **Tip:** The React app uses `data-testid` attributes on all interactive elements — use `[data-testid='event-title']` etc. in your Playwright selectors.

---

## Optional Task 3 – API Tests with WebApplicationFactory

**Project:** `TechConf.Api.Tests`

Use `WebApplicationFactory<Program>` to spin up the API **in-process** with an
in-memory database when you want a focused, single-service test with direct overrides and no Docker requirement.

This is the **open-box** testing path. It is still useful, but in this lab it is intentionally secondary to the AppHost-driven Aspire harness.

### What to implement

| # | Step | Hint |
|---|---|---|
| 1 | Create a test class that uses `IClassFixture<WebApplicationFactory<Program>>` | Override `ConfigureServices` to replace `AppDbContext` with `UseInMemoryDatabase` |
| 2 | `GetEvents_ReturnsEmptyList_WhenNoEvents` | `GET /api/events` → 200 with `[]` |
| 3 | `PostEvent_CreatesEvent_ReturnsCreated` | `POST /api/events` → 201 with the created event |
| 4 | `GetEventById_ReturnsEvent_WhenExists` | Create an event first, then `GET /api/events/{id}` → 200 |
| 5 | `GetEventById_ReturnsNotFound_WhenDoesNotExist` | `GET /api/events/{random-guid}` → 404 |
| 6 | `DeleteEvent_ReturnsNoContent_WhenExists` | Create, delete, then verify it's gone |

### Key APIs

```csharp
builder.ConfigureServices(services =>
{
    var descriptor = services.SingleOrDefault(
        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
    if (descriptor is not null) services.Remove(descriptor);

    services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}"));
});
```

---

## Stretch Goals

| # | Goal |
|---|---|
| 1 | Add a `PUT /api/events/{id}` endpoint and write tests for all three test projects |
| 2 | Add Playwright **screenshot-on-failure** using `Page.ScreenshotAsync()` in the test teardown |
| 3 | Use `IAsyncLifetime` with a shared `DistributedApplication` across the Aspire integration tests (test fixture) |
| 4 | Add a **Playwright trace** (`Tracing.StartAsync` / `Tracing.StopAsync`) to debug failing E2E tests |

---

## Running Tests

```bash
# Run these from labs/lab-testing/exercise
cd exercise

# Task 1 – Aspire closed-box integration tests (Docker required)
dotnet test TechConf.AppHost.Tests

# Task 2 – Playwright E2E tests on the Aspire harness (Docker + Playwright browsers required)
dotnet test TechConf.E2E.Tests

# Optional Task 3 – WebApplicationFactory tests (no Docker needed)
dotnet test TechConf.Api.Tests
```
