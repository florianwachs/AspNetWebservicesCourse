# Exercise 3: Multi-Service E-Commerce Application

## 🎯 Objectives

In this exercise, you will:

- Build a complete microservices-based e-commerce application
- Create separate services for catalog, cart, and orders
- Develop a Blazor frontend
- Implement service-to-service communication
- Add comprehensive observability
- Apply microservices patterns

## ⏱️ Estimated Time

90-120 minutes

## 📋 Prerequisites

- Completed Exercise 1 and 2
- Understanding of microservices architecture
- Familiarity with Blazor (basic)
- REST API design knowledge

## 🏗️ Architecture Overview

We'll build:

```
┌──────────────┐
│   Blazor     │
│   Frontend   │
└──────┬───────┘
       │
   ┌───┴────────────────────┐
   │                        │
┌──▼───────┐    ┌──────────▼─┐    ┌──────────────┐
│ Catalog  │    │   Cart     │    │    Order     │
│   API    │    │   API      │    │     API      │
└───┬──────┘    └──────┬─────┘    └──────┬───────┘
    │                  │                  │
┌───▼──────┐    ┌──────▼─────┐    ┌──────▼───────┐
│PostgreSQL│    │   Redis    │    │   RabbitMQ   │
└──────────┘    └────────────┘    └──────────────┘
```

## 🛠️ Part 1: Create the Solution Structure

### Step 1: Create Solution and Projects

```bash
cd aspire-labs
mkdir ECommerceApp
cd ECommerceApp

# Create solution
dotnet new sln -n ECommerceApp

# Create Aspire AppHost
dotnet new aspire-apphost -n ECommerceApp.AppHost
dotnet sln add ECommerceApp.AppHost

# Create ServiceDefaults
dotnet new aspire-servicedefaults -n ECommerceApp.ServiceDefaults
dotnet sln add ECommerceApp.ServiceDefaults

# Create API projects
dotnet new webapi -n ECommerceApp.CatalogApi
dotnet new webapi -n ECommerceApp.CartApi
dotnet new webapi -n ECommerceApp.OrderApi
dotnet sln add ECommerceApp.CatalogApi
dotnet sln add ECommerceApp.CartApi
dotnet sln add ECommerceApp.OrderApi

# Create Blazor frontend
dotnet new blazor -n ECommerceApp.Web
dotnet sln add ECommerceApp.Web
```

### Step 2: Add References

```bash
# Add ServiceDefaults to all projects
cd ECommerceApp.CatalogApi
dotnet add reference ../ECommerceApp.ServiceDefaults

cd ../ECommerceApp.CartApi
dotnet add reference ../ECommerceApp.ServiceDefaults

cd ../ECommerceApp.OrderApi
dotnet add reference ../ECommerceApp.ServiceDefaults

cd ../ECommerceApp.Web
dotnet add reference ../ECommerceApp.ServiceDefaults

# Add project references to AppHost
cd ../ECommerceApp.AppHost
dotnet add reference ../ECommerceApp.CatalogApi
dotnet add reference ../ECommerceApp.CartApi
dotnet add reference ../ECommerceApp.OrderApi
dotnet add reference ../ECommerceApp.Web
```

## 📦 Part 2: Build Catalog API

### Step 1: Add Required Packages

```bash
cd ../ECommerceApp.CatalogApi
dotnet add package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### Step 2: Create Models

Create `Models/Product.cs`:

```csharp
namespace ECommerceApp.CatalogApi.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public required string Category { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
```

### Step 3: Create DbContext

Create `Data/CatalogDbContext.cs`:

```csharp
using ECommerceApp.CatalogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.CatalogApi.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
        });

        // Seed categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices" },
            new Category { Id = 2, Name = "Clothing", Description = "Apparel and accessories" },
            new Category { Id = 3, Name = "Books", Description = "Physical and digital books" }
        );

        // Seed products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop Pro 15",
                Description = "High-performance laptop for professionals",
                Price = 1299.99m,
                Category = "Electronics",
                StockQuantity = 50,
                ImageUrl = "https://via.placeholder.com/300x200?text=Laptop"
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse",
                Price = 29.99m,
                Category = "Electronics",
                StockQuantity = 200,
                ImageUrl = "https://via.placeholder.com/300x200?text=Mouse"
            },
            new Product
            {
                Id = 3,
                Name = "T-Shirt Classic",
                Description = "Comfortable cotton t-shirt",
                Price = 19.99m,
                Category = "Clothing",
                StockQuantity = 150,
                ImageUrl = "https://via.placeholder.com/300x200?text=TShirt"
            },
            new Product
            {
                Id = 4,
                Name = "C# Programming Guide",
                Description = "Comprehensive C# programming book",
                Price = 49.99m,
                Category = "Books",
                StockQuantity = 75,
                ImageUrl = "https://via.placeholder.com/300x200?text=Book"
            }
        );
    }
}
```

### Step 4: Implement Catalog API

Replace `Program.cs`:

```csharp
using ECommerceApp.CatalogApi.Data;
using ECommerceApp.CatalogApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add database
builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");

