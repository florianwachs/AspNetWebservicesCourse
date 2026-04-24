# Lab – Testing  🧪

## Learning Objectives

| # | Objective |
|---|---|
| 1 | Write API **integration** tests with `WebApplicationFactory` and an in-memory database |
| 2 | Run **closed-box distributed-app integration tests** with `Aspire.Hosting.Testing` and the AppHost |
| 3 | Author end-to-end (E2E) tests for a **React + Minimal API** application using **Playwright** on top of the same Aspire test harness |

## Scenario

You are building **TechConf**, a small conference-management platform.
The solution consists of:

| Project | Purpose |
|---|---|
| `TechConf.Api` | Minimal API with Events CRUD, backed by PostgreSQL (EF Core) |
| `TechConf.AppHost` | .NET Aspire orchestrator (PostgreSQL + API + React frontend) |
| `TechConf.ServiceDefaults` | Shared Aspire defaults (OpenTelemetry, health checks, resilience) |
| `TechConf.Web` | React / Vite frontend that lists and creates events |
| `TechConf.Api.Tests` | **Task 1** – API-only open-box tests with `WebApplicationFactory` |
| `TechConf.AppHost.Tests` | **Task 2** – closed-box distributed-app tests with Aspire testing |
| `TechConf.E2E.Tests` | **Task 3** – Playwright E2E tests on top of the Aspire harness |

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

## Task 1 – API Tests with WebApplicationFactory

**Project:** `TechConf.Api.Tests`

Use `WebApplicationFactory<Program>` to spin up the API **in-process** with an
in-memory database (no Docker required).

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
// Override DB in test fixture
builder.ConfigureServices(services =>
{
    // Remove the real DbContext
    var descriptor = services.SingleOrDefault(
        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
    if (descriptor is not null) services.Remove(descriptor);

    // Add in-memory database
    services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}"));
});
```

---

## Task 2 – Closed-box Integration Tests with Aspire Testing

**Project:** `TechConf.AppHost.Tests`

Use `DistributedApplicationTestingBuilder` to start the **full distributed application through the AppHost**.

This is Aspire's **closed-box integration testing** model:

- your test starts the **AppHost**,
- the AppHost starts the API, PostgreSQL, and other resources,
- and your test interacts with the named resource from the outside.

Because the AppHost orchestrates the real PostgreSQL resource, this still gives you realistic persistence behavior without manually wiring the database in the test project.

### What to implement

| # | Step | Hint |
|---|---|---|
| 1 | Create a test that starts the AppHost and verifies the API health endpoint | `GET /health` → 200 |
| 2 | `ApiService_ReturnsOk_ForEventsEndpoint` | `GET /api/events` → 200 |
| 3 | `ApiService_CanCreateAndRetrieveEvent_WithPostgres` | POST then GET to verify data is persisted in real Postgres |

### Key APIs

```csharp
// Create and start the AppHost-driven distributed app
var appHost = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.TechConf_AppHost>();

await using var app = await appHost.BuildAsync();
await app.StartAsync();

// Get an HttpClient for a named Aspire resource
var httpClient = app.CreateHttpClient("api");
```

> **Note:** Docker must be running. Aspire manages the real infrastructure resources that the AppHost defines, so your test is exercising the actual distributed setup rather than a mocked replacement.

---

## Task 3 – End-to-End Tests with Playwright on the Aspire Harness

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

## Stretch Goals

| # | Goal |
|---|---|
| 1 | Add a `PUT /api/events/{id}` endpoint and write tests for it in all three test projects |
| 2 | Add Playwright **screenshot-on-failure** using `Page.ScreenshotAsync()` in the test teardown |
| 3 | Use `IAsyncLifetime` with a shared `DistributedApplication` across all Aspire integration tests (test fixture) |
| 4 | Add a **Playwright trace** (`Tracing.StartAsync` / `Tracing.StopAsync`) to debug failing E2E tests |

---

## Running Tests

```bash
# Run these from labs/lab-testing/exercise
cd exercise

# Task 1 – WebApplicationFactory tests (no Docker needed)
dotnet test TechConf.Api.Tests

# Task 2 – Aspire closed-box integration tests (Docker required)
dotnet test TechConf.AppHost.Tests

# Task 3 – Playwright E2E tests on the Aspire harness (Docker + Playwright browsers required)
dotnet test TechConf.E2E.Tests
```
