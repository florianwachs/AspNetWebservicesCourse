# Module 13: Performance Optimization & HybridCache

## 🎯 Learning Objectives

After completing this module, you will be able to:
- Understand .NET performance fundamentals
- Implement effective caching strategies with HybridCache
- Profile and diagnose performance bottlenecks
- Optimize database queries and data access
- Implement efficient API patterns
- Use System.Threading.Channels for high-throughput scenarios
- Apply memory management best practices
- Measure and improve application performance

## 📚 Topics Covered

### 1. Performance Fundamentals
- Understanding .NET performance characteristics
- Performance vs scalability
- Performance budgets and SLAs
- Common performance anti-patterns
- Tools for performance analysis

### 2. HybridCache (New in .NET 9+)
- What is HybridCache?
- L1 (in-memory) and L2 (distributed) caching
- Cache stampede prevention
- Serialization optimization
- Cache invalidation strategies
- HybridCache vs IDistributedCache vs IMemoryCache

### 3. Caching Strategies
- When to cache and what to cache
- Cache-aside pattern
- Write-through and write-behind caching
- Cache warming and preloading
- Time-based vs event-based invalidation
- Distributed caching with Redis

### 4. Database Performance
- Entity Framework Core performance tips
- Query optimization and LINQ best practices
- Avoiding N+1 queries
- Compiled queries
- Connection pooling
- Database indexing strategies
- Using AsNoTracking effectively

### 5. API Performance Patterns
- Response compression
- HTTP/2 and HTTP/3 benefits
- Pagination and filtering
- Partial responses (field selection)
- ETags and conditional requests
- Rate limiting and throttling

### 6. Asynchronous Programming
- async/await best practices
- Avoiding synchronous blocking
- ValueTask optimization
- Channels for producer-consumer scenarios
- Parallel processing patterns

### 7. Memory Management
- Reducing allocations
- ArrayPool and MemoryPool
- Span<T> and Memory<T>
- String optimization
- Object pooling
- Garbage collection tuning

## 🛠️ Prerequisites

- Completed Module 02 (ASP.NET Core Fundamentals)
- Completed Module 04 (Entity Framework Core)
- Completed Module 10 (.NET Aspire Deep Dive)
- Understanding of async/await
- Basic understanding of memory concepts

## 📖 Key Concepts

### HybridCache Architecture

```
Request → Check L1 (Memory) → Cache Hit? → Return
             ↓ Cache Miss
          Check L2 (Redis) → Cache Hit? → Return + Populate L1
             ↓ Cache Miss
          Execute (DB/API) → Populate L2 + L1 → Return
```

### Performance Metrics
- **Throughput**: Requests per second (RPS)
- **Latency**: Response time (p50, p95, p99)
- **Memory**: Working set and allocations
- **CPU**: Utilization percentage
- **I/O**: Database and network calls

## 💻 Hands-on Exercises

### Exercise 1: Implement HybridCache
Create an API with HybridCache:
- Set up HybridCache with Redis
- Implement basic caching for API responses
- Measure cache hit rates
- Compare performance with and without caching

### Exercise 2: Database Query Optimization
Optimize a slow API:
- Identify N+1 query problems
- Apply eager loading
- Use compiled queries
- Add appropriate indexes
- Measure query performance

### Exercise 3: Implement Response Compression
- Add Brotli/Gzip compression
- Measure payload size reduction
- Test performance impact
- Configure compression levels

### Exercise 4: Build a High-Throughput Pipeline
Use System.Threading.Channels:
- Create producer-consumer pattern
- Process messages efficiently
- Implement backpressure handling
- Measure throughput improvements

### Exercise 5: Memory Optimization
- Profile memory allocations
- Use ArrayPool for buffers
- Implement object pooling
- Use Span<T> for string manipulation
- Measure allocation reduction

## 📝 Sample Code Structure

```
performance-demo/
├── src/
│   ├── PerformanceApi/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   │   ├── CachingService.cs
│   │   │   └── OptimizedDataService.cs
│   │   ├── Infrastructure/
│   │   │   ├── HybridCacheSetup.cs
│   │   │   └── CompressionSetup.cs
│   │   └── Program.cs
│   └── PerformanceApi.Benchmarks/
│       └── ApiBenchmarks.cs
├── AppHost/
└── k6-scripts/
    └── load-test.js
```

## 🔧 HybridCache Implementation

### Setup HybridCache

```csharp
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);

// Add HybridCache with Redis L2
builder.AddRedisDistributedCache("redis");

builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1024 * 1024; // 1 MB
    options.MaximumKeyLength = 512;
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(1)
    };
});

var app = builder.Build();
app.Run();
```