// Add services
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseExceptionHandler();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// API Endpoints

// Get all products
app.MapGet("/api/products", async (CatalogDbContext db, string? category, bool? inStock) =>
{
    var query = db.Products.Where(p => p.IsActive);

    if (!string.IsNullOrEmpty(category))
    {
        query = query.Where(p => p.Category == category);
    }

    if (inStock == true)
    {
        query = query.Where(p => p.StockQuantity > 0);
    }

    return await query.ToListAsync();
})
.WithName("GetProducts")
.WithOpenApi();

// Get product by ID
app.MapGet("/api/products/{id}", async (int id, CatalogDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProduct")
.WithOpenApi();

// Search products
app.MapGet("/api/products/search", async (string query, CatalogDbContext db) =>
{
    var products = await db.Products
        .Where(p => p.IsActive && p.Name.Contains(query))
        .ToListAsync();
    return products;
})
.WithName("SearchProducts")
.WithOpenApi();

// Get categories
app.MapGet("/api/categories", async (CatalogDbContext db) =>
{
    return await db.Categories.ToListAsync();
})
.WithName("GetCategories")
.WithOpenApi();

// Create product
app.MapPost("/api/products", async (Product product, CatalogDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", product);
})
.WithName("CreateProduct")
.WithOpenApi();

// Update stock
app.MapPatch("/api/products/{id}/stock", async (int id, int quantity, CatalogDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    product.StockQuantity = quantity;
    product.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.Ok(product);
})
.WithName("UpdateStock")
.WithOpenApi();

app.MapDefaultEndpoints();

app.Run();
```

## 🛒 Part 3: Build Cart API

### Step 1: Add Required Packages

```bash
cd ../ECommerceApp.CartApi
dotnet add package Aspire.StackExchange.Redis
```

### Step 2: Create Models

Create `Models/CartModels.cs`:

```csharp
namespace ECommerceApp.CartApi.Models;

public class Cart
{
    public required string UserId { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    public int TotalItems => Items.Sum(i => i.Quantity);
}

public class CartItem
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }

    public decimal TotalPrice => Price * Quantity;
}

public record AddToCartRequest(int ProductId, string ProductName, decimal Price, int Quantity, string? ImageUrl);
public record UpdateCartItemRequest(int Quantity);
```

### Step 3: Create Cart Service

Create `Services/CartService.cs`:

```csharp
using System.Text.Json;
using ECommerceApp.CartApi.Models;
using StackExchange.Redis;

namespace ECommerceApp.CartApi.Services;

public class CartService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<CartService> _logger;
    private const string CartKeyPrefix = "cart:";

    public CartService(IConnectionMultiplexer redis, ILogger<CartService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<Cart> GetCartAsync(string userId)
    {
        var db = _redis.GetDatabase();
        var key = $"{CartKeyPrefix}{userId}";
        
        var data = await db.StringGetAsync(key);
        
        if (data.HasValue)
        {
            return JsonSerializer.Deserialize<Cart>(data!)!;
        }

        // Return empty cart
        return new Cart { UserId = userId };
    }

    public async Task<Cart> AddItemAsync(string userId, AddToCartRequest request)
    {
        var cart = await GetCartAsync(userId);
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = request.ProductId,
                ProductName = request.ProductName,
                Price = request.Price,
                Quantity = request.Quantity,
                ImageUrl = request.ImageUrl
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await SaveCartAsync(cart);

        _logger.LogInformation("Added {Quantity}x {ProductName} to cart for user {UserId}", 
            request.Quantity, request.ProductName, userId);

        return cart;
    }

    public async Task<Cart> UpdateItemAsync(string userId, int productId, int quantity)
    {
        var cart = await GetCartAsync(userId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item == null)
        {
            throw new InvalidOperationException("Item not found in cart");
        }

        if (quantity <= 0)
        {
            cart.Items.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await SaveCartAsync(cart);

        return cart;
    }

    public async Task<Cart> RemoveItemAsync(string userId, int productId)
    {
        var cart = await GetCartAsync(userId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item != null)
        {
            cart.Items.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await SaveCartAsync(cart);
        }

        return cart;
    }

    public async Task ClearCartAsync(string userId)
    {
        var db = _redis.GetDatabase();
        var key = $"{CartKeyPrefix}{userId}";
        await db.KeyDeleteAsync(key);
        
        _logger.LogInformation("Cleared cart for user {UserId}", userId);
    }

    private async Task SaveCartAsync(Cart cart)
    {
        var db = _redis.GetDatabase();
        var key = $"{CartKeyPrefix}{cart.UserId}";
        var json = JsonSerializer.Serialize(cart);
        
        await db.StringSetAsync(key, json, TimeSpan.FromDays(7));
    }
}
```

### Step 4: Implement Cart API

Replace `Program.cs`:

```csharp
using ECommerceApp.CartApi.Models;
using ECommerceApp.CartApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add Redis
builder.AddRedisClient("cache");

// Add services
builder.Services.AddSingleton<CartService>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseExceptionHandler();

// Cart API Endpoints

app.MapGet("/api/cart/{userId}", async (string userId, CartService cartService) =>
{
    var cart = await cartService.GetCartAsync(userId);
    return Results.Ok(cart);
})
.WithName("GetCart")
.WithOpenApi();

app.MapPost("/api/cart/{userId}/items", async (string userId, AddToCartRequest request, CartService cartService) =>
{
    var cart = await cartService.AddItemAsync(userId, request);
    return Results.Ok(cart);
})
.WithName("AddToCart")
.WithOpenApi();

app.MapPut("/api/cart/{userId}/items/{productId}", async (string userId, int productId, UpdateCartItemRequest request, CartService cartService) =>
{
    try
    {
        var cart = await cartService.UpdateItemAsync(userId, productId, request.Quantity);
        return Results.Ok(cart);
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
})
.WithName("UpdateCartItem")
.WithOpenApi();

app.MapDelete("/api/cart/{userId}/items/{productId}", async (string userId, int productId, CartService cartService) =>
{
    var cart = await cartService.RemoveItemAsync(userId, productId);
    return Results.Ok(cart);
})
.WithName("RemoveFromCart")
.WithOpenApi();

app.MapDelete("/api/cart/{userId}", async (string userId, CartService cartService) =>
{
    await cartService.ClearCartAsync(userId);
    return Results.NoContent();
})
.WithName("ClearCart")
.WithOpenApi();

app.MapDefaultEndpoints();

app.Run();
```

## 📋 Part 4: Build Order API

### Step 1: Add Required Packages

```bash
cd ../ECommerceApp.OrderApi
dotnet add package Aspire.RabbitMQ.Client
```

### Step 2: Create Models

Create `Models/OrderModels.cs`:

```csharp
namespace ECommerceApp.OrderApi.Models;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string UserId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => Price * Quantity;
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public record CreateOrderRequest(string UserId, List<OrderItemRequest> Items);
public record OrderItemRequest(int ProductId, string ProductName, decimal Price, int Quantity);
```

### Step 3: Create Order Service

Create `Services/OrderService.cs`:

```csharp
using System.Text;
using System.Text.Json;
using ECommerceApp.OrderApi.Models;
using RabbitMQ.Client;

namespace ECommerceApp.OrderApi.Services;

public class OrderService
{
    private readonly IConnection _rabbitConnection;
    private readonly ILogger<OrderService> _logger;
    private readonly List<Order> _orders = new(); // In-memory storage for demo

    public OrderService(IConnection rabbitConnection, ILogger<OrderService> logger)
    {
        _rabbitConnection = rabbitConnection;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            UserId = request.UserId,
            Items = request.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(i => i.TotalPrice);

        _orders.Add(order);

        // Publish order created event
        PublishOrderCreatedEvent(order);

        _logger.LogInformation("Order {OrderId} created for user {UserId} with total {TotalAmount:C}", 
            order.Id, order.UserId, order.TotalAmount);

        return order;
    }

    public Order? GetOrder(Guid orderId)
    {
        return _orders.FirstOrDefault(o => o.Id == orderId);
    }

    public List<Order> GetUserOrders(string userId)
    {
        return _orders.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAt).ToList();
    }

    public async Task<Order?> UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        if (order == null) return null;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        // Publish order status changed event
        PublishOrderStatusChangedEvent(order);

        _logger.LogInformation("Order {OrderId} status updated to {Status}", orderId, status);

        return order;
    }

    private void PublishOrderCreatedEvent(Order order)
    {
        using var channel = _rabbitConnection.CreateModel();
        
        channel.ExchangeDeclare("orders", ExchangeType.Topic, durable: true);

        var message = new
        {
            order.Id,
            order.UserId,
            order.TotalAmount,
            order.CreatedAt,
            ItemCount = order.Items.Count
        };

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish("orders", "order.created", null, body);

        _logger.LogInformation("Published order.created event for order {OrderId}", order.Id);
    }

    private void PublishOrderStatusChangedEvent(Order order)
    {
        using var channel = _rabbitConnection.CreateModel();
        
        channel.ExchangeDeclare("orders", ExchangeType.Topic, durable: true);

        var message = new
        {
            order.Id,
            order.Status,
            order.UpdatedAt
        };

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish("orders", "order.status.changed", null, body);

        _logger.LogInformation("Published order.status.changed event for order {OrderId}", order.Id);
    }
}
```

### Step 4: Implement Order API

Replace `Program.cs`:

```csharp
using ECommerceApp.OrderApi.Models;
using ECommerceApp.OrderApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add RabbitMQ
builder.AddRabbitMQClient("messaging");

// Add services
builder.Services.AddSingleton<OrderService>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseExceptionHandler();

// Order API Endpoints

app.MapPost("/api/orders", (CreateOrderRequest request, OrderService orderService) =>
{
    var order = orderService.CreateOrderAsync(request);
    return Results.Created($"/api/orders/{order.Result.Id}", order.Result);
})
.WithName("CreateOrder")
.WithOpenApi();

app.MapGet("/api/orders/{orderId:guid}", (Guid orderId, OrderService orderService) =>
{
    var order = orderService.GetOrder(orderId);
    return order is not null ? Results.Ok(order) : Results.NotFound();
})
.WithName("GetOrder")
.WithOpenApi();

app.MapGet("/api/orders/user/{userId}", (string userId, OrderService orderService) =>
{
    var orders = orderService.GetUserOrders(userId);
    return Results.Ok(orders);
})
.WithName("GetUserOrders")
.WithOpenApi();

app.MapPatch("/api/orders/{orderId:guid}/status", async (Guid orderId, OrderStatus status, OrderService orderService) =>
{
    var order = await orderService.UpdateOrderStatusAsync(orderId, status);
    return order is not null ? Results.Ok(order) : Results.NotFound();
})
.WithName("UpdateOrderStatus")
.WithOpenApi();

app.MapDefaultEndpoints();

app.Run();
```

## 🌐 Part 5: Configure AppHost

Update `ECommerceApp.AppHost/Program.cs`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalogdb");

var redis = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent);

var messaging = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

// API Services
var catalogApi = builder.AddProject<Projects.ECommerceApp_CatalogApi>("catalogapi")
    .WithReference(catalogDb);

var cartApi = builder.AddProject<Projects.ECommerceApp_CartApi>("cartapi")
    .WithReference(redis);

var orderApi = builder.AddProject<Projects.ECommerceApp_OrderApi>("orderapi")
    .WithReference(messaging);

// Web Frontend
builder.AddProject<Projects.ECommerceApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(catalogApi)
    .WithReference(cartApi)
    .WithReference(orderApi);

builder.Build().Run();
```

## ✅ Verification

### Test the Complete System

```bash
cd ECommerceApp.AppHost
dotnet run
```

**Test Catalog API:**
```bash
curl https://localhost:7xxx/api/products
curl https://localhost:7xxx/api/categories
```

**Test Cart API:**
```bash
# Add to cart
curl -X POST https://localhost:7xxx/api/cart/user123/items \
  -H "Content-Type: application/json" \
  -d '{"productId":1,"productName":"Laptop","price":1299.99,"quantity":1}'

# Get cart
curl https://localhost:7xxx/api/cart/user123
```

**Test Order API:**
```bash
# Create order
curl -X POST https://localhost:7xxx/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "userId":"user123",
    "items":[{"productId":1,"productName":"Laptop","price":1299.99,"quantity":1}]
  }'
```

### In the Dashboard

1. **Resources** - All 6 services running (3 APIs + Web + 3 infrastructure)
2. **Traces** - End-to-end request flows
3. **Structured Logs** - Messages from all services
4. **Metrics** - Performance data

## 📝 Summary

You built a complete microservices e-commerce application with:
- ✅ 3 independent APIs (Catalog, Cart, Order)
- ✅ Database, caching, and messaging
- ✅ Proper service isolation
- ✅ Event-driven communication
- ✅ Comprehensive observability

## ➡️ Next Steps

Continue to [Exercise 4: Observability Deep Dive](04_observability_lab.md)
