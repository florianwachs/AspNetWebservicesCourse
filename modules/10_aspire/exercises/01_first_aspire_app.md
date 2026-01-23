# Exercise 1: Your First Aspire Application

## 🎯 Objectives

In this exercise, you will:

- Create your first .NET Aspire application
- Understand the Aspire project structure
- Explore the Aspire dashboard
- Add health checks to your services
- Implement simple endpoints
- Test service-to-service communication

## ⏱️ Estimated Time

45-60 minutes

## 📋 Prerequisites

- .NET 8.0 SDK or later installed
- Docker Desktop running
- .NET Aspire workload installed
- Basic understanding of ASP.NET Core

## 🚀 Part 1: Create Your First Aspire Application

### Step 1: Create the Solution

Open a terminal and run:

```bash
# Create a working directory
mkdir aspire-labs
cd aspire-labs

# Create a new Aspire Starter Application
dotnet new aspire-starter -n MyFirstAspireApp
cd MyFirstAspireApp
```

### Step 2: Explore the Project Structure

Your solution should contain three projects:

```
MyFirstAspireApp/
├── MyFirstAspireApp.AppHost/          # Orchestrator project
├── MyFirstAspireApp.ServiceDefaults/  # Shared configuration
└── MyFirstAspireApp.ApiService/       # API project
```

#### Examine the AppHost Project

Open `MyFirstAspireApp.AppHost/Program.cs`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.MyFirstAspireApp_ApiService>("apiservice");

builder.AddProject<Projects.MyFirstAspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
```

**Key concepts:**
- `DistributedApplication` - The orchestrator
- `AddProject<T>()` - Adds a project to the app model
- `WithReference()` - Creates service-to-service communication
- `WithExternalHttpEndpoints()` - Exposes service externally

#### Examine ServiceDefaults

Open `MyFirstAspireApp.ServiceDefaults/Extensions.cs`:

This project contains shared configurations:
- Service discovery
- OpenTelemetry (logs, metrics, traces)
- Health checks
- HttpClient resilience

### Step 3: Run the Application

```bash
# Run from the AppHost directory
cd MyFirstAspireApp.AppHost
dotnet run
```

**Expected output:**
```
info: Aspire.Hosting.DistributedApplication[0]
      Aspire version: 8.x.x
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application starting.
info: Aspire.Hosting.DistributedApplication[0]
      Now listening on: https://localhost:17005
```

### Step 4: Explore the Aspire Dashboard

Open your browser to the URL shown in the output (typically `https://localhost:17005` or `http://localhost:15001`).

**Dashboard features to explore:**

1. **Resources Tab** - Shows all services and their status
2. **Console Logs** - Real-time logs from all services
3. **Structured Logs** - Searchable, filterable logs
4. **Traces** - Distributed tracing visualization
5. **Metrics** - Performance metrics and graphs

### ✅ Verification Point 1

**Check:**
- [ ] Dashboard opens successfully
- [ ] All services show "Running" status
- [ ] Console logs show activity
- [ ] You can navigate to the web frontend
- [ ] Web frontend can call the API service

## 🔨 Part 2: Add Custom Endpoints

### Step 1: Add a Weather Forecast Endpoint

The starter template includes a `/weatherforecast` endpoint. Let's add a custom endpoint.

Open `MyFirstAspireApp.ApiService/Program.cs` and add:

```csharp
app.MapGet("/hello/{name}", (string name) =>
{
    var greeting = new
    {
        Message = $"Hello, {name}!",
        Timestamp = DateTime.UtcNow,
        MachineName = Environment.MachineName
    };
    
    return Results.Ok(greeting);
});

app.MapGet("/health/details", () =>
{
    var details = new
    {
        Status = "Healthy",
        Version = "1.0.0",
        Uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime,
        MemoryUsage = GC.GetTotalMemory(false) / 1024 / 1024, // MB
        ThreadCount = Process.GetCurrentProcess().Threads.Count
    };
    
    return Results.Ok(details);
});
```

Add this using statement at the top:

```csharp
using System.Diagnostics;
```

### Step 2: Test the New Endpoints

The application should auto-reload. In the dashboard:

1. Find your API service in the Resources tab
2. Click on the endpoint URL
3. Navigate to `/hello/YourName`
4. Navigate to `/health/details`

**Expected response for `/hello/John`:**
```json
{
  "message": "Hello, John!",
  "timestamp": "2024-01-15T10:30:00Z",
  "machineName": "..."
}
```

