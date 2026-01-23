# Exercise 2: Adding Aspire Components

## 🎯 Objectives

In this exercise, you will:

- Add PostgreSQL database with Entity Framework Core
- Implement Redis caching with cache-aside pattern
- Add RabbitMQ messaging for async communication
- Configure and manage connection strings
- Test integrated components
- Observe component behavior in the dashboard

## ⏱️ Estimated Time

60-90 minutes

## 📋 Prerequisites

- Completed Exercise 1
- Docker Desktop running
- Basic understanding of Entity Framework Core
- Familiarity with caching concepts

## 🗄️ Part 1: Add PostgreSQL Database

### Step 1: Create a New Aspire Application

```bash
cd aspire-labs
dotnet new aspire-starter -n AspireComponents
cd AspireComponents
```

### Step 2: Add PostgreSQL Component

Add the Aspire PostgreSQL NuGet package to the AppHost:

```bash
cd AspireComponents.AppHost
dotnet add package Aspire.Hosting.PostgreSQL
```

### Step 3: Configure PostgreSQL in AppHost

Open `AspireComponents.AppHost/Program.cs` and modify:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL server and database
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin(); // Optional: Adds pgAdmin UI

var catalogDb = postgres.AddDatabase("catalogdb");

// Add API service with database reference
var apiService = builder.AddProject<Projects.AspireComponents_ApiService>("apiservice")
    .WithReference(catalogDb);

builder.AddProject<Projects.AspireComponents_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
```

### Step 4: Add Entity Framework Core to API Service

```bash
cd ../AspireComponents.ApiService
dotnet add package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### Step 5: Create Data Models

Create `AspireComponents.ApiService/Models/Product.cs`:

```csharp
namespace AspireComponents.ApiService.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### Step 6: Create DbContext

Create `AspireComponents.ApiService/Data/CatalogDbContext.cs`:

```csharp
using AspireComponents.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireComponents.ApiService.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Name);
        });

        // Seed data
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 1299.99m,
                StockQuantity = 50
            },
            new Product
            {
                Id = 2,
                Name = "Mouse",
                Description = "Wireless mouse",
                Price = 29.99m,
                StockQuantity = 200
            },
            new Product
            {
                Id = 3,
                Name = "Keyboard",
                Description = "Mechanical keyboard",
                Price = 89.99m,
                StockQuantity = 100
            }
        );
    }
}
```

### Step 7: Configure Database in API Service

Open `AspireComponents.ApiService/Program.cs`:

```csharp
using AspireComponents.ApiService.Data;
using AspireComponents.ApiService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add PostgreSQL with Entity Framework Core
builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    db.Database.EnsureCreated(); // Or use: await db.Database.MigrateAsync();
}

// API Endpoints
app.MapGet("/products", async (CatalogDbContext db) =>
{
    return await db.Products.ToListAsync();
});

