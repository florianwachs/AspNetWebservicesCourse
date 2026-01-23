# Exercise 4: Observability Deep Dive

## 🎯 Objectives

In this exercise, you will:

- Implement custom metrics with `Meter` and `IMeterFactory`
- Create custom traces with `Activity` and `ActivitySource`
- Add structured logging with semantic logging
- Use the Aspire dashboard for debugging
- Trace complex workflows across services
- Analyze performance bottlenecks
- Create custom dashboards and alerts

## ⏱️ Estimated Time

60-90 minutes

## 📋 Prerequisites

- Completed Exercises 1-3
- Understanding of OpenTelemetry concepts
- Familiarity with distributed tracing

## 📊 Part 1: Custom Metrics

### Step 1: Create a New Project

```bash
cd aspire-labs
dotnet new aspire-starter -n ObservabilityLab
cd ObservabilityLab
```

### Step 2: Create a Metrics Service

Create `ObservabilityLab.ApiService/Services/MetricsService.cs`:

```csharp
using System.Diagnostics.Metrics;

namespace ObservabilityLab.ApiService.Services;

public class MetricsService
{
    private readonly Meter _meter;
    private readonly Counter<long> _requestCounter;
    private readonly Histogram<double> _requestDuration;
    private readonly ObservableGauge<int> _activeRequests;
    private readonly Counter<long> _errorCounter;
    private readonly Histogram<long> _cacheHits;
    
    private int _activeRequestCount;

    public MetricsService(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create("ObservabilityLab.Api");

        // Counter: Track total requests
        _requestCounter = _meter.CreateCounter<long>(
            "api.requests.total",
            description: "Total number of API requests");

        // Histogram: Track request durations
        _requestDuration = _meter.CreateHistogram<double>(
            "api.requests.duration",
            unit: "ms",
            description: "Duration of API requests in milliseconds");

        // Observable Gauge: Track active requests
        _activeRequests = _meter.CreateObservableGauge(
            "api.requests.active",
            () => _activeRequestCount,
            description: "Number of currently active requests");

        // Counter: Track errors
        _errorCounter = _meter.CreateCounter<long>(
            "api.errors.total",
            description: "Total number of errors");

        // Histogram: Track cache performance
        _cacheHits = _meter.CreateHistogram<long>(
            "api.cache.hits",
            description: "Cache hit or miss");
    }

    public void RecordRequest(string endpoint, string method)
    {
        _requestCounter.Add(1, 
            new KeyValuePair<string, object?>("endpoint", endpoint),
            new KeyValuePair<string, object?>("method", method));
    }

    public void RecordRequestDuration(double durationMs, string endpoint, int statusCode)
    {
        _requestDuration.Record(durationMs,
            new KeyValuePair<string, object?>("endpoint", endpoint),
            new KeyValuePair<string, object?>("status_code", statusCode));
    }

    public void IncrementActiveRequests()
    {
        Interlocked.Increment(ref _activeRequestCount);
    }

    public void DecrementActiveRequests()
    {
        Interlocked.Decrement(ref _activeRequestCount);
    }

    public void RecordError(string errorType, string endpoint)
    {
        _errorCounter.Add(1,
            new KeyValuePair<string, object?>("error_type", errorType),
            new KeyValuePair<string, object?>("endpoint", endpoint));
    }

    public void RecordCacheResult(bool isHit, string cacheKey)
    {
        _cacheHits.Record(isHit ? 1 : 0,
            new KeyValuePair<string, object?>("result", isHit ? "hit" : "miss"),
            new KeyValuePair<string, object?>("cache_key", cacheKey));
    }
}
```

### Step 3: Create Metrics Middleware

Create `ObservabilityLab.ApiService/Middleware/MetricsMiddleware.cs`:

