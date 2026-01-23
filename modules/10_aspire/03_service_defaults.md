# Lesson 3: Service Defaults - Standardizing Cross-Cutting Concerns

## Introduction

The ServiceDefaults project is where .NET Aspire implements shared, cross-cutting concerns for all your services. It provides a standardized way to configure observability, resilience, health checks, and HTTP client behavior across your distributed application.

Think of ServiceDefaults as the "shared configuration library" that ensures all your services behave consistently and follow cloud-native best practices.

## Table of Contents

1. [ServiceDefaults Overview](#servicedefaults-overview)
2. [Project Structure](#project-structure)
3. [OpenTelemetry Integration](#opentelemetry-integration)
4. [Logging Configuration](#logging-configuration)
5. [Metrics Collection](#metrics-collection)
6. [Distributed Tracing](#distributed-tracing)
7. [Health Checks](#health-checks)
8. [HTTP Resilience](#http-resilience)
9. [Service Discovery](#service-discovery)
10. [Customization](#customization)
11. [Best Practices](#best-practices)

## ServiceDefaults Overview

### What is ServiceDefaults?

ServiceDefaults is a shared class library project that:
- Provides extension methods for service configuration
- Configures OpenTelemetry for observability
- Sets up health check endpoints
- Configures HTTP client resilience patterns
- Enables service discovery
- Standardizes cross-cutting concerns

### The Purpose

```
┌──────────────────────────────────────────────────────────────┐
│                  Without ServiceDefaults                      │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  Every service needs to configure:                           │
│  • OpenTelemetry (30+ lines)                                │
│  • Health checks (10+ lines)                                 │
│  • Resilience patterns (20+ lines)                           │
│  • Service discovery (10+ lines)                             │
│                                                              │
│  Result: Duplicated code, inconsistent configuration         │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│                   With ServiceDefaults                        │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  In each service:                                            │
│  builder.AddServiceDefaults();  // One line!                 │
│                                                              │
│  Result: Consistent, maintainable, best practices enforced   │
└──────────────────────────────────────────────────────────────┘
```

### Key Responsibilities

1. **Observability**: OpenTelemetry configuration for logs, metrics, and traces
2. **Resilience**: HTTP client retry policies, circuit breakers, timeouts
3. **Health**: Health check endpoints for readiness and liveness
4. **Discovery**: Service discovery and HTTP client configuration
5. **Standards**: Enforcing organizational standards and best practices

## Project Structure

A typical ServiceDefaults project:

```
MyApp.ServiceDefaults/
├── Extensions.cs                # Main extension methods
├── MyApp.ServiceDefaults.csproj # Project file
└── README.md                    # Documentation
```

### Project File

**MyApp.ServiceDefaults.csproj**:
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireSharedProject>true</IsAspireSharedProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Http.Resilience" 
                      Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.ServiceDiscovery" 
                      Version="10.0.0" />
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" 
                      Version="1.10.0" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" 
                      Version="1.10.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" 
                      Version="1.10.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Http" 
                      Version="1.10.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Runtime" 
                      Version="1.10.0" />
  </ItemGroup>

</Project>
```

### Basic Extensions.cs

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(
        this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();
        builder.AddDefaultHealthChecks();
        builder.Services.AddServiceDiscovery();
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(
        this IHostApplicationBuilder builder)
    {
        // Implementation details below...
        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(
        this IHostApplicationBuilder builder)
    {
        // Implementation details below...
        return builder;
    }

    public static WebApplication MapDefaultEndpoints(
        this WebApplication app)
    {
        // Implementation details below...
        return app;
    }
}
```

## OpenTelemetry Integration

OpenTelemetry is the cornerstone of observability in Aspire applications.

### What is OpenTelemetry?

OpenTelemetry is a vendor-neutral observability framework that provides:
- **Logging**: Structured logs with context
- **Metrics**: Performance counters and measurements
- **Tracing**: Request flow across services

### OpenTelemetry Configuration

```csharp
public static IHostApplicationBuilder ConfigureOpenTelemetry(
    this IHostApplicationBuilder builder)
{
    builder.Logging.AddOpenTelemetry(logging =>
    {
        logging.IncludeFormattedMessage = true;
        logging.IncludeScopes = true;
    });

    builder.Services.AddOpenTelemetry()
        .WithMetrics(metrics =>
        {
            metrics.AddAspNetCoreInstrumentation()
                   .AddHttpClientInstrumentation()
                   .AddRuntimeInstrumentation();
        })
        .WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation()
                   .AddHttpClientInstrumentation();
        });

    builder.AddOpenTelemetryExporters();

    return builder;
}

private static IHostApplicationBuilder AddOpenTelemetryExporters(
    this IHostApplicationBuilder builder)
{
    var useOtlpExporter = !string.IsNullOrWhiteSpace(
        builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

    if (useOtlpExporter)
    {
        builder.Services.AddOpenTelemetry()
            .UseOtlpExporter();
    }

    return builder;
}
```

### How It Works

When running via AppHost:
1. AppHost starts the Aspire Dashboard
2. AppHost sets `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable
3. Services send telemetry to the Dashboard via OTLP protocol
4. Dashboard displays logs, metrics, and traces in real-time

```
┌─────────────┐         OTLP          ┌──────────────────┐
│   Service   │─────────────────────→│ Aspire Dashboard │
│             │    Logs/Metrics/     │                  │
│ ConfigureOTel()    Traces          │  Visualization   │
└─────────────┘                       └──────────────────┘
```

## Logging Configuration

### Enhanced Logging

ServiceDefaults enhances logging with:
- Structured logging support
- Log scopes included
- Formatted messages preserved
- Automatic context enrichment

```csharp
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;  // Keep formatted strings
    logging.IncludeScopes = true;            // Include log scopes
});
```

### Using Logging in Services

```csharp
// In your service
public class CatalogService(ILogger<CatalogService> logger)
{
    public async Task<Product> GetProductAsync(int id)
    {
        using var scope = logger.BeginScope(
            new Dictionary<string, object> { ["ProductId"] = id });

        logger.LogInformation("Fetching product {ProductId}", id);

        try
        {
            var product = await _repository.GetAsync(id);
            logger.LogInformation("Product found: {ProductName}", product.Name);
            return product;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching product {ProductId}", id);
            throw;
        }
    }
}
```

### Log Levels

Configure log levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "MyApp": "Debug"
    }
  }
}
```

### Structured Logging Best Practices

```csharp
// ✅ Good - structured logging
logger.LogInformation("User {UserId} purchased {ProductId} for {Price:C}", 
    userId, productId, price);

// ❌ Bad - string interpolation
logger.LogInformation($"User {userId} purchased {productId} for {price}");

// ✅ Good - exception logging
logger.LogError(ex, "Failed to process order {OrderId}", orderId);

// ❌ Bad - exception in message
logger.LogError($"Failed to process order {orderId}: {ex.Message}");
```

## Metrics Collection

### Built-in Metrics

ServiceDefaults automatically collects:

**ASP.NET Core Metrics**:
- Request duration
- Request rate
- Active requests
- Failed requests

**HTTP Client Metrics**:
- Request duration
- Request rate
- Failed requests per endpoint

**Runtime Metrics**:
- GC collections
- Memory usage
- Thread pool usage
- Exception rate

### Metrics Configuration

```csharp
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        // ASP.NET Core metrics
        metrics.AddAspNetCoreInstrumentation();
        
        // HTTP Client metrics
        metrics.AddHttpClientInstrumentation();
        
        // .NET Runtime metrics
        metrics.AddRuntimeInstrumentation();
        
        // Process metrics
        metrics.AddProcessInstrumentation();
    });
```

### Custom Metrics

Add custom metrics in your services:

```csharp
using System.Diagnostics.Metrics;

public class OrderService
{
    private static readonly Meter Meter = new("MyApp.Orders");
    private static readonly Counter<long> OrdersCreated = 
        Meter.CreateCounter<long>("orders.created");
    private static readonly Histogram<double> OrderValue = 
        Meter.CreateHistogram<double>("orders.value");

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = await _repository.CreateAsync(request);
        
        // Record metrics
        OrdersCreated.Add(1, 
            new KeyValuePair<string, object?>("customerId", request.CustomerId));
        OrderValue.Record(order.TotalAmount,
            new KeyValuePair<string, object?>("currency", order.Currency));
        
        return order;
    }
}
```

View custom metrics in the Aspire Dashboard under the Metrics tab.

## Distributed Tracing

### What is Distributed Tracing?

Distributed tracing tracks requests as they flow through multiple services:

```
User Request
    │
    ├─→ Frontend (Trace ID: abc123)
    │   ├─→ Catalog API (Span ID: 001)
    │   │   └─→ PostgreSQL (Span ID: 002)
    │   │
    │   └─→ Cart API (Span ID: 003)
    │       └─→ Redis (Span ID: 004)
    │
    └─→ Response
```

### Tracing Configuration

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        // Instrument ASP.NET Core
        tracing.AddAspNetCoreInstrumentation(options =>
        {
            options.RecordException = true;
            options.EnrichWithHttpRequest = (activity, request) =>
            {
                activity.SetTag("user.id", request.Headers["X-User-Id"]);
            };
        });
        
        // Instrument HTTP clients
        tracing.AddHttpClientInstrumentation(options =>
        {
            options.RecordException = true;
        });
        
        // Add your service as a source
        tracing.AddSource("MyApp.*");
    });
```

### Custom Spans

Create custom spans for business operations:

```csharp
using System.Diagnostics;

public class OrderService
{
    private static readonly ActivitySource ActivitySource = 
        new("MyApp.Orders");

    public async Task<Order> ProcessOrderAsync(int orderId)
    {
        using var activity = ActivitySource.StartActivity("ProcessOrder");
        activity?.SetTag("order.id", orderId);

        try
        {
            // Step 1: Validate
            using (ActivitySource.StartActivity("ValidateOrder"))
            {
                await ValidateOrderAsync(orderId);
            }

            // Step 2: Process payment
            using (ActivitySource.StartActivity("ProcessPayment"))
            {
                await ProcessPaymentAsync(orderId);
            }

            // Step 3: Update inventory
            using (ActivitySource.StartActivity("UpdateInventory"))
            {
                await UpdateInventoryAsync(orderId);
            }

            activity?.SetTag("order.status", "completed");
            return await _repository.GetAsync(orderId);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }
}
```

### Viewing Traces

In the Aspire Dashboard:
1. Go to **Traces** tab
2. See all traces with timing information
3. Click a trace to see the full span hierarchy
4. View tags, events, and exceptions

## Health Checks

### Default Health Checks

ServiceDefaults configures health check endpoints:

```csharp
public static IHostApplicationBuilder AddDefaultHealthChecks(
    this IHostApplicationBuilder builder)
{
    builder.Services.AddHealthChecks()
        // Add a default health check
        .AddCheck("self", () => HealthCheckResult.Healthy(), 
            tags: new[] { "live" });

    return builder;
}

public static WebApplication MapDefaultEndpoints(
    this WebApplication app)
{
    // Liveness probe - is the app running?
    app.MapHealthChecks("/health");

    // Readiness probe - is the app ready to serve traffic?
    app.MapHealthChecks("/alive", new HealthCheckOptions
    {
        Predicate = r => r.Tags.Contains("live")
    });

    return app;
}
```

### Adding Service-Specific Health Checks

In your services:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add database health check
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("catalogdb")!,
        name: "catalogdb",
        tags: new[] { "db", "postgres" });

// Add Redis health check
builder.Services.AddHealthChecks()
    .AddRedis(
        builder.Configuration.GetConnectionString("cache")!,
        name: "redis-cache",
        tags: new[] { "cache", "redis" });

var app = builder.Build();

app.MapDefaultEndpoints();  // Maps health check endpoints
```

### Custom Health Checks

```csharp
public class ExternalApiHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;

    public ExternalApiHealthCheck(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                "https://api.example.com/health", 
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("External API is healthy");
            }

            return HealthCheckResult.Degraded(
                $"External API returned {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "External API is unreachable", 
                ex);
        }
    }
}

// Register it
builder.Services.AddHealthChecks()
    .AddCheck<ExternalApiHealthCheck>(
        "external-api",
        tags: new[] { "external", "api" });
```

### Health Check UI in Dashboard

The Aspire Dashboard displays:
- Health status of all services
- Individual health check results
- Response times
- Historical health data

## HTTP Resilience

### Standard Resilience Handler

ServiceDefaults configures HTTP clients with resilience patterns:

```csharp
builder.Services.ConfigureHttpClientDefaults(http =>
{
    // Add standard resilience (retry, circuit breaker, timeout)
    http.AddStandardResilienceHandler();
    
    // Add service discovery
    http.AddServiceDiscovery();
});
```

### What's Included

The standard resilience handler includes:

1. **Retry**: Automatic retry on transient failures
2. **Circuit Breaker**: Stops calling failing services
3. **Timeout**: Per-request timeout
4. **Rate Limiter**: Prevents overwhelming services

```
Request → Timeout → Circuit Breaker → Rate Limiter → Retry → HTTP Call
```

### Resilience Configuration

Customize resilience in `appsettings.json`:

```json
{
  "HttpClientResilience": {
    "Retry": {
      "MaxRetryAttempts": 3,
      "BackoffType": "Exponential",
      "Delay": "00:00:02"
    },
    "CircuitBreaker": {
      "FailureThreshold": 0.5,
      "SamplingDuration": "00:00:30",
      "MinimumThroughput": 10,
      "BreakDuration": "00:00:30"
    },
    "Timeout": {
      "Timeout": "00:00:30"
    }
  }
}
```

### Custom Resilience Policies

For specific HTTP clients:

```csharp
builder.Services.AddHttpClient<ICatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalog");
})
.AddStandardResilienceHandler(options =>
{
    // Customize retry
    options.Retry.MaxRetryAttempts = 5;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
    
    // Customize circuit breaker
    options.CircuitBreaker.FailureThreshold = 0.3;
    options.CircuitBreaker.BreakDuration = TimeSpan.FromMinutes(1);
    
    // Customize timeout
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
});
```

### Resilience Telemetry

Resilience events are automatically traced:
- Retry attempts
- Circuit breaker state changes
- Timeout occurrences
- Rate limit rejections

View these in the Aspire Dashboard under Traces.

## Service Discovery

### How Service Discovery Works

When you reference a service in AppHost:

```csharp
// In AppHost
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog");
var cartApi = builder.AddProject<Projects.CartApi>("cart")
    .WithReference(catalogApi);
```

ServiceDefaults enables the Cart API to discover the Catalog API:

```csharp
// In Cart API
builder.Services.AddHttpClient<ICatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalog");  // Service name!
})
.AddServiceDiscovery();  // Resolves "catalog" to actual endpoint
```

### Service Discovery Flow

```
1. Cart API makes request to "http://catalog/products"
2. Service Discovery intercepts the request
3. Looks up "catalog" in service registry
4. Finds: http://localhost:5234 (actual URL)
5. Request is sent to http://localhost:5234/products
```

### Multiple Instances

Service discovery handles load balancing:

```csharp
// In AppHost - run 3 instances
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog")
    .WithReplicas(3);
```

Service discovery automatically distributes requests across instances:
- `catalog-0`: http://localhost:5234
- `catalog-1`: http://localhost:5235
- `catalog-2`: http://localhost:5236

### Configuration

Service discovery is configured via `ConfigureHttpClientDefaults`:

```csharp
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddServiceDiscovery();
});
```

This applies to **all** HTTP clients unless overridden.

### Named Endpoints

Reference specific endpoints:

```csharp
// In AppHost
var api = builder.AddProject<Projects.Api>("api")
    .WithHttpEndpoint(name: "public-api")
    .WithHttpEndpoint(name: "admin-api");

// In another service
builder.Services.AddHttpClient("public", client =>
{
    client.BaseAddress = new Uri("http://api/public-api");
});
```

## Customization

### Extending ServiceDefaults

Add your own extensions:

```csharp
public static class CustomExtensions
{
    public static IHostApplicationBuilder AddCustomDefaults(
        this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();  // Start with standard defaults
        
        // Add custom configuration
        builder.Services.AddSingleton<ICorrelationIdProvider, 
            CorrelationIdProvider>();
        
        // Add custom middleware
        builder.Services.AddScoped<CorrelationIdMiddleware>();
        
        return builder;
    }
    
    public static WebApplication UseCustomDefaults(
        this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }
}
```

Use in services:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddCustomDefaults();  // Use your custom defaults

var app = builder.Build();

app.UseCustomDefaults();
app.MapDefaultEndpoints();
```

### Environment-Specific Defaults

```csharp
public static IHostApplicationBuilder AddServiceDefaults(
    this IHostApplicationBuilder builder)
{
    builder.ConfigureOpenTelemetry();
    builder.AddDefaultHealthChecks();
    
    if (builder.Environment.IsDevelopment())
    {
        // Development-specific configuration
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    }
    else if (builder.Environment.IsProduction())
    {
        // Production-specific configuration
        builder.Services.Configure<OpenTelemetryOptions>(options =>
        {
            options.SamplingRatio = 0.1;  // Sample 10% in production
        });
    }
    
    return builder;
}
```

### Per-Service Customization

Override defaults in specific services:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Override HTTP client defaults for this service
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 10;  // More retries
    });
});
```

## Best Practices

### 1. Always Use ServiceDefaults

```csharp
// ✅ Good - every service uses defaults
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// ❌ Bad - missing service defaults
var builder = WebApplication.CreateBuilder(args);
// No AddServiceDefaults() call
```

### 2. Keep ServiceDefaults Consistent

```csharp
// ✅ Good - all services use same defaults
// Service A
builder.AddServiceDefaults();

// Service B
builder.AddServiceDefaults();

// ❌ Bad - inconsistent configuration
// Service A
builder.AddServiceDefaults();

// Service B
builder.AddOpenTelemetry();  // Manual configuration
```

### 3. Extend, Don't Replace

```csharp
// ✅ Good - extend existing defaults
public static IHostApplicationBuilder AddApiDefaults(
    this IHostApplicationBuilder builder)
{
    builder.AddServiceDefaults();  // Start with standard
    
    // Add API-specific configuration
    builder.Services.AddSwagger();
    return builder;
}

// ❌ Bad - reimplementing everything
public static IHostApplicationBuilder AddApiDefaults(
    this IHostApplicationBuilder builder)
{
    builder.Services.AddOpenTelemetry();  // Reimplementing
    builder.Services.AddHealthChecks();   // Reimplementing
    // ...
}
```

### 4. Document Custom Extensions

```csharp
/// <summary>
/// Adds company-standard service defaults including:
/// - OpenTelemetry with custom attributes
/// - Health checks
/// - HTTP resilience
/// - Correlation ID middleware
/// - Custom error handling
/// </summary>
public static IHostApplicationBuilder AddCompanyDefaults(
    this IHostApplicationBuilder builder)
{
    // Implementation...
}
```

### 5. Version ServiceDefaults

Treat ServiceDefaults as a versioned package:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <Version>1.2.0</Version>
    <Description>
      v1.2.0: Added custom correlation ID middleware
      v1.1.0: Enhanced OpenTelemetry configuration
      v1.0.0: Initial release
    </Description>
  </PropertyGroup>
</Project>
```

### 6. Test ServiceDefaults

Create integration tests:

```csharp
[Fact]
public async Task ServiceDefaults_ConfiguresHealthChecks()
{
    var builder = WebApplication.CreateBuilder();
    builder.AddServiceDefaults();
    
    var app = builder.Build();
    app.MapDefaultEndpoints();
    
    await app.StartAsync();
    
    var client = new HttpClient { BaseAddress = new Uri("http://localhost") };
    var response = await client.GetAsync("/health");
    
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

## Summary

In this lesson, you learned:

- ✅ The purpose and structure of the ServiceDefaults project
- ✅ How OpenTelemetry integrates for observability
- ✅ Logging, metrics, and tracing configuration
- ✅ Health check setup and customization
- ✅ HTTP resilience patterns (retry, circuit breaker, timeout)
- ✅ Service discovery configuration
- ✅ How to customize and extend ServiceDefaults
- ✅ Best practices for maintaining consistency

### Key Takeaways

1. **ServiceDefaults standardizes cross-cutting concerns** across all services in your application.

2. **OpenTelemetry provides comprehensive observability** with minimal configuration.

3. **One line of code** (`AddServiceDefaults()`) configures logging, metrics, tracing, health checks, and resilience.

4. **Resilience is built-in**, handling transient failures automatically.

5. **Service discovery** enables services to find each other by name.

6. **Consistency is key** - all services should use the same ServiceDefaults.

## Next Steps

Proceed to:
- **[Lesson 4: Aspire Components](./04_components.md)** - Explore the component ecosystem
- **[Exercise 3](./exercises/03_observability.md)** - Practice implementing observability

## Additional Resources

- [Service Defaults Documentation](https://learn.microsoft.com/dotnet/aspire/fundamentals/service-defaults)
- [OpenTelemetry in .NET](https://learn.microsoft.com/dotnet/core/diagnostics/observability-with-otel)
- [HTTP Resilience](https://learn.microsoft.com/dotnet/core/resilience/)

---

**Previous**: [← Lesson 2: AppHost Project](./02_app_host.md)  
**Next**: [Lesson 4: Aspire Components →](./04_components.md)
