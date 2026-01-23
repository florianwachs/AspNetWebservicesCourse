# Lesson 5: Service Discovery - Connecting Distributed Services

## Introduction

In a distributed application, services need to communicate with each other. Service discovery is the mechanism that allows services to find and connect to each other dynamically without hardcoding URLs or endpoints. .NET Aspire provides a sophisticated yet simple service discovery system that works seamlessly in development and can be adapted for various production environments.

In this lesson, we'll explore how service discovery works in Aspire, learn how to configure service-to-service communication, and understand best practices for building resilient distributed applications.

## Table of Contents

1. [Service Discovery Overview](#service-discovery-overview)
2. [How Aspire Service Discovery Works](#how-aspire-service-discovery-works)
3. [HTTP Service Communication](#http-service-communication)
4. [Named HTTP Clients](#named-http-clients)
5. [Service Endpoints and Configuration](#service-endpoints-and-configuration)
6. [Multiple Instances and Load Balancing](#multiple-instances-and-load-balancing)
7. [gRPC Service Discovery](#grpc-service-discovery)
8. [Testing Service Discovery](#testing-service-discovery)
9. [Common Patterns](#common-patterns)
10. [Best Practices](#best-practices)

## Service Discovery Overview

### The Service Discovery Problem

Consider a typical microservices architecture:

```
┌──────────┐         ┌──────────┐         ┌──────────┐
│ Frontend │────────▶│ Catalog  │────────▶│ Pricing  │
│   App    │         │   API    │         │  Service │
└──────────┘         └──────────┘         └──────────┘
                           │
                           ▼
                     ┌──────────┐
                     │ Inventory│
                     │  Service │
                     └──────────┘
```

**Problems without service discovery:**
- ❌ Hardcoded URLs break when services move or scale
- ❌ Port conflicts in local development
- ❌ Different URLs between development, staging, and production
- ❌ Difficult to test with service mocks
- ❌ Manual configuration is error-prone

**With service discovery:**
- ✅ Services find each other automatically
- ✅ Ports are dynamically assigned
- ✅ Works across all environments
- ✅ Easy to mock and test
- ✅ Configuration is centralized

### Aspire's Service Discovery Model

```
┌───────────────────────────────────────────────────────────┐
│                     Service Discovery                     │
├───────────────────────────────────────────────────────────┤
│                                                           │
│  1. Registration Phase (AppHost)                          │
│     ┌─────────────────────────────────────┐              │
│     │ var api = builder.AddProject("api");│              │
│     │ // Registers "api" as discoverable  │              │
│     └─────────────────────────────────────┘              │
│                                                           │
│  2. Reference Phase (AppHost)                             │
│     ┌──────────────────────────────────────┐             │
│     │ builder.AddProject("frontend")       │             │
│     │        .WithReference(api);          │             │
│     │ // Frontend can now discover "api"   │             │
│     └──────────────────────────────────────┘             │
│                                                           │
│  3. Resolution Phase (Runtime)                            │
│     ┌────────────────────────────────────────┐           │
│     │ var client = httpClientFactory        │           │
│     │     .CreateClient("api");              │           │
│     │ // Resolves to http://api:PORT         │           │
│     └────────────────────────────────────────┘           │
│                                                           │
└───────────────────────────────────────────────────────────┘
```

## How Aspire Service Discovery Works

### Configuration-Based Discovery

Aspire uses .NET's configuration system for service discovery. When you reference a service, Aspire injects configuration that maps service names to endpoints.

#### Under the Hood

```json
{
  "services": {
    "api": {
      "http": ["http://localhost:5001"],
      "https": ["https://localhost:7001"]
    },
    "catalogapi": {
      "http": ["http://localhost:5002"],
      "https": ["https://localhost:7002"]
    }
  }
}
```

This configuration is automatically generated and injected by the AppHost at runtime.

### Service Discovery Components

Aspire service discovery has three main parts:

1. **Aspire.Hosting.AppHost**: Registers services and their endpoints
2. **Microsoft.Extensions.ServiceDiscovery**: Client library for resolution
3. **Configuration injection**: Bridges the AppHost and client services

## HTTP Service Communication

### Basic HTTP Communication

The most common pattern for service-to-service communication.

#### AppHost Setup

```csharp
// Program.cs in AppHost
var builder = DistributedApplication.CreateBuilder(args);

// Register backend API
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalogapi");

// Register frontend with reference to API
var frontend = builder.AddProject<Projects.Frontend>("frontend")
                     .WithReference(catalogApi);

builder.Build().Run();
```

#### Frontend Service Setup

Install the service discovery client:

```bash
dotnet add package Microsoft.Extensions.ServiceDiscovery
```

Register service discovery:

```csharp
// Program.cs in Frontend
var builder = WebApplication.CreateBuilder(args);

// Add service discovery
builder.Services.AddServiceDiscovery();

// Configure HttpClient with service discovery
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddServiceDiscovery();
});

var app = builder.Build();
```

#### Using the Service

```csharp
// Services/CatalogService.cs
public class CatalogService(HttpClient httpClient)
{
    public async Task<List<Product>> GetProductsAsync()
    {
        // Service name resolves to actual endpoint
        var response = await httpClient.GetAsync("http://catalogapi/api/products");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<List<Product>>() 
            ?? [];
    }
}

// Register the service
builder.Services.AddHttpClient<CatalogService>();
```

### Service Name Resolution

Service discovery resolves service names using this pattern:

```csharp
// These all work:
"http://catalogapi"              // Scheme + service name
"https://catalogapi"             // HTTPS variant
"http://catalogapi/api/products" // With path
"http://catalogapi:8080"         // Explicit port (ignored, uses discovered)
```

The service discovery system:
1. Extracts the host name (`catalogapi`)
2. Looks up the endpoint in configuration
3. Replaces with the actual endpoint
4. Makes the request

## Named HTTP Clients

### Using Typed Clients

Typed HTTP clients provide better encapsulation and testability.

```csharp
// Configure in Program.cs
builder.Services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalogapi");
});

// Client implementation
public interface ICatalogClient
{
    Task<List<Product>> GetProductsAsync();
    Task<Product?> GetProductAsync(int id);
    Task<Product> CreateProductAsync(Product product);
}

public class CatalogClient(HttpClient httpClient) : ICatalogClient
{
    public async Task<List<Product>> GetProductsAsync()
    {
        return await httpClient.GetFromJsonAsync<List<Product>>("/api/products") 
            ?? [];
    }
    
    public async Task<Product?> GetProductAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<Product>($"/api/products/{id}");
    }
    
    public async Task<Product> CreateProductAsync(Product product)
    {
        var response = await httpClient.PostAsJsonAsync("/api/products", product);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>() 
            ?? throw new InvalidOperationException();
    }
}

// Usage in a controller or service
public class ProductsController(ICatalogClient catalogClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
        var products = await catalogClient.GetProductsAsync();
        return Ok(products);
    }
}
```

### Named Clients

For multiple backend services, use named clients:

```csharp
// Program.cs
builder.Services.AddHttpClient("catalog", client =>
{
    client.BaseAddress = new Uri("http://catalogapi");
});

builder.Services.AddHttpClient("inventory", client =>
{
    client.BaseAddress = new Uri("http://inventoryapi");
});

builder.Services.AddHttpClient("pricing", client =>
{
    client.BaseAddress = new Uri("http://pricingapi");
});

// Usage
public class ProductAggregator(IHttpClientFactory httpClientFactory)
{
    public async Task<ProductDetails> GetProductDetailsAsync(int productId)
    {
        // Get product from catalog
        var catalogClient = httpClientFactory.CreateClient("catalog");
        var product = await catalogClient
            .GetFromJsonAsync<Product>($"/api/products/{productId}");
        
        // Get inventory from inventory service
        var inventoryClient = httpClientFactory.CreateClient("inventory");
        var inventory = await inventoryClient
            .GetFromJsonAsync<Inventory>($"/api/inventory/{productId}");
        
        // Get pricing from pricing service
        var pricingClient = httpClientFactory.CreateClient("pricing");
        var pricing = await pricingClient
            .GetFromJsonAsync<Price>($"/api/prices/{productId}");
        
        return new ProductDetails
        {
            Product = product!,
            Inventory = inventory!,
            Pricing = pricing!
        };
    }
}
```

## Service Endpoints and Configuration

### Multiple Endpoints

Services can expose multiple endpoints (HTTP, HTTPS, gRPC, etc.).

```csharp
// AppHost
var api = builder.AddProject<Projects.Api>("api")
                .WithHttpEndpoint(port: 5000, name: "http")
                .WithHttpsEndpoint(port: 5001, name: "https");

// Access specific endpoints
var frontend = builder.AddProject<Projects.Frontend>("frontend")
                     .WithReference(api, "http"); // Use HTTP endpoint
```

### Endpoint Selection

```csharp
// In consuming service
builder.Services.AddHttpClient<ApiClient>(client =>
{
    // Default - uses first available endpoint
    client.BaseAddress = new Uri("http://api");
    
    // Or explicitly use HTTPS
    client.BaseAddress = new Uri("https://api");
});
```

### External Services

You can register external services for service discovery:

```csharp
// AppHost - register external service
builder.AddProject<Projects.Frontend>("frontend")
       .WithEnvironment("services__external-api__http__0", 
                       "https://api.external.com");

// Frontend can now discover "external-api"
var client = httpClientFactory.CreateClient();
var response = await client.GetAsync("http://external-api/endpoint");
```

## Multiple Instances and Load Balancing

### Scaling Services

Aspire allows you to run multiple instances of a service:

```csharp
// AppHost
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalogapi")
                       .WithReplicas(3); // Run 3 instances

var frontend = builder.AddProject<Projects.Frontend>("frontend")
                     .WithReference(catalogApi);
```

### How Load Balancing Works

When multiple instances are available:

```
┌──────────┐
│ Frontend │
└────┬─────┘
     │ HTTP request to "catalogapi"
     │
     ▼
┌────────────────┐
│ Service        │
│ Discovery      │
│ Resolver       │
└────┬───────────┘
     │ Returns multiple endpoints:
     │ • http://localhost:5001
     │ • http://localhost:5002
     │ • http://localhost:5003
     │
     ▼
┌────────────────┐
│ HttpClient     │
│ Load Balancer  │  ◄── Round-robin by default
└────┬───────────┘
     │
     ├────▶ Instance 1
     ├────▶ Instance 2
     └────▶ Instance 3
```

### Load Balancing Configuration

```csharp
builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalogapi");
})
.AddServiceDiscovery() // Enables service discovery
.AddStandardResilienceHandler(); // Adds retry, circuit breaker, timeout
```

### Custom Load Balancing

```csharp
builder.Services.AddHttpClient<CatalogClient>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri("http://catalogapi");
    })
    .AddServiceDiscovery()
    .AddResilienceHandler("catalog-pipeline", builder =>
    {
        // Custom resilience strategy
        builder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(100),
            BackoffType = DelayBackoffType.Exponential
        });
        
        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            BreakDuration = TimeSpan.FromSeconds(30),
            SamplingDuration = TimeSpan.FromSeconds(10),
            FailureRatio = 0.5,
            MinimumThroughput = 10
        });
        
        builder.AddTimeout(TimeSpan.FromSeconds(10));
    });
```

## gRPC Service Discovery

### gRPC Communication

Aspire supports gRPC service discovery out of the box.

#### AppHost Setup

```csharp
var grpcService = builder.AddProject<Projects.GrpcService>("grpcservice");

var frontend = builder.AddProject<Projects.Frontend>("frontend")
                     .WithReference(grpcService);
```

#### Frontend Setup

```bash
dotnet add package Grpc.Net.ClientFactory
dotnet add package Microsoft.Extensions.ServiceDiscovery
```

```csharp
// Program.cs
builder.Services.AddGrpcClient<Greeter.GreeterClient>(options =>
{
    options.Address = new Uri("http://grpcservice");
})
.AddServiceDiscovery();

// Usage
public class GreetingService(Greeter.GreeterClient grpcClient)
{
    public async Task<string> GetGreetingAsync(string name)
    {
        var request = new HelloRequest { Name = name };
        var response = await grpcClient.SayHelloAsync(request);
        return response.Message;
    }
}
```

## Testing Service Discovery

### Unit Testing with Mock Services

```csharp
public class CatalogServiceTests
{
    [Fact]
    public async Task GetProductsAsync_ReturnsProducts()
    {
        // Arrange
        var handler = new MockHttpMessageHandler();
        handler.SetupResponse("/api/products", new List<Product>
        {
            new() { Id = 1, Name = "Product 1" },
            new() { Id = 2, Name = "Product 2" }
        });
        
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        
        var service = new CatalogService(httpClient);
        
        // Act
        var products = await service.GetProductsAsync();
        
        // Assert
        Assert.Equal(2, products.Count);
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, object> _responses = new();
    
    public void SetupResponse<T>(string path, T response)
    {
        _responses[path] = response!;
    }
    
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.PathAndQuery ?? "";
        
        if (_responses.TryGetValue(path, out var response))
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(response)
            });
        }
        
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        });
    }
}
```

### Integration Testing with Real Services

```csharp
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task GetProducts_ReturnsSuccessfully()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/products");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var products = await response.Content
            .ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
    }
}
```

### Testing with Aspire AppHost

```csharp
[Collection("Integration")]
public class ServiceDiscoveryTests
{
    [Fact]
    public async Task Services_CanCommunicate()
    {
        // Create AppHost for testing
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AppHost>();
        
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();
        
        // Get the frontend client
        var httpClient = app.CreateHttpClient("frontend");
        
        // Frontend should be able to call catalog API
        var response = await httpClient.GetAsync("/products");
        
        response.EnsureSuccessStatusCode();
    }
}
```

## Common Patterns

### Backend for Frontend (BFF)

```csharp
// AppHost
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalogapi");
var orderApi = builder.AddProject<Projects.OrderApi>("orderapi");
var inventoryApi = builder.AddProject<Projects.InventoryApi>("inventoryapi");

// BFF aggregates multiple backend services
var bff = builder.AddProject<Projects.BFF>("bff")
                .WithReference(catalogApi)
                .WithReference(orderApi)
                .WithReference(inventoryApi);

var frontend = builder.AddProject<Projects.Frontend>("frontend")
                     .WithReference(bff);

// Frontend only talks to BFF, BFF talks to backends
```

### API Gateway Pattern

```csharp
// AppHost with API Gateway
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalogapi");
var orderApi = builder.AddProject<Projects.OrderApi>("orderapi");

var gateway = builder.AddProject<Projects.Gateway>("gateway")
                    .WithReference(catalogApi)
                    .WithReference(orderApi);

var frontend = builder.AddProject<Projects.Frontend>("frontend")
                     .WithReference(gateway);
```

### Service Mesh (Development)

```csharp
// All services can discover each other
var catalog = builder.AddProject<Projects.CatalogApi>("catalogapi");
var inventory = builder.AddProject<Projects.InventoryApi>("inventoryapi");
var pricing = builder.AddProject<Projects.PricingApi>("pricingapi");
var orders = builder.AddProject<Projects.OrderApi>("orderapi");

// Create a mesh where services reference each other
catalog.WithReference(inventory)
       .WithReference(pricing);

orders.WithReference(catalog)
      .WithReference(inventory);
```

### Strangler Fig Pattern

Gradually replace legacy services:

```csharp
// Both old and new services running
var legacyApi = builder.AddProject<Projects.LegacyApi>("legacyapi");
var newApi = builder.AddProject<Projects.NewApi>("newapi");

var frontend = builder.AddProject<Projects.Frontend>("frontend")
                     .WithReference(legacyApi)
                     .WithReference(newApi);

// Frontend gradually migrates endpoints from legacy to new
```

## Best Practices

### 1. Use Typed HTTP Clients

```csharp
// Good - typed client
public interface ICatalogClient
{
    Task<List<Product>> GetProductsAsync();
}

builder.Services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalogapi");
});

// Avoid - creating HttpClient manually
var client = new HttpClient(); // ❌ Bypasses service discovery
```

### 2. Always Add Resilience

```csharp
builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalogapi");
})
.AddStandardResilienceHandler(); // ✅ Adds retries, circuit breaker, timeout
```

### 3. Use Service Names Consistently

```csharp
// AppHost
var api = builder.AddProject<Projects.Api>("catalogapi");

// Service - use SAME name
builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalogapi"); // ✅ Same name
});
```

### 4. Handle Service Unavailability

```csharp
public class CatalogService(ICatalogClient catalogClient, ILogger<CatalogService> logger)
{
    public async Task<List<Product>> GetProductsAsync()
    {
        try
        {
            return await catalogClient.GetProductsAsync();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to fetch products from catalog API");
            // Return cached data or empty list
            return [];
        }
    }
}
```

### 5. Configure Timeouts Appropriately

```csharp
builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalogapi");
    client.Timeout = TimeSpan.FromSeconds(30); // ✅ Explicit timeout
});
```

### 6. Use Health Checks

```csharp
// In backend API
builder.Services.AddHealthChecks();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// AppHost waits for health checks before marking service as ready
```

### 7. Leverage Configuration for Environments

```json
{
  "services": {
    "catalogapi": {
      "http": ["http://localhost:5000"]
    }
  }
}
```

In production, override with actual endpoints:

```json
{
  "services": {
    "catalogapi": {
      "https": ["https://catalogapi.production.com"]
    }
  }
}
```

### 8. Monitor Service Communication

Use OpenTelemetry tracing to monitor service calls:

```csharp
// Automatically configured by Aspire.ServiceDefaults
// View traces in Aspire Dashboard
```

### 9. Avoid Circular Dependencies

```csharp
// ❌ Avoid
var serviceA = builder.AddProject<Projects.ServiceA>("servicea");
var serviceB = builder.AddProject<Projects.ServiceB>("serviceb");

serviceA.WithReference(serviceB);
serviceB.WithReference(serviceA); // ❌ Circular dependency

// ✅ Better - use messaging or event-driven architecture
var messaging = builder.AddRabbitMQ("messaging");

serviceA.WithReference(messaging);
serviceB.WithReference(messaging);
```

### 10. Test Service Discovery

```csharp
[Fact]
public async Task ServiceDiscovery_ResolvesCorrectEndpoint()
{
    var services = new ServiceCollection();
    services.AddServiceDiscovery();
    
    // Configure test endpoint
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["services:testapi:http:0"] = "http://localhost:5555"
        })
        .Build();
    
    services.AddSingleton<IConfiguration>(configuration);
    
    var provider = services.BuildServiceProvider();
    var resolver = provider.GetRequiredService<ServiceEndpointResolver>();
    
    // Resolve endpoint
    var endpoint = await resolver.GetEndpointAsync("http://testapi");
    
    Assert.Equal("http://localhost:5555", endpoint.ToString());
}
```

## Summary

Service discovery in .NET Aspire provides:

✅ **Automatic service resolution** without hardcoded URLs  
✅ **Configuration-based** discovery that works across environments  
✅ **Load balancing** across multiple instances  
✅ **Resilience patterns** with retries and circuit breakers  
✅ **Testability** with mock and integration testing support  

Key takeaways:
- Service discovery uses .NET configuration for endpoint resolution
- Named and typed HTTP clients provide clean abstractions
- Multiple instances enable load balancing and high availability
- Resilience handlers protect against transient failures
- Service discovery works in development and adapts to production

In the next lesson, we'll explore the Aspire Dashboard and how to use it for monitoring and debugging distributed applications.
