# Lesson 05: Middleware Pipeline

## Table of Contents

1. [Introduction](#introduction)
2. [The Problem: Cross-Cutting Concerns](#the-problem-cross-cutting-concerns)
3. [What is Middleware?](#what-is-middleware)
4. [The Request/Response Pipeline](#the-requestresponse-pipeline)
5. [How Middleware Works](#how-middleware-works)
6. [Built-in Middleware](#built-in-middleware)
7. [Middleware Order and Why It Matters](#middleware-order-and-why-it-matters)
8. [Creating Custom Middleware](#creating-custom-middleware)
9. [Short-Circuiting the Pipeline](#short-circuiting-the-pipeline)
10. [Terminal Middleware](#terminal-middleware)
11. [Conditional Middleware](#conditional-middleware)
12. [Practical Examples](#practical-examples)
13. [Best Practices](#best-practices)
14. [Common Mistakes to Avoid](#common-mistakes-to-avoid)
15. [Exercises](#exercises)

## Introduction

Middleware is the foundation of request processing in ASP.NET Core. Every request flows through a pipeline of middleware components, each with a specific responsibility—from logging and authentication to routing and response generation.

### Learning Objectives

After completing this lesson, you will be able to:

- Explain what middleware is and how the pipeline works
- Understand the request/response flow through middleware
- Use built-in middleware components correctly
- Create custom middleware (inline and class-based)
- Understand why middleware order matters
- Implement short-circuiting and terminal middleware
- Apply conditional middleware based on environment or conditions
- Follow middleware best practices

### What You'll Build

You'll enhance the Chuck Norris Joke API with custom middleware:

- ✅ Request logging middleware
- ✅ Response time tracking middleware
- ✅ API key validation middleware
- ✅ Custom error handling middleware
- ✅ Custom headers middleware
- ✅ Rate limiting simulation

## The Problem: Cross-Cutting Concerns

### Example: Scattered Cross-Cutting Logic ❌

```csharp
// Program.cs - BAD PRACTICE
app.MapGet("/api/jokes", (IJokeService jokeService, HttpContext ctx) =>
{
    // Logging scattered in every endpoint
    Console.WriteLine($"[{DateTime.UtcNow}] GET /api/jokes");
    
    // API key validation repeated in every endpoint
    if (!ctx.Request.Headers.ContainsKey("X-API-Key"))
        return Results.Unauthorized();
    
    var startTime = DateTime.UtcNow;
    var jokes = jokeService.GetAll();
    var duration = DateTime.UtcNow - startTime;
    
    // Performance logging repeated everywhere
    Console.WriteLine($"Request took {duration.TotalMilliseconds}ms");
    
    return Results.Ok(jokes);
});

app.MapGet("/api/jokes/{id:int}", (int id, IJokeService jokeService, HttpContext ctx) =>
{
    // Same logging code repeated
    Console.WriteLine($"[{DateTime.UtcNow}] GET /api/jokes/{id}");
    
    // Same API key validation repeated
    if (!ctx.Request.Headers.ContainsKey("X-API-Key"))
        return Results.Unauthorized();
    
    // Business logic mixed with infrastructure concerns
    var joke = jokeService.GetById(id);
    return joke is null ? Results.NotFound() : Results.Ok(joke);
});
```

### Problems

1. ❌ **Code duplication** - Same logic repeated in every endpoint
2. ❌ **Hard to maintain** - Changes require updating all endpoints
3. ❌ **Mixed concerns** - Infrastructure mixed with business logic
4. ❌ **Error-prone** - Easy to forget adding logging or validation
5. ❌ **Difficult to test** - Cross-cutting concerns tightly coupled

## What is Middleware?

**Middleware** is software that sits between the incoming HTTP request and your application logic. Each middleware component:

- Receives the HTTP request
- Can perform operations on the request
- **Decides** whether to pass the request to the next middleware
- Can perform operations on the response
- Returns the response back through the chain

### Key Characteristics

- 🔗 **Chained together** - Forms a pipeline
- 🎯 **Single responsibility** - Each handles one concern
- ⚡ **Ordered execution** - Order matters!
- 🔄 **Bidirectional** - Processes both request and response
- 🛑 **Can short-circuit** - Can stop the pipeline early

## The Request/Response Pipeline

The middleware pipeline is a chain of components that process requests and responses.

### Visual Pipeline Flow

```
Incoming Request
     |
     v
┌────────────────────────────┐
│  Exception Handler         │ <-- Catches errors from all below
└───────────┬────────────────┘
            |
            v
┌────────────────────────────┐
│  Request Logging           │ <-- Logs all requests
└───────────┬────────────────┘
            |
            v
┌────────────────────────────┐
│  Authentication            │ <-- Verifies identity
└───────────┬────────────────┘
            |
            v
┌────────────────────────────┐
│  Authorization             │ <-- Checks permissions
└───────────┬────────────────┘
            |
            v
┌────────────────────────────┐
│  CORS                      │ <-- Cross-origin checks
└───────────┬────────────────┘
            |
            v
┌────────────────────────────┐
│  Routing                   │ <-- Matches endpoints
└───────────┬────────────────┘
            |
            v
┌────────────────────────────┐
│  Endpoint (Your API)       │ <-- Your business logic
└───────────┬────────────────┘
            |
            v
     Response flows back up
```

### Request and Response Flow

```
Request  ──────────────────────────────────────►
         Middleware 1 → Middleware 2 → Middleware 3
Response ◄──────────────────────────────────────
```

Each middleware can:
1. **Process the request** before calling next
2. **Call next()** to pass to next middleware
3. **Process the response** after next returns
4. **Short-circuit** by NOT calling next

## How Middleware Works

### The `next()` Pattern

```csharp
app.Use(async (context, next) =>
{
    // ⬇️ Request processing (going down the pipeline)
    Console.WriteLine("Before next - Request processing");
    
    // Call the next middleware in the pipeline
    await next(context);
    
    // ⬆️ Response processing (going back up the pipeline)
    Console.WriteLine("After next - Response processing");
});
```

### Complete Example with Multiple Middleware

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Middleware 1
app.Use(async (context, next) =>
{
    Console.WriteLine("M1: Before");
    await next(context);
    Console.WriteLine("M1: After");
});

// Middleware 2
app.Use(async (context, next) =>
{
    Console.WriteLine("M2: Before");
    await next(context);
    Console.WriteLine("M2: After");
});

// Terminal middleware (endpoint)
app.MapGet("/", () => 
{
    Console.WriteLine("Endpoint: Processing");
    return "Hello World!";
});

app.Run();

// Output when you visit /:
// M1: Before
// M2: Before
// Endpoint: Processing
// M2: After
// M1: After
```

### Visualization of Flow

```
Request arrives
    |
    v
M1: Before ────────┐
    |              │
    v              │
M2: Before ────┐   │
    |          │   │
    v          │   │
Endpoint       │   │
    |          │   │
    v          │   │
M2: After  ◄───┘   │
    |              │
    v              │
M1: After  ◄───────┘
    |
    v
Response sent
```

## Built-in Middleware

ASP.NET Core provides many built-in middleware components. Here are the most common:

### Common Built-in Middleware

```csharp
var app = builder.Build();

// 1. Exception handling - MUST be first!
app.UseExceptionHandler("/error");

// 2. HTTPS redirection
app.UseHttpsRedirection();

// 3. Static files (wwwroot)
app.UseStaticFiles();

// 4. Routing - matches endpoints
app.UseRouting();

// 5. CORS - Cross-Origin Resource Sharing
app.UseCors();

// 6. Authentication - Who are you?
app.UseAuthentication();

// 7. Authorization - What can you do?
app.UseAuthorization();

// 8. Map endpoints - Terminal middleware
app.MapControllers();
app.MapGet("/", () => "Hello");

app.Run();
```

### Middleware Reference Table

| Middleware | Purpose | Position |
|-----------|---------|----------|
| `UseExceptionHandler()` | Global error handling | First |
| `UseHttpsRedirection()` | Redirect HTTP to HTTPS | Early |
| `UseStaticFiles()` | Serve static files | Before routing |
| `UseRouting()` | Enable endpoint routing | Middle |
| `UseCors()` | Handle CORS requests | After routing |
| `UseAuthentication()` | Verify identity | After CORS |
| `UseAuthorization()` | Check permissions | After authentication |
| `UseSession()` | Enable session state | After routing |
| `UseResponseCaching()` | Cache responses | Early |
| `UseResponseCompression()` | Compress responses | Early |
| `MapControllers()`/`MapGet()` | Define endpoints | Last |

## Middleware Order and Why It Matters

**Order is critical!** Middleware executes in the order you add it.

### Example: Wrong Order ❌

```csharp
// BAD - Authorization before Authentication
app.UseAuthorization();  // ❌ Can't authorize if not authenticated yet!
app.UseAuthentication(); // ❌ Too late!

app.MapGet("/secure", () => "Secret").RequireAuthorization();
```

### Example: Correct Order ✅

```csharp
// GOOD - Authentication before Authorization
app.UseAuthentication(); // ✅ First, identify the user
app.UseAuthorization();  // ✅ Then, check permissions

app.MapGet("/secure", () => "Secret").RequireAuthorization();
```

### Visual: Order Matters

```
❌ WRONG ORDER:
Request → Authorization (No user yet!) → Authentication → Endpoint

✅ CORRECT ORDER:
Request → Authentication → Authorization → Endpoint
```

### Recommended Order Template

```csharp
var app = builder.Build();

// 1. Error handling (catches all errors below)
app.UseExceptionHandler("/error");

// 2. HTTPS redirection
app.UseHttpsRedirection();

// 3. Static files (if using)
// app.UseStaticFiles();

// 4. Routing
app.UseRouting();

// 5. CORS (after routing)
// app.UseCors("MyPolicy");

// 6. Authentication
// app.UseAuthentication();

// 7. Authorization
// app.UseAuthorization();

// 8. Custom middleware
// app.UseMiddleware<RequestLoggingMiddleware>();

// 9. Endpoints (terminal middleware)
app.MapControllers();
app.MapGet("/", () => "Hello");

app.Run();
```

## Creating Custom Middleware

There are two ways to create custom middleware: **inline** and **class-based**.

### 1. Inline Middleware (Simple)

Use `app.Use()` for simple, one-off middleware.

```csharp
app.Use(async (context, next) =>
{
    // Do something before
    Console.WriteLine($"Request: {context.Request.Path}");
    
    // Call next middleware
    await next(context);
    
    // Do something after
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});
```

### 2. Class-Based Middleware (Reusable)

For complex or reusable middleware, create a class.

#### Pattern 1: Convention-Based

```csharp
// Middleware/RequestLoggingMiddleware.cs
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    // Constructor receives next middleware and dependencies
    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // InvokeAsync (or Invoke) is called for each request
    public async Task InvokeAsync(HttpContext context)
    {
        // Before next middleware
        _logger.LogInformation(
            "Incoming {Method} request to {Path}",
            context.Request.Method,
            context.Request.Path);

        // Call next middleware
        await _next(context);

        // After next middleware
        _logger.LogInformation(
            "Response {StatusCode} for {Path}",
            context.Response.StatusCode,
            context.Request.Path);
    }
}

// Extension method for easy registration
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
```

#### Usage

```csharp
// Program.cs
var app = builder.Build();

// Use your custom middleware
app.UseRequestLogging();

app.MapGet("/", () => "Hello");
app.Run();
```

## Short-Circuiting the Pipeline

Middleware can **short-circuit** by not calling `next()`, stopping the pipeline early.

### Example: API Key Validation

```csharp
app.Use(async (context, next) =>
{
    // Check for API key
    if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new 
        { 
            error = "API key is required" 
        });
        
        // Don't call next() - short-circuit!
        return;
    }

    // Validate API key
    if (apiKey != "my-secret-key")
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsJsonAsync(new 
        { 
            error = "Invalid API key" 
        });
        
        // Don't call next() - short-circuit!
        return;
    }

    // Valid API key - continue pipeline
    await next(context);
});
```

### Visual: Short-Circuiting

```
Normal Flow:
Request → M1 → M2 → M3 → Endpoint → M3 → M2 → M1 → Response

Short-Circuit at M2:
Request → M1 → M2 → (stop) → M1 → Response
                ↓
            (M2 writes response)
```

## Terminal Middleware

**Terminal middleware** doesn't call `next()` and always ends the pipeline. Endpoints are terminal middleware.

### Using `app.Run()`

```csharp
// Run() is terminal - always last
app.Run(async context =>
{
    await context.Response.WriteAsync("Request reached the end");
    // Note: No next() parameter - this is terminal
});
```

### Example: Custom 404 Handler

```csharp
// All other middleware and endpoints above...

// Catch-all terminal middleware
app.Run(async context =>
{
    context.Response.StatusCode = 404;
    await context.Response.WriteAsJsonAsync(new
    {
        error = "Not Found",
        path = context.Request.Path.Value,
        message = "The requested resource was not found"
    });
});
```

## Conditional Middleware

Apply middleware only in certain conditions or environments.

### Environment-Based

```csharp
// Only in Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
```

### Path-Based

```csharp
// Only for /api paths
app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/api"),
    appBuilder =>
    {
        appBuilder.UseMiddleware<ApiKeyValidationMiddleware>();
    });
```

### Method-Based

```csharp
// Only for specific HTTP methods
app.MapWhen(
    context => context.Request.Method == "POST",
    appBuilder =>
    {
        appBuilder.Use(async (context, next) =>
        {
            Console.WriteLine("This is a POST request");
            await next(context);
        });
    });
```

## Practical Examples

### Example 1: Response Time Tracking

```csharp
// Middleware/ResponseTimeMiddleware.cs
public class ResponseTimeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseTimeMiddleware> _logger;

    public ResponseTimeMiddleware(
        RequestDelegate next,
        ILogger<ResponseTimeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Start timer
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Process request
        await _next(context);

        // Stop timer
        stopwatch.Stop();

        // Add custom header with response time
        context.Response.Headers.Add(
            "X-Response-Time-Ms",
            stopwatch.ElapsedMilliseconds.ToString());

        // Log slow requests
        if (stopwatch.ElapsedMilliseconds > 1000)
        {
            _logger.LogWarning(
                "Slow request: {Method} {Path} took {Ms}ms",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
```

### Example 2: API Key Validation Middleware

```csharp
// Middleware/ApiKeyValidationMiddleware.cs
public class ApiKeyValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private const string API_KEY_HEADER = "X-API-Key";

    public ApiKeyValidationMiddleware(
        RequestDelegate next,
        IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip validation for certain paths (e.g., health check)
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        // Check for API key header
        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "API Key is missing",
                message = $"Please provide {API_KEY_HEADER} header"
            });
            return;
        }

        // Validate API key
        var validApiKey = _configuration["ApiSettings:ApiKey"];
        if (extractedApiKey != validApiKey)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Invalid API Key",
                message = "The provided API key is not valid"
            });
            return;
        }

        // Valid - continue pipeline
        await _next(context);
    }
}
```

### Example 3: Custom Error Handling Middleware

```csharp
// Middleware/ErrorHandlingMiddleware.cs
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new
        {
            error = exception.Message,
            statusCode = context.Response.StatusCode,
            // Only show stack trace in development
            stackTrace = _env.IsDevelopment() ? exception.StackTrace : null
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
```

### Example 4: Custom Headers Middleware

```csharp
// Middleware/CustomHeadersMiddleware.cs
public class CustomHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public CustomHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers before calling next
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "no-referrer");
            context.Response.Headers.Add("X-Powered-By", "Chuck Norris");
            
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
```

### Complete Chuck Norris API with Middleware

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<IJokeService, JokeService>();
builder.Services.AddSingleton<IJokeRepository, InMemoryJokeRepository>();

var app = builder.Build();

// 1. Error handling (first!)
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. Request logging
app.UseMiddleware<RequestLoggingMiddleware>();

// 3. Response time tracking
app.UseMiddleware<ResponseTimeMiddleware>();

// 4. Custom security headers
app.UseMiddleware<CustomHeadersMiddleware>();

// 5. API key validation (for /api routes only)
app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/api"),
    appBuilder => appBuilder.UseMiddleware<ApiKeyValidationMiddleware>());

// 6. Routing
app.UseRouting();

// Endpoints
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.MapGet("/api/jokes", (IJokeService jokeService) =>
{
    return Results.Ok(jokeService.GetAll());
});

app.MapGet("/api/jokes/{id:int}", (int id, IJokeService jokeService) =>
{
    var joke = jokeService.GetById(id);
    return joke is null ? Results.NotFound() : Results.Ok(joke);
});

app.MapPost("/api/jokes", (Joke joke, IJokeService jokeService) =>
{
    var created = jokeService.Create(joke);
    return Results.Created($"/api/jokes/{created.Id}", created);
});

app.Run();
```

### Testing with curl

```bash
# Health check (no API key required)
curl http://localhost:5000/health

# Get jokes (requires API key)
curl -H "X-API-Key: your-api-key-here" http://localhost:5000/api/jokes

# Get specific joke (see response time header)
curl -v -H "X-API-Key: your-api-key-here" http://localhost:5000/api/jokes/1

# Missing API key (returns 401)
curl http://localhost:5000/api/jokes

# Invalid API key (returns 403)
curl -H "X-API-Key: wrong-key" http://localhost:5000/api/jokes
```

## Best Practices

### ✅ DO

1. **Place error handling first**
   ```csharp
   app.UseExceptionHandler("/error"); // First!
   ```

2. **Follow the recommended order**
   ```csharp
   // Exception → HTTPS → Static → Routing → CORS → Auth → Endpoints
   ```

3. **Use class-based middleware for complex logic**
   ```csharp
   public class MyMiddleware { /* ... */ }
   ```

4. **Inject dependencies in constructor**
   ```csharp
   public MyMiddleware(RequestDelegate next, ILogger logger) { }
   ```

5. **Always await next()**
   ```csharp
   await _next(context); // Don't forget await!
   ```

6. **Use extension methods for registration**
   ```csharp
   public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder app)
       => app.UseMiddleware<MyMiddleware>();
   ```

7. **Add custom headers in OnStarting**
   ```csharp
   context.Response.OnStarting(() => { /* headers */ });
   ```

8. **Log important events**
   ```csharp
   _logger.LogInformation("Request processed");
   ```

### ❌ DON'T

1. **Don't modify response after next() unless necessary**
   ```csharp
   await _next(context);
   context.Response.StatusCode = 200; // ❌ May be too late!
   ```

2. **Don't forget to call next()**
   ```csharp
   // ❌ Pipeline stops here!
   public async Task InvokeAsync(HttpContext context)
   {
       // Do stuff but never call _next(context)
   }
   ```

3. **Don't put authorization before authentication**
   ```csharp
   app.UseAuthorization();   // ❌ Wrong order!
   app.UseAuthentication();  // ❌
   ```

4. **Don't add endpoints before routing**
   ```csharp
   app.MapGet("/", () => "Hello"); // ❌ Before UseRouting
   app.UseRouting();               // ❌
   ```

5. **Don't catch exceptions without logging**
   ```csharp
   catch (Exception) { } // ❌ Silent failure!
   ```

6. **Don't use service locator pattern**
   ```csharp
   var service = context.RequestServices.GetService<IMyService>(); // ❌
   // Use constructor injection instead ✅
   ```

## Common Mistakes to Avoid

### Mistake 1: Wrong Middleware Order

```csharp
// ❌ WRONG
app.UseRouting();
app.UseAuthentication(); // Too late!
```

```csharp
// ✅ CORRECT
app.UseAuthentication();
app.UseRouting();
```

### Mistake 2: Not Awaiting next()

```csharp
// ❌ WRONG
public async Task InvokeAsync(HttpContext context)
{
    _next(context); // Missing await!
}
```

```csharp
// ✅ CORRECT
public async Task InvokeAsync(HttpContext context)
{
    await _next(context);
}
```

### Mistake 3: Modifying Response After next()

```csharp
// ❌ WRONG - Response may already be started
await _next(context);
context.Response.Headers.Add("X-Custom", "Value");
```

```csharp
// ✅ CORRECT - Use OnStarting
context.Response.OnStarting(() =>
{
    context.Response.Headers.Add("X-Custom", "Value");
    return Task.CompletedTask;
});
await _next(context);
```

### Mistake 4: Blocking Operations

```csharp
// ❌ WRONG
public async Task InvokeAsync(HttpContext context)
{
    Thread.Sleep(1000); // Blocking!
    await _next(context);
}
```

```csharp
// ✅ CORRECT
public async Task InvokeAsync(HttpContext context)
{
    await Task.Delay(1000); // Async!
    await _next(context);
}
```

### Mistake 5: Capturing Services with Wrong Lifetime

```csharp
// ❌ WRONG - Captures scoped service in singleton middleware
public class MyMiddleware
{
    private readonly IMyService _service; // Scoped service!

    public MyMiddleware(RequestDelegate next, IMyService service)
    {
        _next = next;
        _service = service; // ❌ Captured!
    }
}
```

```csharp
// ✅ CORRECT - Request services from HttpContext
public async Task InvokeAsync(HttpContext context)
{
    var service = context.RequestServices.GetRequiredService<IMyService>();
    // Or inject in InvokeAsync parameters
}
```

## Exercises

### Exercise 1: Request Counter Middleware

Create middleware that tracks the total number of requests.

**Requirements:**
- Use a static counter or singleton service
- Add the request number to response headers: `X-Request-Number`
- Log every 10th request

<details>
<summary>Solution</summary>

```csharp
public class RequestCounterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestCounterMiddleware> _logger;
    private static long _requestCount = 0;

    public RequestCounterMiddleware(
        RequestDelegate next,
        ILogger<RequestCounterMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestNumber = Interlocked.Increment(ref _requestCount);
        
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add("X-Request-Number", requestNumber.ToString());
            return Task.CompletedTask;
        });

        if (requestNumber % 10 == 0)
        {
            _logger.LogInformation("Processed {Count} requests", requestNumber);
        }

        await _next(context);
    }
}
```
</details>

### Exercise 2: Maintenance Mode Middleware

Create middleware that returns a 503 response when the app is in maintenance mode.

**Requirements:**
- Check a configuration setting: `MaintenanceMode:Enabled`
- Short-circuit if enabled
- Return JSON with maintenance message
- Allow `/health` endpoint to work

<details>
<summary>Solution</summary>

```csharp
public class MaintenanceModeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public MaintenanceModeMiddleware(
        RequestDelegate next,
        IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Allow health checks
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        var isMaintenanceMode = _configuration
            .GetValue<bool>("MaintenanceMode:Enabled", false);

        if (isMaintenanceMode)
        {
            context.Response.StatusCode = 503;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Service Unavailable",
                message = "The API is currently under maintenance. Please try again later.",
                retryAfter = "1 hour"
            });
            return;
        }

        await _next(context);
    }
}
```
</details>

### Exercise 3: Rate Limiting Middleware (Simplified)

Create basic rate limiting middleware that allows max 5 requests per IP per minute.

**Requirements:**
- Track requests per IP address
- Return 429 Too Many Requests if exceeded
- Use `ConcurrentDictionary` for thread safety
- Clear old entries periodically

<details>
<summary>Solution</summary>

```csharp
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, (int Count, DateTime ResetTime)> _requestCounts = new();
    private const int MaxRequests = 5;
    private static readonly TimeSpan TimeWindow = TimeSpan.FromMinutes(1);

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var now = DateTime.UtcNow;

        var (count, resetTime) = _requestCounts.GetOrAdd(
            ipAddress,
            _ => (0, now.Add(TimeWindow)));

        // Reset if time window passed
        if (now >= resetTime)
        {
            count = 0;
            resetTime = now.Add(TimeWindow);
        }

        count++;

        // Update dictionary
        _requestCounts[ipAddress] = (count, resetTime);

        // Check limit
        if (count > MaxRequests)
        {
            context.Response.StatusCode = 429;
            context.Response.Headers.Add("Retry-After", 
                ((int)(resetTime - now).TotalSeconds).ToString());
            
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Too Many Requests",
                message = $"Rate limit exceeded. Max {MaxRequests} requests per minute.",
                retryAfter = resetTime
            });
            return;
        }

        await _next(context);
    }
}
```
</details>

### Exercise 4: Correlation ID Middleware

Create middleware that adds a correlation ID to each request for tracking.

**Requirements:**
- Generate or read `X-Correlation-ID` from request header
- Add to response headers
- Store in HttpContext.Items for use in other middleware
- Log the correlation ID

<details>
<summary>Solution</summary>

```csharp
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get or generate correlation ID
        var correlationId = context.Request.Headers[CorrelationIdHeader]
            .FirstOrDefault() ?? Guid.NewGuid().ToString();

        // Store in HttpContext for use by other middleware/services
        context.Items[CorrelationIdHeader] = correlationId;

        // Add to response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add(CorrelationIdHeader, correlationId);
            return Task.CompletedTask;
        });

        // Log with correlation ID
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            [CorrelationIdHeader] = correlationId
        }))
        {
            _logger.LogInformation(
                "Request {Method} {Path} - CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                correlationId);

            await _next(context);

            _logger.LogInformation(
                "Response {StatusCode} - CorrelationId: {CorrelationId}",
                context.Response.StatusCode,
                correlationId);
        }
    }
}
```
</details>

### Exercise 5: Chuck Norris Quote of the Day

Add middleware that returns a random Chuck Norris joke for the root path `/`.

**Requirements:**
- Only handle `/` path
- Return plain text joke
- Don't call next() (terminal middleware)
- Use IJokeService from DI container

<details>
<summary>Solution</summary>

```csharp
public class ChuckNorrisQuoteMiddleware
{
    private readonly RequestDelegate _next;

    public ChuckNorrisQuoteMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJokeService jokeService)
    {
        // Only handle root path
        if (context.Request.Path != "/")
        {
            await _next(context);
            return;
        }

        // Get random joke
        var jokes = jokeService.GetAll();
        var random = new Random();
        var joke = jokes.ElementAt(random.Next(jokes.Count()));

        // Return as plain text
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync($"Chuck Norris Quote of the Day:\n\n{joke.Text}");
        
        // Terminal - don't call next()
    }
}
```
</details>

---

## Summary

In this lesson, you learned:

✅ **What middleware is** - Components that process HTTP requests/responses  
✅ **Pipeline flow** - How requests flow through middleware chain  
✅ **Built-in middleware** - Authentication, authorization, routing, etc.  
✅ **Middleware order** - Why order matters and the recommended sequence  
✅ **Custom middleware** - Creating inline and class-based middleware  
✅ **Short-circuiting** - Stopping the pipeline early  
✅ **Terminal middleware** - Endpoints that end the pipeline  
✅ **Conditional middleware** - Applying middleware selectively  
✅ **Best practices** - Common patterns and mistakes to avoid  

### Next Steps

- **Lesson 06**: Routing and Endpoint Patterns
- **Lesson 07**: Model Binding and Validation
- **Lesson 08**: Error Handling and Logging

### Additional Resources

- [ASP.NET Core Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/)
- [Custom Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write)
- [Middleware Order](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/#middleware-order)

---

**Happy coding! Remember: Middleware is like Chuck Norris—it's always in control of the pipeline!** 🥋