```csharp
using System.Diagnostics;
using ObservabilityLab.ApiService.Services;

namespace ObservabilityLab.ApiService.Middleware;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MetricsService _metricsService;

    public MetricsMiddleware(RequestDelegate next, MetricsService metricsService)
    {
        _next = next;
        _metricsService = metricsService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.Request.Path.Value ?? "/";
        var method = context.Request.Method;
        var stopwatch = Stopwatch.StartNew();

        _metricsService.IncrementActiveRequests();
        _metricsService.RecordRequest(endpoint, method);

        try
        {
            await _next(context);
            
            stopwatch.Stop();
            _metricsService.RecordRequestDuration(
                stopwatch.Elapsed.TotalMilliseconds,
                endpoint,
                context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metricsService.RecordError(ex.GetType().Name, endpoint);
            throw;
        }
        finally
        {
            _metricsService.DecrementActiveRequests();
        }
    }
}

public static class MetricsMiddlewareExtensions
{
    public static IApplicationBuilder UseMetrics(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MetricsMiddleware>();
    }
}
```

### Step 4: Register and Use Metrics

Update `ObservabilityLab.ApiService/Program.cs`:

```csharp
using ObservabilityLab.ApiService.Middleware;
using ObservabilityLab.ApiService.Services;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Register metrics service
builder.Services.AddSingleton<MetricsService>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

// Use metrics middleware
app.UseMetrics();

// Sample endpoints
app.MapGet("/api/fast", (MetricsService metrics) =>
{
    metrics.RecordCacheResult(true, "sample-data");
    return new { message = "Fast response", timestamp = DateTime.UtcNow };
});

app.MapGet("/api/slow", async (MetricsService metrics) =>
{
    metrics.RecordCacheResult(false, "sample-data");
    await Task.Delay(Random.Shared.Next(100, 500)); // Simulate slow operation
    return new { message = "Slow response", timestamp = DateTime.UtcNow };
});

app.MapGet("/api/error", (MetricsService metrics) =>
{
    throw new InvalidOperationException("Simulated error");
});

app.MapGet("/api/data/{id}", (int id, MetricsService metrics) =>
{
    // Simulate cache behavior
    var isCacheHit = id % 3 == 0;
    metrics.RecordCacheResult(isCacheHit, $"data:{id}");
    
    if (isCacheHit)
    {
        return new { id, data = "From cache", timestamp = DateTime.UtcNow };
    }
    
    Thread.Sleep(50); // Simulate database query
    return new { id, data = "From database", timestamp = DateTime.UtcNow };
});

app.MapDefaultEndpoints();

app.Run();
```

### ✅ Verification Point 1

Run the application and generate traffic:

```bash
# In one terminal
cd ObservabilityLab.AppHost
dotnet run

# In another terminal, generate requests
for i in {1..50}; do
  curl https://localhost:7xxx/api/fast
  curl https://localhost:7xxx/api/slow
  curl https://localhost:7xxx/api/data/$i
done

# Generate some errors
curl https://localhost:7xxx/api/error
```

**In the Dashboard:**
1. Go to **Metrics** tab
2. Select "apiservice"
3. View your custom metrics:
   - `api.requests.total`
   - `api.requests.duration`
   - `api.requests.active`
   - `api.errors.total`
   - `api.cache.hits`

## 🔍 Part 2: Custom Distributed Tracing

### Step 1: Create Activity Source

Create `ObservabilityLab.ApiService/Tracing/ActivitySources.cs`:

```csharp
using System.Diagnostics;

namespace ObservabilityLab.ApiService.Tracing;

public static class ActivitySources
{
    public static readonly ActivitySource ApiService = new("ObservabilityLab.Api");
    public static readonly ActivitySource DataAccess = new("ObservabilityLab.DataAccess");
    public static readonly ActivitySource ExternalService = new("ObservabilityLab.ExternalService");
}
```

### Step 2: Register Activity Sources

Update `Program.cs` to register activity sources with OpenTelemetry:

```csharp
using ObservabilityLab.ApiService.Tracing;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add custom activity sources to OpenTelemetry
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
{
    tracing.AddSource(ActivitySources.ApiService.Name);
    tracing.AddSource(ActivitySources.DataAccess.Name);
    tracing.AddSource(ActivitySources.ExternalService.Name);
});

// ... rest of the configuration
```

