# Lesson 4: Aspire Components - Building Blocks for Cloud-Native Apps

## Introduction

.NET Aspire Components are pre-built, production-ready integrations for common cloud services and infrastructure dependencies. They provide standardized, opinionated configurations that follow best practices for observability, resilience, and health monitoring. Components dramatically reduce the boilerplate code needed to connect your applications to databases, caches, message brokers, and storage services.

In this lesson, we'll explore the rich ecosystem of Aspire components, learn how to use them effectively, and understand how to create custom components when needed.

## Table of Contents

1. [Component Overview](#component-overview)
2. [Database Components](#database-components)
3. [Caching Components](#caching-components)
4. [Messaging Components](#messaging-components)
5. [Storage Components](#storage-components)
6. [Using Components in Services](#using-components-in-services)
7. [Component Configuration](#component-configuration)
8. [Creating Custom Components](#creating-custom-components)
9. [Best Practices](#best-practices)

## Component Overview

### What are Aspire Components?

Aspire Components are NuGet packages that provide:
- **Client libraries**: Configured and ready-to-use clients for external services
- **Health checks**: Built-in health monitoring for dependencies
- **Telemetry**: Automatic logging, metrics, and distributed tracing
- **Resilience**: Retry policies, circuit breakers, and timeout handling
- **Configuration**: Convention-based configuration with sensible defaults

### Component Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│                     Aspire Component                             │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────────┐  │
│  │  Client Setup  │  │  Health Checks │  │   Telemetry      │  │
│  │                │  │                │  │                  │  │
│  │  • Connection  │  │  • Liveness    │  │  • Logging       │  │
│  │  • Auth        │  │  • Readiness   │  │  • Metrics       │  │
│  │  • Pooling     │  │  • Startup     │  │  • Tracing       │  │
│  └────────────────┘  └────────────────┘  └──────────────────┘  │
│                                                                  │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────────┐  │
│  │  Resilience    │  │  Configuration │  │   DI Integration │  │
│  │                │  │                │  │                  │  │
│  │  • Retries     │  │  • Defaults    │  │  • Services      │  │
│  │  • Timeouts    │  │  • Overrides   │  │  • Options       │  │
│  │  • Fallbacks   │  │  • Validation  │  │  • Factories     │  │
│  └────────────────┘  └────────────────┘  └──────────────────┘  │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

### Component Naming Convention

Aspire components follow a consistent naming pattern:

- **Hosting packages**: `Aspire.Hosting.[Technology]` (used in AppHost)
- **Client packages**: `Aspire.[Technology]` (used in your services)

Example:
```csharp
// In AppHost - hosting package
builder.AddPostgres("postgres")
       .AddDatabase("catalogdb");

// In your service - client package
builder.AddNpgsqlDataSource("catalogdb");
```

## Database Components

### PostgreSQL Component

PostgreSQL is a powerful open-source relational database. Aspire provides comprehensive support through Npgsql.

#### AppHost Configuration

```csharp
// Program.cs in AppHost
var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL server
var postgres = builder.AddPostgres("postgres")
                     .WithPgAdmin()  // Optional: Add pgAdmin for management
                     .WithDataVolume(); // Persist data between runs

// Add databases
var catalogDb = postgres.AddDatabase("catalogdb");
var identityDb = postgres.AddDatabase("identitydb");

// Use in services
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalogapi")
                       .WithReference(catalogDb);

builder.Build().Run();
```

#### Service Configuration

Install the client component:

```bash
dotnet add package Aspire.Npgsql
```

Register in your service:

```csharp
// Program.cs in your service
var builder = WebApplication.CreateBuilder(args);

// Add PostgreSQL data source
builder.AddNpgsqlDataSource("catalogdb");

var app = builder.Build();
```

#### Using the Database

```csharp
// With Npgsql directly
public class CatalogRepository(NpgsqlDataSource dataSource)
{
    public async Task<List<Product>> GetProductsAsync()
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, name, price FROM products";
        
        var products = new List<Product>();
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2)
            });
        }
        
        return products;
    }
}

// With Entity Framework Core
builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) 
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}
```

### SQL Server Component

SQL Server is Microsoft's enterprise relational database.

#### AppHost Configuration

```csharp
var sqlServer = builder.AddSqlServer("sqlserver")
                      .WithDataVolume();

var orderDb = sqlServer.AddDatabase("orderdb");

builder.AddProject<Projects.OrderApi>("orderapi")
       .WithReference(orderDb);
```

#### Service Configuration

```bash
dotnet add package Aspire.Microsoft.Data.SqlClient
```

```csharp
// For ADO.NET
builder.AddSqlServerClient("orderdb");

// For Entity Framework Core
builder.AddSqlServerDbContext<OrderDbContext>("orderdb");
```

### MongoDB Component

MongoDB is a popular NoSQL document database.

#### AppHost Configuration

```csharp
var mongodb = builder.AddMongoDB("mongodb")
                    .WithDataVolume();

var productsDb = mongodb.AddDatabase("productsdb");

builder.AddProject<Projects.ProductApi>("productapi")
       .WithReference(productsDb);
```

#### Service Configuration

```bash
dotnet add package Aspire.MongoDB.Driver
```

```csharp
builder.AddMongoDBClient("mongodb");

// Usage
public class ProductRepository(IMongoClient mongoClient)
{
    private readonly IMongoDatabase _database = 
        mongoClient.GetDatabase("productsdb");
    
    public async Task<List<Product>> GetProductsAsync()
    {
        var collection = _database.GetCollection<Product>("products");
        return await collection.Find(_ => true).ToListAsync();
    }
}
```

### MySQL Component

MySQL is another popular open-source relational database.

#### AppHost Configuration

```csharp
var mysql = builder.AddMySql("mysql")
                  .WithDataVolume();

var inventoryDb = mysql.AddDatabase("inventorydb");

builder.AddProject<Projects.InventoryApi>("inventoryapi")
       .WithReference(inventoryDb);
```

#### Service Configuration

```bash
dotnet add package Aspire.MySqlConnector
```

```csharp
builder.AddMySqlDataSource("inventorydb");
```

## Caching Components

### Redis Component

Redis is an in-memory data structure store used as a cache, message broker, and database.

#### AppHost Configuration

```csharp
var redis = builder.AddRedis("redis")
                  .WithDataVolume(); // Persist to disk

var cache = redis.WithReference(redis);

builder.AddProject<Projects.Api>("api")
       .WithReference(cache);
```

#### Service Configuration

```bash
dotnet add package Aspire.StackExchange.Redis
```

```csharp
builder.AddRedisClient("redis");

// Usage with IConnectionMultiplexer
public class CacheService(IConnectionMultiplexer redis)
{
    public async Task<string?> GetAsync(string key)
    {
        var db = redis.GetDatabase();
        return await db.StringGetAsync(key);
    }
    
    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(key, value, expiry);
    }
}
```

#### Distributed Caching

Redis can be used as an IDistributedCache:

```bash
dotnet add package Aspire.StackExchange.Redis.DistributedCaching
```

```csharp
builder.AddRedisDistributedCache("redis");

// Usage
public class ProductService(IDistributedCache cache)
{
    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";
        var cached = await cache.GetStringAsync(cacheKey);
        
        if (cached != null)
        {
            return JsonSerializer.Deserialize<Product>(cached);
        }
        
        var product = await FetchFromDatabaseAsync(id);
        
        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        
        return product;
    }
}
```

#### Output Caching

Redis can also be used for output caching:

```bash
dotnet add package Aspire.StackExchange.Redis.OutputCaching
```

```csharp
builder.AddRedisOutputCache("redis");

var app = builder.Build();
app.UseOutputCache();

app.MapGet("/products", async (ProductService service) =>
{
    return await service.GetAllProductsAsync();
})
.CacheOutput(policy => policy.Expire(TimeSpan.FromMinutes(5)));
```

### Valkey Component

Valkey is a Redis fork that's fully compatible with Redis protocols.

```csharp
// AppHost
var valkey = builder.AddValkey("cache");

// Service
builder.AddValkeyClient("cache");
```

## Messaging Components

### RabbitMQ Component

RabbitMQ is a widely-used message broker.

#### AppHost Configuration

```csharp
var messaging = builder.AddRabbitMQ("messaging")
                      .WithManagementPlugin() // Adds web UI on port 15672
                      .WithDataVolume();

builder.AddProject<Projects.OrderProcessor>("orderprocessor")
       .WithReference(messaging);
```

#### Service Configuration

```bash
dotnet add package Aspire.RabbitMQ.Client
```

```csharp
builder.AddRabbitMQClient("messaging");

// Publishing messages
public class OrderService(IConnection rabbitConnection)
{
    public async Task PublishOrderAsync(Order order)
    {
        using var channel = rabbitConnection.CreateModel();
        
        channel.QueueDeclare(
            queue: "orders",
            durable: true,
            exclusive: false,
            autoDelete: false);
        
        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(order));
        
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        
        channel.BasicPublish(
            exchange: "",
            routingKey: "orders",
            basicProperties: properties,
            body: body);
    }
}

// Consuming messages
public class OrderProcessorService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly ILogger<OrderProcessorService> _logger;
    
    public OrderProcessorService(
        IConnection connection,
        ILogger<OrderProcessorService> logger)
    {
        _connection = connection;
        _logger = logger;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _connection.CreateModel();
        
        channel.QueueDeclare(
            queue: "orders",
            durable: true,
            exclusive: false,
            autoDelete: false);
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var order = JsonSerializer.Deserialize<Order>(message);
            
            _logger.LogInformation("Processing order {OrderId}", order?.Id);
            
            // Process the order
            await ProcessOrderAsync(order!);
            
            channel.BasicAck(ea.DeliveryTag, multiple: false);
        };
        
        channel.BasicConsume(queue: "orders", autoAck: false, consumer: consumer);
        
        return Task.CompletedTask;
    }
    
    private async Task ProcessOrderAsync(Order order)
    {
        // Processing logic
        await Task.Delay(100);
    }
}
```

### Azure Service Bus Component

Azure Service Bus is a fully managed enterprise message broker.

#### AppHost Configuration

```csharp
// For local development (uses emulator)
var serviceBus = builder.AddAzureServiceBus("messaging");

// For Azure (uses real Service Bus)
var serviceBus = builder.AddAzureServiceBus("messaging")
                       .RunAsEmulator(); // Only for local dev
```

#### Service Configuration

```bash
dotnet add package Aspire.Azure.Messaging.ServiceBus
```

```csharp
builder.AddAzureServiceBusClient("messaging");

// Usage
public class NotificationService(ServiceBusClient client)
{
    public async Task SendNotificationAsync(Notification notification)
    {
        var sender = client.CreateSender("notifications");
        
        var message = new ServiceBusMessage(
            JsonSerializer.Serialize(notification));
        
        await sender.SendMessageAsync(message);
    }
}
```

### Kafka Component

Apache Kafka is a distributed event streaming platform.

#### AppHost Configuration

```csharp
var kafka = builder.AddKafka("kafka")
                  .WithDataVolume();

builder.AddProject<Projects.EventProcessor>("eventprocessor")
       .WithReference(kafka);
```

#### Service Configuration

```bash
dotnet add package Aspire.Confluent.Kafka
```

```csharp
builder.AddKafkaProducer<string, string>("kafka");
builder.AddKafkaConsumer<string, string>("kafka", consumerBuilder =>
{
    consumerBuilder.Config.GroupId = "event-processor";
    consumerBuilder.Config.AutoOffsetReset = AutoOffsetReset.Earliest;
});

// Producer usage
public class EventPublisher(IProducer<string, string> producer)
{
    public async Task PublishEventAsync(string topic, string key, string value)
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = value
        };
        
        await producer.ProduceAsync(topic, message);
    }
}

// Consumer usage
public class EventConsumerService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    
    public EventConsumerService(IConsumer<string, string> consumer)
    {
        _consumer = consumer;
        _consumer.Subscribe("events");
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(stoppingToken);
            
            // Process the event
            await ProcessEventAsync(result.Message.Key, result.Message.Value);
            
            _consumer.Commit(result);
        }
    }
}
```

## Storage Components

### Azure Blob Storage Component

Azure Blob Storage provides object storage for unstructured data.

#### AppHost Configuration

```csharp
// For local development (uses Azurite emulator)
var storage = builder.AddAzureStorage("storage")
                    .RunAsEmulator();

var blobs = storage.AddBlobs("blobs");

builder.AddProject<Projects.FileApi>("fileapi")
       .WithReference(blobs);
```

#### Service Configuration

```bash
dotnet add package Aspire.Azure.Storage.Blobs
```

```csharp
builder.AddAzureBlobClient("blobs");

// Usage
public class FileStorageService(BlobServiceClient blobClient)
{
    public async Task<string> UploadFileAsync(
        string containerName,
        string fileName,
        Stream content)
    {
        var container = blobClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync();
        
        var blob = container.GetBlobClient(fileName);
        await blob.UploadAsync(content, overwrite: true);
        
        return blob.Uri.ToString();
    }
    
    public async Task<Stream> DownloadFileAsync(
        string containerName,
        string fileName)
    {
        var container = blobClient.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(fileName);
        
        var response = await blob.DownloadStreamingAsync();
        return response.Value.Content;
    }
}
```

### AWS S3 Component (Community)

While not officially part of Aspire, S3 integration follows similar patterns.

```csharp
// Custom component pattern for S3
builder.Services.AddAWSService<IAmazonS3>();
```

## Using Components in Services

### Basic Usage Pattern

The standard pattern for using Aspire components:

1. **Install the component package**
   ```bash
   dotnet add package Aspire.[Technology]
   ```

2. **Register in Program.cs**
   ```csharp
   builder.Add[Technology]Client("connectionName");
   ```

3. **Inject and use**
   ```csharp
   public class MyService(ClientType client)
   {
       // Use the client
   }
   ```

### Multiple Instances

You can register multiple instances of the same component:

```csharp
// AppHost
var primaryDb = postgres.AddDatabase("primary");
var reportsDb = postgres.AddDatabase("reports");

// Service
builder.AddNpgsqlDataSource("primary", "PrimaryConnection");
builder.AddNpgsqlDataSource("reports", "ReportsConnection");

// Usage with keyed services
public class DataService(
    [FromKeyedServices("PrimaryConnection")] NpgsqlDataSource primary,
    [FromKeyedServices("ReportsConnection")] NpgsqlDataSource reports)
{
    // Use both connections
}
```

## Component Configuration

### Configuration Sources

Components get configuration from multiple sources (in order of precedence):

1. Explicit code configuration
2. appsettings.json
3. Environment variables
4. User secrets
5. AppHost-injected configuration

### appsettings.json Configuration

```json
{
  "Aspire": {
    "Npgsql": {
      "DisableHealthChecks": false,
      "DisableTracing": false,
      "DisableMetrics": false
    },
    "StackExchange": {
      "Redis": {
        "DisableHealthChecks": false,
        "DisableTracing": false
      }
    }
  }
}
```

### Programmatic Configuration

```csharp
builder.AddNpgsqlDataSource("catalogdb", settings =>
{
    settings.DisableHealthChecks = false;
    settings.DisableTracing = false;
    settings.DisableMetrics = false;
});

builder.AddRedisClient("cache", settings =>
{
    settings.DisableHealthChecks = false;
    settings.DisableTracing = false;
});
```

### Connection String Customization

```csharp
// In AppHost
var postgres = builder.AddPostgres("postgres");
var db = postgres.AddDatabase("mydb");

// Override connection string parameters
builder.AddProject<Projects.Api>("api")
       .WithReference(db)
       .WithEnvironment("ConnectionStrings__mydb", 
           ctx => $"{ctx.GetConnectionString(db)};Include Error Detail=true");
```

## Creating Custom Components

### When to Create Custom Components

Create custom components when:
- You have an internal service that's reused across projects
- You want to standardize configuration for a third-party library
- You need to add health checks and telemetry to existing integrations

### Custom Component Structure

```csharp
// MyCustomComponent/AspireHostingExtensions.cs
namespace Aspire.Hosting;

public static class MyServiceHostingExtensions
{
    public static IResourceBuilder<MyServiceResource> AddMyService(
        this IDistributedApplicationBuilder builder,
        string name,
        int? port = null)
    {
        var resource = new MyServiceResource(name);
        
        return builder.AddResource(resource)
                     .WithEndpoint(
                         port: port,
                         targetPort: 8080,
                         name: MyServiceResource.PrimaryEndpointName)
                     .WithImage("mycompany/myservice")
                     .WithImageTag("latest");
    }
}

public class MyServiceResource(string name) : ContainerResource(name)
{
    internal const string PrimaryEndpointName = "http";
}

// MyCustomComponent/AspireClientExtensions.cs
namespace Microsoft.Extensions.Hosting;

public static class MyServiceClientExtensions
{
    public static void AddMyServiceClient(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<MyServiceClientSettings>? configureSettings = null)
    {
        var settings = new MyServiceClientSettings();
        configureSettings?.Invoke(settings);
        
        // Register the client
        builder.Services.AddHttpClient<MyServiceClient>((sp, client) =>
        {
            var connectionString = builder.Configuration
                .GetConnectionString(connectionName)
                ?? throw new InvalidOperationException(
                    $"Connection string '{connectionName}' not found.");
            
            client.BaseAddress = new Uri(connectionString);
        });
        
        // Add health checks
        if (!settings.DisableHealthChecks)
        {
            builder.Services.AddHealthChecks()
                .AddCheck<MyServiceHealthCheck>(
                    $"myservice_{connectionName}",
                    tags: ["ready"]);
        }
        
        // Add telemetry
        if (!settings.DisableTracing)
        {
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.AddHttpClientInstrumentation();
                });
        }
    }
}

public class MyServiceClientSettings
{
    public bool DisableHealthChecks { get; set; }
    public bool DisableTracing { get; set; }
    public bool DisableMetrics { get; set; }
}

public class MyServiceClient(HttpClient httpClient)
{
    public async Task<string> GetDataAsync()
    {
        return await httpClient.GetStringAsync("/api/data");
    }
}

public class MyServiceHealthCheck(MyServiceClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await client.GetDataAsync();
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Failed to connect to MyService",
                ex);
        }
    }
}
```

### Using Your Custom Component

```csharp
// In AppHost
var myService = builder.AddMyService("myservice");

builder.AddProject<Projects.Api>("api")
       .WithReference(myService);

// In your service
builder.AddMyServiceClient("myservice");

// Inject and use
public class SomeController(MyServiceClient client) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var data = await client.GetDataAsync();
        return Ok(data);
    }
}
```

## Best Practices

### 1. Use Health Checks

Always keep health checks enabled unless you have a specific reason to disable them:

```csharp
// Good - health checks enabled (default)
builder.AddNpgsqlDataSource("catalogdb");

// Avoid - unless necessary
builder.AddNpgsqlDataSource("catalogdb", settings =>
{
    settings.DisableHealthChecks = true;
});
```

### 2. Leverage Telemetry

Keep tracing and metrics enabled for observability:

```csharp
// Components provide automatic telemetry
// View traces in the Aspire Dashboard
```

### 3. Use Connection Names Consistently

Maintain consistent naming between AppHost and services:

```csharp
// AppHost
var db = postgres.AddDatabase("catalogdb");  // Name: "catalogdb"

// Service - use same name
builder.AddNpgsqlDataSource("catalogdb");    // Same name
```

### 4. Externalize Configuration

Use configuration for environment-specific settings:

```json
{
  "ConnectionStrings": {
    "cache": "localhost:6379"
  },
  "Aspire": {
    "StackExchange": {
      "Redis": {
        "DisableHealthChecks": false
      }
    }
  }
}
```

### 5. Use Data Volumes for Persistence

Always use data volumes for databases in development:

```csharp
var postgres = builder.AddPostgres("postgres")
                     .WithDataVolume(); // Persists data

var redis = builder.AddRedis("cache")
                  .WithDataVolume(); // Persists cache
```

### 6. Scope Component References

Only reference components in services that need them:

```csharp
// Good - only catalog API needs catalog DB
builder.AddProject<Projects.CatalogApi>("catalogapi")
       .WithReference(catalogDb);

// Avoid - frontend doesn't need direct DB access
builder.AddProject<Projects.Frontend>("frontend")
       .WithReference(catalogDb);  // ❌ Backend should mediate
```

### 7. Use Appropriate Abstractions

Choose the right level of abstraction:

```csharp
// Direct ADO.NET for simple queries
builder.AddNpgsqlDataSource("db");

// Entity Framework for complex domain models
builder.AddNpgsqlDbContext<CatalogDbContext>("db");

// Dapper for performance-critical queries (manual setup)
```

### 8. Test with Real Components

Use real instances of components in integration tests:

```csharp
[Collection("Integration")]
public class CatalogApiTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public async Task<Product> CreateProductAsync(Product product)
    {
        // Tests run against real PostgreSQL from Aspire
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/products", product);
        return await response.Content.ReadFromJsonAsync<Product>();
    }
}
```

### 9. Monitor Resource Usage

Keep an eye on resource consumption in the dashboard:

```csharp
// Check metrics for:
// - Database connection pool usage
// - Cache hit rates
// - Message queue depths
// - Storage throughput
```

### 10. Plan for Production

Remember that Aspire components are client libraries - they work in production:

```csharp
// Development - Aspire orchestrates PostgreSQL container
var postgres = builder.AddPostgres("postgres");

// Production - connects to Azure Database for PostgreSQL
// Connection string from configuration or Azure App Configuration
```

## Summary

Aspire Components provide:

✅ **Standardized integrations** for databases, caches, messaging, and storage  
✅ **Built-in observability** with health checks, logging, metrics, and tracing  
✅ **Resilience patterns** with retries, timeouts, and circuit breakers  
✅ **Reduced boilerplate** through convention-based configuration  
✅ **Production-ready** clients that work in any environment  

Key takeaways:
- Components are NuGet packages that simplify service integrations
- They follow a consistent pattern: AddX in AppHost, AddXClient in services
- Built-in telemetry and health checks improve observability
- Custom components can be created for internal or third-party services
- Components are just client libraries - they work anywhere .NET runs

In the next lesson, we'll explore service discovery and how services communicate with each other in Aspire applications.
