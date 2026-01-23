# Lesson 2: AppHost Project - The Orchestration Layer

## Introduction

The AppHost project is the heart of .NET Aspire's orchestration capabilities. It's where you define your application's topology, configure services, manage dependencies, and control how everything connects. Think of it as the "conductor" of your distributed application orchestra.

In this lesson, we'll explore the AppHost in depth, learning how to configure services, manage resources, handle secrets, and orchestrate complex distributed applications.

## Table of Contents

1. [AppHost Overview](#apphost-overview)
2. [Creating an AppHost Project](#creating-an-apphost-project)
3. [Service Registration](#service-registration)
4. [Resource Management](#resource-management)
5. [Service References and Dependencies](#service-references-and-dependencies)
6. [Configuration and Secrets](#configuration-and-secrets)
7. [Health Checks and Readiness](#health-checks-and-readiness)
8. [Advanced Orchestration](#advanced-orchestration)
9. [Best Practices](#best-practices)

## AppHost Overview

### What is the AppHost?

The AppHost is a special .NET project that:
- Defines the structure of your distributed application
- Orchestrates service startup and shutdown
- Manages dependencies between services
- Configures service discovery
- Injects configuration into services
- Provides the entry point for running your entire application stack

### AppHost Responsibilities

```
┌──────────────────────────────────────────────────────────────┐
│                     AppHost Project                          │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌────────────────────────────────────────────────────┐     │
│  │  1. Service & Resource Discovery                   │     │
│  │     • Register all services and resources          │     │
│  │     • Define names and identifiers                 │     │
│  └────────────────────────────────────────────────────┘     │
│                                                              │
│  ┌────────────────────────────────────────────────────┐     │
│  │  2. Dependency Management                          │     │
│  │     • Define service dependencies                  │     │
│  │     • Control startup order                        │     │
│  │     • Wait for health checks                       │     │
│  └────────────────────────────────────────────────────┘     │
│                                                              │
│  ┌────────────────────────────────────────────────────┐     │
│  │  3. Configuration Injection                        │     │
│  │     • Connection strings                           │     │
│  │     • Service endpoints                            │     │
│  │     • Environment variables                        │     │
│  └────────────────────────────────────────────────────┘     │
│                                                              │
│  ┌────────────────────────────────────────────────────┐     │
│  │  4. Development Environment                        │     │
│  │     • Launch Aspire Dashboard                      │     │
│  │     • Enable hot reload                            │     │
│  │     • Manage lifecycle                             │     │
│  └────────────────────────────────────────────────────┘     │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

### AppHost Project Structure

A typical AppHost project contains:

```
MyApp.AppHost/
├── Program.cs                    # Main orchestration logic
├── MyApp.AppHost.csproj         # Project file
├── appsettings.json             # AppHost configuration
├── appsettings.Development.json # Development overrides
├── Properties/
│   └── launchSettings.json      # Launch configuration
└── aspire-manifest.json         # Generated deployment manifest
```

## Creating an AppHost Project

### Using Templates

The easiest way to create an AppHost is using templates:

```bash
# Create with starter template (includes sample services)
dotnet new aspire-starter -n MyApp

# Create empty Aspire solution
dotnet new aspire -n MyApp

# Add AppHost to existing solution
dotnet new aspire-apphost -n MyApp.AppHost
```

### Manual Creation

You can also create an AppHost manually:

```bash
# Create the project
dotnet new web -n MyApp.AppHost
cd MyApp.AppHost

# Add Aspire hosting package
dotnet add package Aspire.Hosting
```

**MyApp.AppHost.csproj**:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsAspireHost>true</IsAspireHost>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting" Version="10.0.0" />
  </ItemGroup>

  <ItemGroup>
    <!-- Reference your service projects -->
    <ProjectReference Include="..\MyApp.ApiService\MyApp.ApiService.csproj" />
  </ItemGroup>

</Project>
```

**Program.cs**:
```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Register services and resources here

builder.Build().Run();
```

## Service Registration

### Registering .NET Projects

The most common operation is registering your .NET service projects:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Register a single project
var apiService = builder.AddProject<Projects.CatalogApi>("catalog-api");

// Register multiple projects
var orderApi = builder.AddProject<Projects.OrderApi>("order-api");
var cartApi = builder.AddProject<Projects.CartApi>("cart-api");
var webFrontend = builder.AddProject<Projects.WebApp>("web");

builder.Build().Run();
```

#### The `Projects` Namespace

When you reference a project in your AppHost, Aspire generates a strongly-typed reference:

```csharp
// This is generated automatically
namespace Projects
{
    public class CatalogApi { }
    public class OrderApi { }
    public class CartApi { }
    public class WebApp { }
}

// Use it like this
var api = builder.AddProject<Projects.CatalogApi>("catalog-api");
```

This provides:
- ✅ Compile-time checking
- ✅ IntelliSense support
- ✅ Refactoring support

### Service Configuration

You can configure various aspects of services:

```csharp
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog-api")
    .WithHttpEndpoint(port: 5001, name: "api")      // Expose HTTP endpoint
    .WithHttpsEndpoint(port: 7001, name: "api-https") // Expose HTTPS endpoint
    .WithReplicas(3)                                 // Run 3 instances
    .WithEnvironment("LOG_LEVEL", "Debug");          // Set environment variable
```

### Registering Containers

You can also orchestrate Docker containers:

```csharp
// Register a generic container
var mailServer = builder.AddContainer("mailhog", "mailhog/mailhog")
    .WithHttpEndpoint(port: 8025, targetPort: 8025, name: "ui")
    .WithEndpoint(port: 1025, targetPort: 1025, name: "smtp");

// Register with environment variables
var customApp = builder.AddContainer("myapp", "mycompany/myapp")
    .WithEnvironment("CUSTOM_VAR", "value")
    .WithEnvironment("API_KEY", () => builder.Configuration["ApiKey"]!);
```

### Registering Executables

Run native executables as part of your app:

```csharp
// Run a Node.js application
var nodeApp = builder.AddExecutable("node-app", "node", ".",
    args: ["server.js"])
    .WithHttpEndpoint(port: 3000);

// Run a Python script
var pythonWorker = builder.AddExecutable("worker", "python", ".",
    args: ["worker.py"]);
```

## Resource Management

Aspire provides built-in support for common infrastructure resources.

### Databases

#### PostgreSQL

```csharp
// Add PostgreSQL server
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "17")              // Specify version
    .WithEnvironment("POSTGRES_PASSWORD", "secret")
    .WithPgAdmin();                           // Add pgAdmin UI

// Add databases to the server
var catalogDb = postgres.AddDatabase("catalogdb");
var orderDb = postgres.AddDatabase("orderdb");

// Use in services
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog")
    .WithReference(catalogDb);
```

#### SQL Server

```csharp
var sqlServer = builder.AddSqlServer("sql")
    .WithImage("mcr.microsoft.com/mssql/server", "2022-latest");

var inventoryDb = sqlServer.AddDatabase("inventorydb");

var inventoryApi = builder.AddProject<Projects.InventoryApi>("inventory")
    .WithReference(inventoryDb);
```

#### MongoDB

```csharp
var mongodb = builder.AddMongoDB("mongodb")
    .WithMongoExpress();  // Add Mongo Express UI

var productsDb = mongodb.AddDatabase("productsdb");

var productApi = builder.AddProject<Projects.ProductApi>("products")
    .WithReference(productsDb);
```

### Caching

#### Redis

```csharp
var redis = builder.AddRedis("cache")
    .WithImage("redis", "7")
    .WithRedisCommander();  // Add Redis Commander UI

// Use in services
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(redis);
```

In your service, the connection string is automatically available:

```csharp
// In your API service
builder.AddRedisClient("cache");

// Use it
app.MapGet("/cached", async (IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    return await db.StringGetAsync("key");
});
```

#### Valkey (Redis alternative)

```csharp
var valkey = builder.AddValkey("cache")
    .WithImage("valkey/valkey", "8");

var api = builder.AddProject<Projects.Api>("api")
    .WithReference(valkey);
```

### Messaging

#### RabbitMQ

```csharp
var messaging = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin()  // Enable management UI
    .WithImage("rabbitmq", "4-management");

var orderProcessor = builder.AddProject<Projects.OrderProcessor>("order-processor")
    .WithReference(messaging);
```

In your service:

```csharp
builder.AddRabbitMQClient("messaging");

// Use with MassTransit or RabbitMQ.Client
```

#### Azure Service Bus

```csharp
var serviceBus = builder.AddAzureServiceBus("messaging");

// In production, this uses Azure Service Bus
// In development, can use emulator or actual Azure resource
```

#### Apache Kafka

```csharp
var kafka = builder.AddKafka("kafka")
    .WithKafkaUI();  // Add Kafka UI for management

var eventProcessor = builder.AddProject<Projects.EventProcessor>("events")
    .WithReference(kafka);
```

### Storage

#### Azure Blob Storage

```csharp
// For development, uses Azurite emulator
var storage = builder.AddAzureStorage("storage");
var blobs = storage.AddBlobs("blobs");

var uploadApi = builder.AddProject<Projects.UploadApi>("upload")
    .WithReference(blobs);
```

In your service:

```csharp
builder.AddAzureBlobClient("blobs");

// Use it
app.MapPost("/upload", async (BlobServiceClient blobClient, IFormFile file) =>
{
    var containerClient = blobClient.GetBlobContainerClient("uploads");
    await containerClient.UploadBlobAsync(file.FileName, file.OpenReadStream());
});
```

## Service References and Dependencies

### Understanding References

When you add a reference from one service to a resource or another service, Aspire:

1. **Injects configuration**: Connection strings, endpoints, etc.
2. **Establishes dependencies**: Ensures proper startup order
3. **Enables service discovery**: Services can find each other by name

### Basic References

```csharp
var postgres = builder.AddPostgres("postgres");
var db = postgres.AddDatabase("mydb");

var cache = builder.AddRedis("cache");

// API depends on database and cache
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(db)
    .WithReference(cache);
```

### Service-to-Service References

```csharp
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog");

// Cart API depends on Catalog API
var cartApi = builder.AddProject<Projects.CartApi>("cart")
    .WithReference(catalogApi);  // Can now call catalog API

// Frontend depends on both
var frontend = builder.AddProject<Projects.Web>("web")
    .WithReference(catalogApi)
    .WithReference(cartApi);
```

In the Cart API, you can now call Catalog API:

```csharp
// In CartApi/Program.cs
builder.Services.AddHttpClient<ICatalogClient>(client =>
{
    // "catalog" is the name from AppHost
    client.BaseAddress = new Uri("http://catalog");
});
```

### Reference with Custom Configuration

```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(db)
    .WithReference(cache, connectionName: "redis-cache")  // Custom name
    .WithReference(catalogApi, env: "CATALOG_API_URL");   // Custom env var
```

### Optional References

```csharp
var api = builder.AddProject<Projects.Api>("api");

// Conditionally add references
if (builder.Environment.IsDevelopment())
{
    var localDb = builder.AddPostgres("postgres").AddDatabase("db");
    api.WithReference(localDb);
}
else
{
    // In production, use existing database
    var prodDb = builder.AddConnectionString("db");
    api.WithReference(prodDb);
}
```

## Configuration and Secrets

### Environment Variables

Add environment variables to services:

```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithEnvironment("LOG_LEVEL", "Debug")
    .WithEnvironment("FEATURE_FLAG_X", "true")
    .WithEnvironment("MAX_RETRIES", "3");
```

### Using Configuration

Access AppHost configuration:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// From appsettings.json
var apiKey = builder.Configuration["ExternalApi:ApiKey"];

var api = builder.AddProject<Projects.Api>("api")
    .WithEnvironment("EXTERNAL_API_KEY", apiKey!);
```

### User Secrets

For development secrets:

```bash
# Initialize user secrets for AppHost
cd MyApp.AppHost
dotnet user-secrets init

# Add secrets
dotnet user-secrets set "Database:Password" "super-secret-password"
dotnet user-secrets set "ApiKeys:ExternalService" "sk-1234567890"
```

Use in AppHost:

```csharp
var dbPassword = builder.Configuration["Database:Password"];
var apiKey = builder.Configuration["ApiKeys:ExternalService"];

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_PASSWORD", dbPassword!);

var api = builder.AddProject<Projects.Api>("api")
    .WithEnvironment("EXTERNAL_API_KEY", apiKey!);
```

### Azure Key Vault

For production secrets:

```csharp
// In AppHost Program.cs
var builder = DistributedApplication.CreateBuilder(args);

// Add Key Vault configuration (in production)
if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

var api = builder.AddProject<Projects.Api>("api")
    .WithEnvironment("DB_PASSWORD", () => builder.Configuration["DbPassword"]!);
```

### Connection Strings

Add existing connection strings:

```csharp
// Reference an existing database
var existingDb = builder.AddConnectionString("production-db");

var api = builder.AddProject<Projects.Api>("api")
    .WithReference(existingDb);
```

The connection string should be in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "production-db": "Host=prod.database.com;Database=mydb;Username=user;Password=***"
  }
}
```

## Health Checks and Readiness

### Built-in Health Checks

Aspire components automatically include health checks:

```csharp
var postgres = builder.AddPostgres("postgres");
var db = postgres.AddDatabase("mydb");  // Includes health check

var redis = builder.AddRedis("cache");  // Includes health check

// Services wait for dependencies to be healthy before starting
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(db)
    .WithReference(redis);
```

### Service Health Checks

In your services, add health checks:

```csharp
// In your API service
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();  // Adds default health checks

// Add custom health checks
builder.Services.AddHealthChecks()
    .AddCheck("custom", () => HealthCheckResult.Healthy("All good!"));

var app = builder.Build();

app.MapDefaultEndpoints();  // Maps /health and /alive endpoints
```

### Waiting for Dependencies

Aspire automatically waits for dependencies:

```csharp
// This ensures startup order:
// 1. PostgreSQL starts and becomes healthy
// 2. Redis starts and becomes healthy
// 3. CatalogApi starts (waits for PostgreSQL)
// 4. CartApi starts (waits for Redis and CatalogApi)
// 5. Frontend starts (waits for both APIs)

var postgres = builder.AddPostgres("postgres");
var catalogDb = postgres.AddDatabase("catalogdb");
var redis = builder.AddRedis("cache");

var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog")
    .WithReference(catalogDb);

var cartApi = builder.AddProject<Projects.CartApi>("cart")
    .WithReference(redis)
    .WithReference(catalogApi);

var frontend = builder.AddProject<Projects.Web>("frontend")
    .WithReference(catalogApi)
    .WithReference(cartApi);
```

### Custom Readiness Checks

Add custom wait conditions:

```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(db)
    .WithHealthCheck("api-ready");  // Wait for specific health check

// Or wait for specific endpoint
var api = builder.AddProject<Projects.Api>("api")
    .WaitFor(db)  // Explicit wait
    .WaitFor(cache);
```

## Advanced Orchestration

### Replicas and Scaling

Run multiple instances of a service:

```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(db)
    .WithReplicas(3);  // Run 3 instances

// Each instance gets a unique port and name:
// api-0, api-1, api-2
```

### Port Configuration

Control port assignments:

```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithHttpEndpoint(port: 5000, name: "http")
    .WithHttpsEndpoint(port: 5001, name: "https");

// Or use dynamic ports (recommended for development)
var api = builder.AddProject<Projects.Api>("api")
    .WithHttpEndpoint(name: "http");  // Aspire assigns a random port
```

### Volume Mounts

Persist data with volumes:

```csharp
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("postgres-data");  // Named volume

var redis = builder.AddRedis("cache")
    .WithDataVolume();  // Anonymous volume

// Or bind mount
var api = builder.AddContainer("api", "myimage")
    .WithBindMount("./data", "/app/data");
```

### Networking

Configure container networking:

```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithHttpEndpoint(port: 5000, targetPort: 8080)  // Map ports
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080");
```

### Lifecycle Hooks

Execute code at specific points:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.Services.AddLifecycleHook<MyLifecycleHook>();

class MyLifecycleHook : IDistributedApplicationLifecycleHook
{
    public Task BeforeStartAsync(DistributedApplicationModel appModel, 
        CancellationToken cancellationToken = default)
    {
        // Called before any resources start
        Console.WriteLine("Starting application...");
        return Task.CompletedTask;
    }

    public Task AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel,
        CancellationToken cancellationToken = default)
    {
        // Called after ports are assigned but before services start
        return Task.CompletedTask;
    }
}
```

### Conditional Resources

Add resources based on conditions:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres;

if (builder.Environment.IsDevelopment())
{
    // Use containerized PostgreSQL in development
    postgres = builder.AddPostgres("postgres")
        .WithPgAdmin();
}
else
{
    // Use managed PostgreSQL in production
    postgres = builder.AddPostgres("postgres")
        .PublishAsAzurePostgresFlexibleServer();
}

var db = postgres.AddDatabase("mydb");
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(db);
```

### Resource Groups

Organize related resources:

```csharp
// Catalog service cluster
var catalogPostgres = builder.AddPostgres("catalog-db");
var catalogDb = catalogPostgres.AddDatabase("catalogdb");
var catalogCache = builder.AddRedis("catalog-cache");
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog")
    .WithReference(catalogDb)
    .WithReference(catalogCache);

// Order service cluster
var orderPostgres = builder.AddPostgres("order-db");
var orderDb = orderPostgres.AddDatabase("orderdb");
var orderMessaging = builder.AddRabbitMQ("order-messaging");
var orderApi = builder.AddProject<Projects.OrderApi>("orders")
    .WithReference(orderDb)
    .WithReference(orderMessaging);

// Frontend
var frontend = builder.AddProject<Projects.Web>("frontend")
    .WithReference(catalogApi)
    .WithReference(orderApi);
```

## Best Practices

### 1. Use Descriptive Names

```csharp
// ❌ Bad - unclear names
var db1 = builder.AddPostgres("db1");
var api1 = builder.AddProject<Projects.Api>("api1");

// ✅ Good - descriptive names
var catalogDb = builder.AddPostgres("catalog-postgres");
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog-api");
```

### 2. Group Related Resources

```csharp
// ✅ Good - grouped by feature
// Infrastructure
var postgres = builder.AddPostgres("postgres");
var redis = builder.AddRedis("cache");
var rabbitmq = builder.AddRabbitMQ("messaging");

// Databases
var catalogDb = postgres.AddDatabase("catalogdb");
var orderDb = postgres.AddDatabase("orderdb");

// Services
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog")
    .WithReference(catalogDb)
    .WithReference(redis);

var orderApi = builder.AddProject<Projects.OrderApi>("orders")
    .WithReference(orderDb)
    .WithReference(rabbitmq);
```

### 3. Extract Configuration Methods

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var (catalogApi, orderApi) = ConfigureBackendServices(builder);
var frontend = ConfigureFrontend(builder, catalogApi, orderApi);

builder.Build().Run();

static (IResourceBuilder<ProjectResource> catalog, IResourceBuilder<ProjectResource> orders) 
    ConfigureBackendServices(IDistributedApplicationBuilder builder)
{
    var postgres = builder.AddPostgres("postgres");
    var redis = builder.AddRedis("cache");
    
    var catalogDb = postgres.AddDatabase("catalogdb");
    var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog")
        .WithReference(catalogDb)
        .WithReference(redis);
    
    var orderDb = postgres.AddDatabase("orderdb");
    var orderApi = builder.AddProject<Projects.OrderApi>("orders")
        .WithReference(orderDb);
    
    return (catalogApi, orderApi);
}

static IResourceBuilder<ProjectResource> ConfigureFrontend(
    IDistributedApplicationBuilder builder,
    IResourceBuilder<ProjectResource> catalogApi,
    IResourceBuilder<ProjectResource> orderApi)
{
    return builder.AddProject<Projects.Web>("frontend")
        .WithReference(catalogApi)
        .WithReference(orderApi);
}
```

### 4. Use Environment-Specific Configuration

```csharp
var builder = DistributedApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    // Development: use containers with admin UIs
    var postgres = builder.AddPostgres("postgres")
        .WithPgAdmin();
    var redis = builder.AddRedis("cache")
        .WithRedisCommander();
}
else
{
    // Production: use managed services
    var postgres = builder.AddPostgres("postgres")
        .PublishAsAzurePostgresFlexibleServer();
    var redis = builder.AddRedis("cache")
        .PublishAsAzureRedis();
}
```

### 5. Document Complex Configurations

```csharp
/// <summary>
/// Configures the e-commerce application topology.
/// 
/// Architecture:
/// - Catalog API: Product catalog (PostgreSQL + Redis)
/// - Cart API: Shopping cart (Redis + Catalog API)
/// - Order API: Order processing (PostgreSQL + RabbitMQ)
/// - Frontend: Blazor web app (depends on all APIs)
/// </summary>
var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure layer
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);  // Persist data
    
// ... rest of configuration
```

### 6. Use Persistent Volumes for Development

```csharp
// Persist database data between runs
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("my-app-postgres-data")
    .WithLifetime(ContainerLifetime.Persistent);

// Persist Redis data
var redis = builder.AddRedis("cache")
    .WithDataVolume("my-app-redis-data")
    .WithLifetime(ContainerLifetime.Persistent);
```

### 7. Version Your Infrastructure

```csharp
// Specify versions for reproducibility
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "17.2");

var redis = builder.AddRedis("cache")
    .WithImage("redis", "7.2");

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithImage("rabbitmq", "4.0-management");
```

## Summary

In this lesson, you learned:

- ✅ The purpose and responsibilities of the AppHost project
- ✅ How to register .NET projects, containers, and executables
- ✅ How to manage infrastructure resources (databases, caches, messaging)
- ✅ How to configure service references and dependencies
- ✅ How to handle configuration and secrets
- ✅ How health checks and readiness work
- ✅ Advanced orchestration techniques
- ✅ Best practices for AppHost organization

### Key Takeaways

1. **AppHost is the orchestration layer** that defines your application topology.

2. **Service references** automatically configure dependencies, service discovery, and startup order.

3. **Aspire components** provide ready-to-use integrations for common infrastructure.

4. **Configuration injection** happens automatically based on references.

5. **Environment-specific configuration** lets you use containers in dev and managed services in production.

6. **Organization matters** - use descriptive names and group related resources.

## Next Steps

Proceed to:
- **[Lesson 3: Service Defaults](./03_service_defaults.md)** - Learn about shared service configuration
- **[Exercise 2](./exercises/02_apphost_configuration.md)** - Practice AppHost configuration

## Additional Resources

- [Aspire Hosting Overview](https://learn.microsoft.com/dotnet/aspire/fundamentals/app-host-overview)
- [Service Discovery](https://learn.microsoft.com/dotnet/aspire/service-discovery/overview)
- [Resource Configuration](https://learn.microsoft.com/dotnet/aspire/fundamentals/resource-configuration)

---

**Previous**: [← Lesson 1: Aspire Fundamentals](./01_aspire_fundamentals.md)  
**Next**: [Lesson 3: Service Defaults →](./03_service_defaults.md)