### ✅ Verification Point 2

**Test in browser or with curl:**

```bash
# Get the API endpoint from the dashboard, then:
curl https://localhost:7XXX/hello/Student
curl https://localhost:7XXX/health/details
```

## 🔧 Part 3: Add Health Checks

### Step 1: Add Custom Health Check

Create a new file `MyFirstAspireApp.ApiService/HealthChecks/CustomHealthCheck.cs`:

```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyFirstAspireApp.ApiService.HealthChecks;

public class CustomHealthCheck : IHealthCheck
{
    private readonly ILogger<CustomHealthCheck> _logger;

    public CustomHealthCheck(ILogger<CustomHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simulate checking a critical resource
            var memoryUsed = GC.GetTotalMemory(false) / 1024 / 1024; // MB
            
            if (memoryUsed > 500) // More than 500 MB
            {
                return Task.FromResult(
                    HealthCheckResult.Degraded($"High memory usage: {memoryUsed} MB"));
            }

            return Task.FromResult(
                HealthCheckResult.Healthy($"Memory usage normal: {memoryUsed} MB"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return Task.FromResult(
                HealthCheckResult.Unhealthy("Health check failed", ex));
        }
    }
}
```

### Step 2: Register the Health Check

In `MyFirstAspireApp.ApiService/Program.cs`, add after `var builder = WebApplication.CreateBuilder(args);`:

```csharp
using MyFirstAspireApp.ApiService.HealthChecks;

// ... existing code ...

// Add custom health check
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom_check");
```

### Step 3: View Health Check Results

The ServiceDefaults already configures health check endpoints. Navigate to:

```
https://localhost:7XXX/health
```

**Expected response:**
```json
{
  "status": "Healthy",
  "results": {
    "custom_check": {
      "status": "Healthy",
      "description": "Memory usage normal: 45 MB"
    }
  }
}
```

### ✅ Verification Point 3

**Check:**
- [ ] `/health` endpoint returns healthy status
- [ ] Custom health check appears in results
- [ ] Dashboard shows service as healthy

## 🔄 Part 4: Service-to-Service Communication

### Step 1: Add a Service Client in the Web Frontend

Open `MyFirstAspireApp.Web/Program.cs` and find where the API service HTTP client is registered:

```csharp
builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});
```

### Step 2: Create a New Service Client

Create `MyFirstAspireApp.Web/GreetingApiClient.cs`:

```csharp
namespace MyFirstAspireApp.Web;

public class GreetingApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GreetingApiClient> _logger;

    public GreetingApiClient(HttpClient httpClient, ILogger<GreetingApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GreetingResponse?> GetGreetingAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling greeting API for {Name}", name);
            
            var response = await _httpClient.GetFromJsonAsync<GreetingResponse>(
                $"/hello/{name}", 
                cancellationToken);
            
            _logger.LogInformation("Successfully received greeting");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get greeting for {Name}", name);
            return null;
        }
    }
}

public record GreetingResponse(string Message, DateTime Timestamp, string MachineName);
```

### Step 3: Register the Client

In `MyFirstAspireApp.Web/Program.cs`, add:

```csharp
builder.Services.AddHttpClient<GreetingApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});
```

### Step 4: Add a Razor Component

Create `MyFirstAspireApp.Web/Components/Pages/Greeting.razor`:

```razor
@page "/greeting"
@using MyFirstAspireApp.Web
@inject GreetingApiClient GreetingClient

<PageTitle>Greeting</PageTitle>

<h1>Greeting Service</h1>

<div class="mb-3">
    <label for="nameInput" class="form-label">Enter your name:</label>
    <input type="text" class="form-control" id="nameInput" @bind="name" @bind:event="oninput" />
</div>

<button class="btn btn-primary" @onclick="GetGreeting" disabled="@(string.IsNullOrWhiteSpace(name))">
    Get Greeting
</button>

@if (isLoading)
{
    <div class="mt-3">
        <div class="spinner-border" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    </div>
}

@if (greeting != null)
{
    <div class="mt-3 alert alert-success">
        <h4>@greeting.Message</h4>
        <p><small>From: @greeting.MachineName</small></p>
        <p><small>At: @greeting.Timestamp.ToLocalTime()</small></p>
    </div>
}

@if (error != null)
{
    <div class="mt-3 alert alert-danger">
        <strong>Error:</strong> @error
    </div>
}

@code {
    private string name = "";
    private GreetingResponse? greeting;
    private string? error;
    private bool isLoading;

    private async Task GetGreeting()
    {
        isLoading = true;
        error = null;
        greeting = null;

        try
        {
            greeting = await GreetingClient.GetGreetingAsync(name);
            
            if (greeting == null)
            {
                error = "Failed to get greeting from API";
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

### Step 5: Add Navigation Link

Open `MyFirstAspireApp.Web/Components/Layout/NavMenu.razor` and add:

```razor
<div class="nav-item px-3">
    <NavLink class="nav-link" href="greeting">
        <span class="bi bi-chat-dots-fill" aria-hidden="true"></span> Greeting
    </NavLink>