### Step 3: Create Service with Tracing

Create `ObservabilityLab.ApiService/Services/OrderProcessingService.cs`:

```csharp
using System.Diagnostics;
using ObservabilityLab.ApiService.Tracing;

namespace ObservabilityLab.ApiService.Services;

public class OrderProcessingService
{
    private readonly ILogger<OrderProcessingService> _logger;
    private readonly MetricsService _metrics;

    public OrderProcessingService(ILogger<OrderProcessingService> logger, MetricsService metrics)
    {
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<OrderResult> ProcessOrderAsync(int orderId, List<int> productIds)
    {
        using var activity = ActivitySources.ApiService.StartActivity("ProcessOrder");
        activity?.SetTag("order.id", orderId);
        activity?.SetTag("order.product_count", productIds.Count);

        _logger.LogInformation("Processing order {OrderId} with {ProductCount} products", 
            orderId, productIds.Count);

        try
        {
            // Step 1: Validate order
            await ValidateOrderAsync(orderId, productIds);

            // Step 2: Check inventory
            var inventoryResult = await CheckInventoryAsync(productIds);
            
            if (!inventoryResult.IsAvailable)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Insufficient inventory");
                activity?.SetTag("order.status", "rejected");
                return new OrderResult { Success = false, Message = "Insufficient inventory" };
            }

            // Step 3: Calculate pricing
            var totalAmount = await CalculatePricingAsync(productIds);
            activity?.SetTag("order.total_amount", totalAmount);

            // Step 4: Process payment
            var paymentResult = await ProcessPaymentAsync(orderId, totalAmount);
            
            if (!paymentResult.Success)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Payment failed");
                activity?.SetTag("order.status", "payment_failed");
                return new OrderResult { Success = false, Message = "Payment failed" };
            }

            // Step 5: Reserve inventory
            await ReserveInventoryAsync(productIds);

            // Step 6: Create shipment
            var shipmentId = await CreateShipmentAsync(orderId);
            activity?.SetTag("order.shipment_id", shipmentId);

            activity?.SetTag("order.status", "completed");
            
            _logger.LogInformation("Order {OrderId} processed successfully. Shipment: {ShipmentId}", 
                orderId, shipmentId);

            return new OrderResult 
            { 
                Success = true, 
                Message = "Order processed successfully",
                ShipmentId = shipmentId,
                TotalAmount = totalAmount
            };
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            _logger.LogError(ex, "Error processing order {OrderId}", orderId);
            throw;
        }
    }

    private async Task ValidateOrderAsync(int orderId, List<int> productIds)
    {
        using var activity = ActivitySources.ApiService.StartActivity("ValidateOrder");
        activity?.SetTag("order.id", orderId);

        _logger.LogDebug("Validating order {OrderId}", orderId);
        
        await Task.Delay(Random.Shared.Next(10, 50)); // Simulate validation

        if (productIds.Count == 0)
        {
            throw new InvalidOperationException("Order must contain at least one product");
        }

        activity?.AddEvent(new ActivityEvent("Order validated"));
    }

    private async Task<InventoryResult> CheckInventoryAsync(List<int> productIds)
    {
        using var activity = ActivitySources.DataAccess.StartActivity("CheckInventory");
        activity?.SetTag("inventory.product_count", productIds.Count);

        _logger.LogDebug("Checking inventory for {ProductCount} products", productIds.Count);

        // Simulate database query
        await Task.Delay(Random.Shared.Next(50, 150));

        var isAvailable = Random.Shared.Next(100) > 10; // 90% success rate
        
        activity?.SetTag("inventory.available", isAvailable);
        activity?.AddEvent(new ActivityEvent("Inventory checked"));

        _metrics.RecordCacheResult(false, $"inventory:{string.Join(",", productIds)}");

        return new InventoryResult { IsAvailable = isAvailable };
    }

    private async Task<decimal> CalculatePricingAsync(List<int> productIds)
    {
        using var activity = ActivitySources.ApiService.StartActivity("CalculatePricing");
        activity?.SetTag("pricing.product_count", productIds.Count);

        _logger.LogDebug("Calculating pricing for {ProductCount} products", productIds.Count);

        await Task.Delay(Random.Shared.Next(20, 80));

        var totalAmount = productIds.Sum(id => id * 10.99m); // Simple pricing logic
        
        activity?.SetTag("pricing.total_amount", totalAmount);
        activity?.AddEvent(new ActivityEvent("Pricing calculated"));

        return totalAmount;
    }

    private async Task<PaymentResult> ProcessPaymentAsync(int orderId, decimal amount)
    {
        using var activity = ActivitySources.ExternalService.StartActivity("ProcessPayment");
        activity?.SetTag("payment.order_id", orderId);
        activity?.SetTag("payment.amount", amount);
        activity?.SetTag("payment.provider", "stripe");

        _logger.LogInformation("Processing payment for order {OrderId}: {Amount:C}", orderId, amount);

        // Simulate external payment service call
        await Task.Delay(Random.Shared.Next(100, 300));

        var success = Random.Shared.Next(100) > 5; // 95% success rate
        
        activity?.SetTag("payment.success", success);
        
        if (success)
        {
            var transactionId = Guid.NewGuid().ToString();
            activity?.SetTag("payment.transaction_id", transactionId);
            activity?.AddEvent(new ActivityEvent("Payment successful"));
            return new PaymentResult { Success = true, TransactionId = transactionId };
        }
        else
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Payment declined");
            activity?.AddEvent(new ActivityEvent("Payment failed"));
            return new PaymentResult { Success = false };
        }
    }

    private async Task ReserveInventoryAsync(List<int> productIds)
    {
        using var activity = ActivitySources.DataAccess.StartActivity("ReserveInventory");
        activity?.SetTag("inventory.product_count", productIds.Count);

        _logger.LogDebug("Reserving inventory for {ProductCount} products", productIds.Count);

        await Task.Delay(Random.Shared.Next(30, 100));

        activity?.AddEvent(new ActivityEvent("Inventory reserved"));
    }

    private async Task<string> CreateShipmentAsync(int orderId)
    {
        using var activity = ActivitySources.ExternalService.StartActivity("CreateShipment");
        activity?.SetTag("shipment.order_id", orderId);
        activity?.SetTag("shipment.carrier", "fedex");

        _logger.LogInformation("Creating shipment for order {OrderId}", orderId);

        await Task.Delay(Random.Shared.Next(80, 200));

        var shipmentId = $"SHIP-{orderId}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        
        activity?.SetTag("shipment.id", shipmentId);
        activity?.AddEvent(new ActivityEvent("Shipment created"));

        return shipmentId;
    }
}

public record OrderResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
    public string? ShipmentId { get; init; }
    public decimal TotalAmount { get; init; }
}

public record InventoryResult
{
    public bool IsAvailable { get; init; }
}

public record PaymentResult
{
    public bool Success { get; init; }
    public string? TransactionId { get; init; }
}
```