app.MapGet("/products/{id}", async (int id, CatalogDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.MapPost("/products", async (Product product, CatalogDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id}", async (int id, Product inputProduct, CatalogDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    product.Name = inputProduct.Name;
    product.Description = inputProduct.Description;
    product.Price = inputProduct.Price;
    product.StockQuantity = inputProduct.StockQuantity;

    await db.SaveChangesAsync();
    return Results.Ok(product);
});

app.MapDelete("/products/{id}", async (int id, CatalogDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDefaultEndpoints();

app.Run();
```

### Step 8: Run and Test PostgreSQL Integration

```bash
cd ../AspireComponents.AppHost
dotnet run
```

**In the Dashboard:**
1. Note the new "postgres" resource
2. Click on pgAdmin link if you added it
3. View logs from PostgreSQL container

**Test the API:**

```bash
# Get all products
curl https://localhost:7XXX/products

# Get single product
curl https://localhost:7XXX/products/1

# Create product
curl -X POST https://localhost:7XXX/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Monitor",
    "description": "4K Monitor",
    "price": 399.99,
    "stockQuantity": 75
  }'
```

### ✅ Verification Point 1

**Check:**
- [ ] PostgreSQL container is running in dashboard
- [ ] Database is created and seeded
- [ ] Can GET all products
- [ ] Can GET single product
- [ ] Can POST new product
- [ ] Traces show database queries

## 🗂️ Part 2: Add Redis Caching

### Step 1: Add Redis Component to AppHost

```bash
cd AspireComponents.AppHost
dotnet add package Aspire.Hosting.Redis
```

Update `Program.cs`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var catalogDb = postgres.AddDatabase("catalogdb");

// Add Redis
var redis = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.AspireComponents_ApiService>("apiservice")
    .WithReference(catalogDb)
    .WithReference(redis); // Add Redis reference

builder.AddProject<Projects.AspireComponents_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
```

### Step 2: Add Redis Package to API Service

```bash
cd ../AspireComponents.ApiService
dotnet add package Aspire.StackExchange.Redis
dotnet add package Aspire.StackExchange.Redis.OutputCaching
```

### Step 3: Implement Cache-Aside Pattern

Create `AspireComponents.ApiService/Services/ProductService.cs`:

```csharp
using System.Text.Json;
using AspireComponents.ApiService.Data;
using AspireComponents.ApiService.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace AspireComponents.ApiService.Services;

public class ProductService
{
    private readonly CatalogDbContext _db;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<ProductService> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public ProductService(
        CatalogDbContext db,
        IConnectionMultiplexer redis,
        ILogger<ProductService> logger)
    {
        _db = db;
        _redis = redis;
        _logger = logger;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        const string cacheKey = "products:all";
        var cache = _redis.GetDatabase();

        // Try to get from cache
        var cachedData = await cache.StringGetAsync(cacheKey);
        if (cachedData.HasValue)
        {
            _logger.LogInformation("Products retrieved from cache");
            return JsonSerializer.Deserialize<List<Product>>(cachedData!)!;
        }

        // Get from database
        _logger.LogInformation("Products retrieved from database");
        var products = await _db.Products.ToListAsync();

        // Store in cache
        var serialized = JsonSerializer.Serialize(products);
        await cache.StringSetAsync(cacheKey, serialized, _cacheExpiration);

        return products;
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var cacheKey = $"products:{id}";
        var cache = _redis.GetDatabase();

        // Try cache first
        var cachedData = await cache.StringGetAsync(cacheKey);
        if (cachedData.HasValue)
        {
            _logger.LogInformation("Product {ProductId} retrieved from cache", id);
            return JsonSerializer.Deserialize<Product>(cachedData!);
        }

        // Get from database
        _logger.LogInformation("Product {ProductId} retrieved from database", id);
        var product = await _db.Products.FindAsync(id);

        if (product != null)
        {
            var serialized = JsonSerializer.Serialize(product);
            await cache.StringSetAsync(cacheKey, serialized, _cacheExpiration);
        }

        return product;
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // Invalidate cache
        await InvalidateCacheAsync();

        return product;
    }

    public async Task<Product?> UpdateProductAsync(int id, Product inputProduct)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return null;

        product.Name = inputProduct.Name;
        product.Description = inputProduct.Description;
        product.Price = inputProduct.Price;
        product.StockQuantity = inputProduct.StockQuantity;

        await _db.SaveChangesAsync();

        // Invalidate cache
        await InvalidateCacheAsync();
        await InvalidateCacheAsync($"products:{id}");

        return product;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        // Invalidate cache
        await InvalidateCacheAsync();
        await InvalidateCacheAsync($"products:{id}");

        return true;
    }

    private async Task InvalidateCacheAsync(string? key = null)
    {
        var cache = _redis.GetDatabase();
        if (key != null)
        {
            await cache.KeyDeleteAsync(key);
            _logger.LogInformation("Cache invalidated: {CacheKey}", key);
        }
        else
        {
            await cache.KeyDeleteAsync("products:all");
            _logger.LogInformation("Cache invalidated: products:all");
        }
    }
}
```

### Step 4: Update Program.cs to Use ProductService

```csharp
using AspireComponents.ApiService.Data;
using AspireComponents.ApiService.Models;
using AspireComponents.ApiService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add database
builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");

// Add Redis
builder.AddRedisClient("cache");

// Register services
builder.Services.AddScoped<ProductService>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    db.Database.EnsureCreated();
}

// Updated endpoints using ProductService
app.MapGet("/products", async (ProductService productService) =>
{
    return await productService.GetAllProductsAsync();
});

app.MapGet("/products/{id}", async (int id, ProductService productService) =>
{
    var product = await productService.GetProductByIdAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.MapPost("/products", async (Product product, ProductService productService) =>
{
    var created = await productService.CreateProductAsync(product);
    return Results.Created($"/products/{created.Id}", created);
});

app.MapPut("/products/{id}", async (int id, Product inputProduct, ProductService productService) =>
{
    var updated = await productService.UpdateProductAsync(id, inputProduct);
    return updated is not null ? Results.Ok(updated) : Results.NotFound();
});

app.MapDelete("/products/{id}", async (int id, ProductService productService) =>
{
    var deleted = await productService.DeleteProductAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

// Cache stats endpoint
app.MapGet("/cache/stats", async (IConnectionMultiplexer redis) =>
{
    var cache = redis.GetDatabase();
    var endpoints = redis.GetEndPoints();
    var server = redis.GetServer(endpoints.First());
    
    var keys = server.Keys(pattern: "products:*").ToList();
    
    return new
    {
        CachedKeys = keys.Count,
        Keys = keys.Select(k => k.ToString())
    };
});

app.MapDefaultEndpoints();

app.Run();
```

### Step 5: Test Caching

```bash
# Run the application
cd ../AspireComponents.AppHost
dotnet run
```

**Test cache behavior:**

```bash
# First request (from database)
curl https://localhost:7XXX/products

# Second request (from cache - should be faster)
curl https://localhost:7XXX/products

# Check cache stats
curl https://localhost:7XXX/cache/stats
```

**In the Dashboard:**
1. Check Redis logs - should show commands
2. View traces - cache hits should be faster
3. Compare trace durations between cache hit and miss

### ✅ Verification Point 2

**Check:**
- [ ] Redis container running
- [ ] First request slower (database)
- [ ] Second request faster (cache)
- [ ] `/cache/stats` shows cached keys
- [ ] Cache invalidated after POST/PUT/DELETE
- [ ] Logs indicate cache hits/misses

## 📨 Part 3: Add RabbitMQ Messaging

### Step 1: Add RabbitMQ Component

```bash
cd AspireComponents.AppHost
dotnet add package Aspire.Hosting.RabbitMQ
```

Update `Program.cs`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var catalogDb = postgres.AddDatabase("catalogdb");

var redis = builder.AddRedis("cache");

// Add RabbitMQ
var messaging = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin(); // Adds RabbitMQ Management UI

var apiService = builder.AddProject<Projects.AspireComponents_ApiService>("apiservice")
    .WithReference(catalogDb)
    .WithReference(redis)
    .WithReference(messaging); // Add messaging reference

builder.AddProject<Projects.AspireComponents_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
```

### Step 2: Add RabbitMQ Package to API Service

```bash
cd ../AspireComponents.ApiService
dotnet add package Aspire.RabbitMQ.Client
```

### Step 3: Create Message Models

Create `AspireComponents.ApiService/Messages/ProductCreatedMessage.cs`:

```csharp
namespace AspireComponents.ApiService.Messages;

public record ProductCreatedMessage(
    int ProductId,
    string ProductName,
    decimal Price,
    DateTime CreatedAt
);

public record ProductUpdatedMessage(
    int ProductId,
    string ProductName,
    decimal Price,
    DateTime UpdatedAt
);

public record ProductDeletedMessage(
    int ProductId,
    DateTime DeletedAt
);
```

### Step 4: Create Message Publisher

Create `AspireComponents.ApiService/Services/MessagePublisher.cs`:

```csharp
using System.Text;
using System.Text.Json;
using AspireComponents.ApiService.Messages;
using RabbitMQ.Client;

namespace AspireComponents.ApiService.Services;

public class MessagePublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<MessagePublisher> _logger;
    private const string ExchangeName = "product-events";

    public MessagePublisher(IConnection connection, ILogger<MessagePublisher> logger)
    {
        _connection = connection;
        _logger = logger;
        
        // Declare exchange
        using var channel = _connection.CreateModel();
        channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
    }

    public void PublishProductCreated(ProductCreatedMessage message)
    {
        PublishMessage("product.created", message);
    }

    public void PublishProductUpdated(ProductUpdatedMessage message)
    {
        PublishMessage("product.updated", message);
    }

    public void PublishProductDeleted(ProductDeletedMessage message)
    {
        PublishMessage("product.deleted", message);
    }

    private void PublishMessage<T>(string routingKey, T message)
    {
        using var channel = _connection.CreateModel();
        
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: routingKey,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("Published message to {RoutingKey}: {Message}", routingKey, json);
    }
}
```

### Step 5: Update ProductService to Publish Messages

Modify `ProductService.cs`:

```csharp
using AspireComponents.ApiService.Messages;

public class ProductService
{
    private readonly CatalogDbContext _db;
    private readonly IConnectionMultiplexer _redis;
    private readonly MessagePublisher _messagePublisher;
    private readonly ILogger<ProductService> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public ProductService(
        CatalogDbContext db,
        IConnectionMultiplexer redis,
        MessagePublisher messagePublisher,
        ILogger<ProductService> logger)
    {
        _db = db;
        _redis = redis;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // Publish event
        _messagePublisher.PublishProductCreated(new ProductCreatedMessage(
            product.Id,
            product.Name,
            product.Price,
            product.CreatedAt
        ));

        await InvalidateCacheAsync();

        return product;
    }

    public async Task<Product?> UpdateProductAsync(int id, Product inputProduct)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return null;

        product.Name = inputProduct.Name;
        product.Description = inputProduct.Description;
        product.Price = inputProduct.Price;
        product.StockQuantity = inputProduct.StockQuantity;

        await _db.SaveChangesAsync();

        // Publish event
        _messagePublisher.PublishProductUpdated(new ProductUpdatedMessage(
            product.Id,
            product.Name,
            product.Price,
            DateTime.UtcNow
        ));

        await InvalidateCacheAsync();
        await InvalidateCacheAsync($"products:{id}");

        return product;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        // Publish event
        _messagePublisher.PublishProductDeleted(new ProductDeletedMessage(
            id,
            DateTime.UtcNow
        ));

        await InvalidateCacheAsync();
        await InvalidateCacheAsync($"products:{id}");

        return true;
    }

    // ... rest of the methods remain the same
}
```

### Step 6: Register MessagePublisher

Update `Program.cs`:

```csharp
// Add RabbitMQ
builder.AddRabbitMQClient("messaging");