</div>
```

### Step 6: Test Service Communication

1. Navigate to the web frontend in your browser
2. Click on "Greeting" in the navigation
3. Enter your name and click "Get Greeting"
4. Observe the response from the API service

### ✅ Verification Point 4

**In the Aspire Dashboard:**

1. Go to **Traces** tab
2. Find traces for your greeting request
3. Expand the trace to see:
   - Web frontend HTTP request
   - API service handling
   - Response flow

**Check:**
- [ ] Greeting page displays correctly
- [ ] Service call succeeds
- [ ] Trace shows end-to-end request flow
- [ ] Logs show both services communicating

## 📊 Part 5: Explore Observability

### Step 1: Generate Traffic

Make several requests to your greeting endpoint with different names.

### Step 2: Explore Structured Logs

In the dashboard's **Structured Logs** tab:

1. Filter by service: Select "apiservice"
2. Filter by log level: Try different levels (Information, Warning, Error)
3. Search for text: Search for "Calling greeting API"
4. Click on a log entry to see full details

### Step 3: Explore Traces

In the **Traces** tab:

1. Find a trace that spans both services
2. Click to expand the trace details
3. Observe:
   - Total duration
   - Individual span durations
   - Service names
   - HTTP status codes

### Step 4: Explore Metrics

In the **Metrics** tab:

1. Select "apiservice"
2. View metrics like:
   - HTTP request rate
   - HTTP request duration
   - Process memory
   - CPU usage

### ✅ Verification Point 5

**Check:**
- [ ] Can filter logs by service and level
- [ ] Can view detailed trace spans
- [ ] Can see metrics graphs
- [ ] Understand the observability data flow

## 🎯 Challenge Tasks

Try these on your own:

1. **Add a POST endpoint** that accepts JSON and returns a response
2. **Add logging** to track how many times each name is greeted
3. **Create a failing health check** and observe it in the dashboard
4. **Add a delay** to the API endpoint and see it in traces
5. **Add custom metrics** using `IMeterFactory`

### Challenge 1 Solution Hint:

```csharp
app.MapPost("/greet", (GreetRequest request, ILogger<Program> logger) =>
{
    logger.LogInformation("Greeting {Name} with style {Style}", request.Name, request.Style);
    
    var message = request.Style switch
    {
        "formal" => $"Good day, {request.Name}.",
        "casual" => $"Hey {request.Name}!",
        _ => $"Hello, {request.Name}!"
    };
    
    return Results.Ok(new { Message = message });
});

record GreetRequest(string Name, string Style);
```

## 📝 Summary

In this exercise, you:

- ✅ Created your first .NET Aspire application
- ✅ Explored the Aspire project structure
- ✅ Used the Aspire dashboard for observability
- ✅ Added custom endpoints and health checks
- ✅ Implemented service-to-service communication
- ✅ Explored logs, traces, and metrics

## 🧹 Cleanup

To stop the application:

1. Press `Ctrl+C` in the terminal running the app
2. Docker containers will be automatically stopped

To remove Docker containers:

```bash
docker container prune
```

## 🎓 Key Takeaways

1. **AppHost orchestrates** all services and dependencies
2. **ServiceDefaults provide** consistent configuration across services
3. **Dashboard is essential** for development and debugging
4. **Service discovery** works automatically with `https+http://servicename`
5. **Observability is built-in** - no additional configuration needed

## ➡️ Next Steps

Continue to [Exercise 2: Adding Aspire Components](02_adding_components.md) to learn how to integrate databases, caching, and messaging.

## 📚 Additional Resources

- [.NET Aspire documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Aspire Dashboard documentation](https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard)
- [Service Discovery in Aspire](https://learn.microsoft.com/dotnet/aspire/service-discovery/overview)