### Step 4: Add Endpoint to Use Tracing

Update `Program.cs`:

```csharp
using ObservabilityLab.ApiService.Services;

// Register services
builder.Services.AddScoped<OrderProcessingService>();

// ... after app.MapDefaultEndpoints();

app.MapPost("/api/orders/process", async (
    int orderId, 
    List<int> productIds, 
    OrderProcessingService orderService) =>
{
    var result = await orderService.ProcessOrderAsync(orderId, productIds);
    
    if (result.Success)
    {
        return Results.Ok(result);
    }
    else
    {
        return Results.BadRequest(result);
    }
});
```

### ✅ Verification Point 2

Generate complex traces:

```bash
# Process some orders
curl -X POST https://localhost:7xxx/api/orders/process?orderId=1001 \
  -H "Content-Type: application/json" \
  -d '[1, 2, 3]'

curl -X POST https://localhost:7xxx/api/orders/process?orderId=1002 \
  -H "Content-Type: application/json" \
  -d '[4, 5, 6, 7]'
```

**In the Dashboard - Traces Tab:**
1. Find your order processing traces
2. Expand to see the full trace tree:
   - ProcessOrder (root)
   - ValidateOrder
   - CheckInventory
   - CalculatePricing
   - ProcessPayment
   - ReserveInventory
   - CreateShipment