// Register services
builder.Services.AddScoped<ProductService>();
builder.Services.AddSingleton<MessagePublisher>();
```

### Step 7: Create Message Consumer (Background Service)

Create `AspireComponents.ApiService/Services/ProductEventConsumer.cs`:

```csharp
using System.Text;
using AspireComponents.ApiService.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace AspireComponents.ApiService.Services;

public class ProductEventConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly ILogger<ProductEventConsumer> _logger;
    private IModel? _channel;

    public ProductEventConsumer(IConnection connection, ILogger<ProductEventConsumer> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare("product-events", ExchangeType.Topic, durable: true);
        
        var queueName = _channel.QueueDeclare(
            queue: "product-events-log",
            durable: true,
            exclusive: false,
            autoDelete: false).QueueName;

        _channel.QueueBind(queueName, "product-events", "product.*");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            _logger.LogInformation(
                "Received message with routing key {RoutingKey}: {Message}",
                ea.RoutingKey,
                message);

            // Process the message based on routing key
            switch (ea.RoutingKey)
            {
                case "product.created":
                    var created = JsonSerializer.Deserialize<ProductCreatedMessage>(message);
                    _logger.LogInformation("Product created: {ProductName} (ID: {ProductId})", 
                        created!.ProductName, created.ProductId);
                    break;
                    
                case "product.updated":
                    var updated = JsonSerializer.Deserialize<ProductUpdatedMessage>(message);
                    _logger.LogInformation("Product updated: {ProductName} (ID: {ProductId})", 
                        updated!.ProductName, updated.ProductId);
                    break;
                    
                case "product.deleted":
                    var deleted = JsonSerializer.Deserialize<ProductDeletedMessage>(message);
                    _logger.LogInformation("Product deleted: ID {ProductId}", deleted!.ProductId);
                    break;
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queueName, false, consumer);

        _logger.LogInformation("Product event consumer started");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        base.Dispose();
    }
}
```

### Step 8: Register Background Service

In `Program.cs`:

```csharp
builder.Services.AddHostedService<ProductEventConsumer>();
```

### Step 9: Test Messaging

```bash
cd ../AspireComponents.AppHost
dotnet run
```

**Test messaging:**

```bash
# Create a product and watch the logs
curl -X POST https://localhost:7XXX/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Headphones",
    "description": "Noise cancelling",
    "price": 199.99,
    "stockQuantity": 30
  }'