### Using HybridCache

```csharp
public class ProductService
{
    private readonly HybridCache _cache;
    private readonly ApplicationDbContext _context;

    public ProductService(HybridCache cache, ApplicationDbContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task<Product?> GetProductAsync(int id, CancellationToken ct)
    {
        return await _cache.GetOrCreateAsync(
            $"product:{id}",
            async cancel =>
            {
                return await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id, cancel);
            },
            cancellationToken: ct
        );
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(
        string category, 
        CancellationToken ct)
    {
        return await _cache.GetOrCreateAsync(
            $"products:category:{category}",
            async cancel =>
            {
                return await _context.Products
                    .AsNoTracking()
                    .Where(p => p.Category == category)
                    .ToListAsync(cancel);
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(2)
            },
            cancellationToken: ct
        );
    }
}
```

### EF Core Query Optimization

```csharp
// Bad: N+1 queries
var orders = await _context.Orders.ToListAsync();
foreach (var order in orders)
{
    // Each iteration causes a database query
    var customer = await _context.Customers
        .FindAsync(order.CustomerId);
}

// Good: Eager loading
var orders = await _context.Orders
    .Include(o => o.Customer)
    .AsNoTracking()
    .ToListAsync();

// Better: Compiled query for repeated use
private static readonly Func<ApplicationDbContext, int, Task<Order?>> 
    GetOrderById = EF.CompileAsyncQuery(
        (ApplicationDbContext context, int id) =>
            context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == id)
    );

public async Task<Order?> GetOrderAsync(int id)
{
    return await GetOrderById(_context, id);
}
```

## 📊 Performance Testing

### BenchmarkDotNet Example

```csharp
using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class StringConcatBenchmark
{
    private const int Iterations = 1000;

    [Benchmark(Baseline = true)]
    public string StringConcatenation()
    {
        string result = "";
        for (int i = 0; i < Iterations; i++)
        {
            result += i.ToString();
        }
        return result;
    }

    [Benchmark]
    public string StringBuilder()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < Iterations; i++)
        {
            sb.Append(i);
        }
        return sb.ToString();
    }

    [Benchmark]
    public string StringCreate()
    {
        return string.Create(Iterations * 4, Iterations, (span, count) =>
        {
            // Optimized string building
        });
    }
}
```

## 📚 Additional Resources

- [.NET Performance Tips](https://learn.microsoft.com/dotnet/core/performance/)
- [HybridCache Documentation](https://learn.microsoft.com/aspnet/core/performance/caching/hybrid)
- [EF Core Performance](https://learn.microsoft.com/ef/core/performance/)
- [BenchmarkDotNet](https://benchmarkdotnet.org/)
- [Performance Best Practices](https://learn.microsoft.com/aspnet/core/performance/performance-best-practices)
- [Memory Management](https://learn.microsoft.com/dotnet/standard/garbage-collection/)

## 🎓 Best Practices

1. **Measure First**: Always profile before optimizing
2. **Cache Strategically**: Don't cache everything; cache what matters
3. **Use Async**: Leverage async/await for I/O operations
4. **Minimize Allocations**: Reduce garbage collection pressure
5. **Pool Resources**: Reuse expensive objects
6. **Optimize Queries**: Review all database queries
7. **Compress Responses**: Use Brotli compression
8. **Use CDNs**: Offload static content
9. **Implement Rate Limiting**: Protect against abuse
10. **Monitor Production**: Continuous performance monitoring

## ⚠️ Common Performance Issues

- **Synchronous blocking**: Using .Result or .Wait()
- **Excessive allocations**: Creating unnecessary objects
- **N+1 queries**: Not using eager loading
- **Missing indexes**: Slow database queries
- **No caching**: Repeated expensive operations
- **Large payloads**: Not using pagination or compression
- **Thread pool starvation**: Blocking I/O operations
- **Memory leaks**: Not disposing resources

## ✅ Module Checklist

- [ ] Understand performance fundamentals and metrics
- [ ] Implement HybridCache with Redis
- [ ] Optimize database queries with EF Core
- [ ] Add response compression
- [ ] Use System.Threading.Channels
- [ ] Profile and reduce memory allocations
- [ ] Write performance benchmarks
- [ ] Conduct load testing
- [ ] Complete all exercises

## 🚀 Next Module

Continue to [Module 14: Native AOT & Deployment](../14_deployment/) to learn about deploying your applications to the cloud.