3. Click on individual spans to see tags and events
4. Observe timing for each operation

## 📝 Part 3: Structured Logging

### Step 1: Create Logging Extensions

Create `ObservabilityLab.ApiService/Logging/LoggerExtensions.cs`:

```csharp
using Microsoft.Extensions.Logging;

namespace ObservabilityLab.ApiService.Logging;

public static partial class LoggerExtensions
{
    // High-performance logging with source generation

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "Order {OrderId} created by user {UserId} with {ItemCount} items totaling {TotalAmount:C}")]
    public static partial void LogOrderCreated(
        this ILogger logger,
        int orderId,
        string userId,
        int itemCount,
        decimal totalAmount);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Order {OrderId} payment failed. Reason: {Reason}")]
    public static partial void LogPaymentFailed(
        this ILogger logger,
        int orderId,
        string reason);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "Order {OrderId} processing failed")]
    public static partial void LogOrderProcessingFailed(
        this ILogger logger,
        Exception exception,
        int orderId);

    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Information,
        Message = "Cache {CacheKey} {Result} (duration: {DurationMs}ms)")]
    public static partial void LogCacheOperation(
        this ILogger logger,
        string cacheKey,
        string result,
        long durationMs);

    [LoggerMessage(
        EventId = 3000,
        Level = LogLevel.Warning,
        Message = "Slow query detected: {QueryName} took {DurationMs}ms (threshold: {ThresholdMs}ms)")]
    public static partial void LogSlowQuery(
        this ILogger logger,
        string queryName,
        long durationMs,
        long thresholdMs);

    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Critical,
        Message = "External service {ServiceName} is unavailable. Circuit breaker opened.")]
    public static partial void LogServiceUnavailable(
        this ILogger logger,
        string serviceName);
}
```

### Step 2: Use Structured Logging

Update `OrderProcessingService.cs` to use structured logging:

```csharp
using ObservabilityLab.ApiService.Logging;

public class OrderProcessingService
{
    // In ProcessOrderAsync method:
    try
    {
        // ... processing logic ...

        _logger.LogOrderCreated(orderId, "user123", productIds.Count, totalAmount);
        
        return new OrderResult { ... };
    }
    catch (Exception ex)
    {
        _logger.LogOrderProcessingFailed(ex, orderId);
        throw;
    }

    // In ProcessPaymentAsync:
    if (!success)
    {
        _logger.LogPaymentFailed(orderId, "Payment declined by provider");
        // ...
    }
}
```

### Step 3: Add Query Performance Monitoring

Create `ObservabilityLab.ApiService/Services/DataService.cs`:

```csharp
using System.Diagnostics;
using ObservabilityLab.ApiService.Logging;
using ObservabilityLab.ApiService.Tracing;

namespace ObservabilityLab.ApiService.Services;

public class DataService
{
    private readonly ILogger<DataService> _logger;
    private readonly MetricsService _metrics;
    private const long SlowQueryThresholdMs = 100;

    public DataService(ILogger<DataService> logger, MetricsService metrics)
    {
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        using var activity = ActivitySources.DataAccess.StartActivity("GetProducts");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Simulate database query
            await Task.Delay(Random.Shared.Next(20, 150));

            var products = Enumerable.Range(1, 10)
                .Select(i => new Product { Id = i, Name = $"Product {i}", Price = i * 10.99m })
                .ToList();

            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > SlowQueryThresholdMs)
            {
                _logger.LogSlowQuery("GetProducts", stopwatch.ElapsedMilliseconds, SlowQueryThresholdMs);
            }

            _metrics.RecordRequestDuration(stopwatch.Elapsed.TotalMilliseconds, "GetProducts", 200);

            activity?.SetTag("query.row_count", products.Count);
            activity?.SetTag("query.duration_ms", stopwatch.ElapsedMilliseconds);

            return products;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        using var activity = ActivitySources.DataAccess.StartActivity("GetProductById");
        activity?.SetTag("product.id", id);

        var cacheKey = $"product:{id}";
        var stopwatch = Stopwatch.StartNew();

        // Simulate cache check
        var isCacheHit = Random.Shared.Next(100) < 30; // 30% cache hit rate
        
        if (isCacheHit)
        {
            stopwatch.Stop();
            _logger.LogCacheOperation(cacheKey, "HIT", stopwatch.ElapsedMilliseconds);
            _metrics.RecordCacheResult(true, cacheKey);
            
            return new Product { Id = id, Name = $"Product {id}", Price = id * 10.99m };
        }

        // Cache miss - query database
        await Task.Delay(Random.Shared.Next(50, 200));
        stopwatch.Stop();

        _logger.LogCacheOperation(cacheKey, "MISS", stopwatch.ElapsedMilliseconds);
        _metrics.RecordCacheResult(false, cacheKey);

        if (stopwatch.ElapsedMilliseconds > SlowQueryThresholdMs)
        {
            _logger.LogSlowQuery("GetProductById", stopwatch.ElapsedMilliseconds, SlowQueryThresholdMs);
        }

        return new Product { Id = id, Name = $"Product {id}", Price = id * 10.99m };
    }
}

public record Product
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; init; }
}
```

### Step 4: Add Data Endpoints

In `Program.cs`:

```csharp
builder.Services.AddScoped<DataService>();

// ... 

app.MapGet("/api/products", async (DataService dataService) =>
{
    var products = await dataService.GetProductsAsync();
    return Results.Ok(products);
});

app.MapGet("/api/products/{id}", async (int id, DataService dataService) =>
{
    var product = await dataService.GetProductByIdAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});
```

### ✅ Verification Point 3

Test structured logging:

```bash
# Generate various requests
for i in {1..20}; do
  curl https://localhost:7xxx/api/products
  curl https://localhost:7xxx/api/products/$i
done
```

**In the Dashboard - Structured Logs:**
1. Filter by log level
2. Search for specific event IDs (1000, 1001, 2000, 3000)
3. Expand log entries to see structured data
4. Use filters to find slow queries
5. Correlate logs with traces using trace IDs

## 🎯 Challenge Tasks

1. **Create a custom metric** for cache hit ratio percentage
2. **Add baggage propagation** to pass user context through traces
3. **Implement sampling** for high-volume traces
4. **Create alert rules** based on metrics thresholds
5. **Build a performance dashboard** showing key metrics

### Challenge 1 Solution Example:

```csharp
// Custom cache hit ratio metric
public class CacheMetricsService
{
    private long _hits;
    private long _misses;
    private readonly ObservableGauge<double> _hitRatio;

    public CacheMetricsService(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("ObservabilityLab.Cache");
        
        _hitRatio = meter.CreateObservableGauge(
            "cache.hit_ratio",
            () => CalculateHitRatio(),
            unit: "%",
            description: "Cache hit ratio percentage");
    }

    public void RecordHit() => Interlocked.Increment(ref _hits);
    public void RecordMiss() => Interlocked.Increment(ref _misses);

    private double CalculateHitRatio()
    {
        var total = _hits + _misses;
        return total == 0 ? 0 : (_hits * 100.0) / total;
    }
}
```

## 📝 Summary

In this exercise, you:

- ✅ Implemented custom metrics with `Meter`
- ✅ Created distributed traces with `Activity`
- ✅ Added structured logging with source generators
- ✅ Built a complete observability stack
- ✅ Analyzed performance using the dashboard
- ✅ Identified and debugged issues

## 🎓 Key Takeaways

1. **Custom metrics** provide business-specific insights
2. **Distributed tracing** reveals workflow bottlenecks
3. **Structured logging** enables powerful queries
4. **Activity Source** organizes traces by component
5. **OpenTelemetry** provides consistent observability

## ➡️ Next Steps

Continue to [Exercise 5: Deployment to Azure](05_deployment_lab.md) to deploy your application to production.