```

**In the Dashboard:**
1. Go to Structured Logs
2. Filter by "apiservice"
3. Look for "Published message" and "Received message" logs
4. Click on RabbitMQ management UI to see queues and exchanges

### ✅ Verification Point 3

**Check:**
- [ ] RabbitMQ container running
- [ ] Messages published when creating/updating/deleting products
- [ ] Consumer receives and processes messages
- [ ] Logs show message flow
- [ ] RabbitMQ Management UI shows queue activity

## 🎯 Challenge Tasks

1. **Add message retry logic** with dead letter queue
2. **Create separate consumer service** instead of background service
3. **Add message validation** before processing
4. **Implement distributed caching** with cache invalidation via messaging
5. **Add metrics** for cache hit ratio

### Challenge Solution Example:

```csharp
// Dead letter queue configuration
var queueArgs = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", "product-events-dlx" },
    { "x-dead-letter-routing-key", "failed" },
    { "x-message-ttl", 60000 } // 60 seconds
};

_channel.QueueDeclare(
    queue: "product-events-log",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: queueArgs);
```

## 📝 Summary

In this exercise, you:

- ✅ Integrated PostgreSQL with Entity Framework Core
- ✅ Implemented Redis caching with cache-aside pattern
- ✅ Added RabbitMQ for async messaging
- ✅ Created a complete CRUD API with caching and messaging
- ✅ Observed all components in the Aspire dashboard

## 🧹 Cleanup

```bash
# Stop the application (Ctrl+C)
# Clean up Docker resources
docker container prune -f
docker volume prune -f
```

## 🎓 Key Takeaways

1. **Aspire components** simplify infrastructure integration
2. **Connection strings** are managed automatically
3. **Cache-aside pattern** improves performance
4. **Messaging enables** decoupled services
5. **Dashboard provides** unified observability

## ➡️ Next Steps

Continue to [Exercise 3: Multi-Service E-Commerce Application](03_multi_service_app.md) to build a complete microservices system.
