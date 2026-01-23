# Lesson 03: Dependency Injection

## Table of Contents

1. [Introduction](#introduction)
2. [The Problem: Tight Coupling](#the-problem-tight-coupling)
3. [What is Dependency Injection?](#what-is-dependency-injection)
4. [Inversion of Control (IoC)](#inversion-of-control-ioc)
5. [ASP.NET Core's Built-in DI Container](#aspnet-cores-built-in-di-container)
6. [Service Lifetimes](#service-lifetimes)
7. [Registering Services](#registering-services)
8. [Constructor Injection Pattern](#constructor-injection-pattern)
9. [Refactoring the Chuck Norris API](#refactoring-the-chuck-norris-api)
10. [Best Practices](#best-practices)
11. [Common Mistakes to Avoid](#common-mistakes-to-avoid)
12. [When to Use Which Lifetime](#when-to-use-which-lifetime)
13. [Exercises](#exercises)

## Introduction

Dependency Injection (DI) is one of the most important concepts in modern software development and a cornerstone of ASP.NET Core architecture. It's a technique that makes your code more testable, maintainable, and flexible.

### Learning Objectives

After completing this lesson, you will be able to:

- Explain what Dependency Injection is and why it matters
- Understand the difference between tight and loose coupling
- Register services with different lifetimes (Singleton, Scoped, Transient)
- Use constructor injection to consume services
- Refactor tightly-coupled code to use DI
- Choose the appropriate service lifetime for different scenarios
- Follow DI best practices

### What You'll Build

You'll refactor the Chuck Norris Joke API from Lesson 02 to use:

- ✅ `IJokeService` interface for abstraction
- ✅ `JokeService` implementation with business logic
- ✅ `IJokeRepository` for data access
- ✅ `InMemoryJokeRepository` implementation
- ✅ Proper service registration with appropriate lifetimes
- ✅ Clean separation of concerns

## The Problem: Tight Coupling

Before we dive into DI, let's understand the problem it solves. Consider this typical code:

### Example: Tightly-Coupled Code

```csharp
// Program.cs - BAD PRACTICE ❌
var jokes = new List<Joke>
{
    new Joke { Id = 1, Text = "Chuck Norris counted to infinity... twice.", Category = "math" },
    new Joke { Id = 2, Text = "Chuck Norris can divide by zero.", Category = "math" }
};

app.MapGet("/api/jokes", () =>
{
    // Business logic directly in endpoint
    return jokes.OrderBy(j => j.Text);
});

app.MapGet("/api/jokes/{id:int}", (int id) =>
{
    // Data access logic directly in endpoint
    var joke = jokes.FirstOrDefault(j => j.Id == id);
    
    // Business logic directly in endpoint
    if (joke is null)
        return Results.NotFound(new { message = $"Joke with ID {id} not found." });
    
    return Results.Ok(joke);
});

app.MapPost("/api/jokes", (Joke joke) =>
{
    // Validation logic directly in endpoint
    if (string.IsNullOrWhiteSpace(joke.Text))
        return Results.BadRequest(new { message = "Joke text is required." });
    
    // Data access logic directly in endpoint
    joke.Id = jokes.Max(j => j.Id) + 1;
    jokes.Add(joke);
    
    return Results.Created($"/api/jokes/{joke.Id}", joke);
});
```

### Problems with This Approach

1. **❌ Not Testable**: How do you unit test the business logic without starting the web server?
2. **❌ Code Duplication**: Similar logic repeated across endpoints
3. **❌ Tight Coupling**: Endpoints are directly dependent on the `List<Joke>` data store
4. **❌ Hard to Change**: Want to switch to a database? You'll need to modify every endpoint
5. **❌ No Separation of Concerns**: Business logic, data access, and HTTP concerns all mixed together
6. **❌ Hard to Reuse**: Can't reuse the business logic in other contexts (background jobs, console apps, etc.)

### The Solution Preview

Here's what the same code looks like with DI:

```csharp
// Program.cs - GOOD PRACTICE ✅
builder.Services.AddSingleton<IJokeService, JokeService>();

var app = builder.Build();

app.MapGet("/api/jokes", (IJokeService service) => service.GetAll());

app.MapGet("/api/jokes/{id:int}", (int id, IJokeService service) =>
{
    var joke = service.GetById(id);
    return joke is null ? Results.NotFound() : Results.Ok(joke);
});

app.MapPost("/api/jokes", (Joke joke, IJokeService service) =>
{
    var result = service.Create(joke);
    return result.IsSuccess 
        ? Results.Created($"/api/jokes/{result.Value.Id}", result.Value)
        : Results.BadRequest(new { message = result.Error });
});
```

**Benefits:**
- ✅ **Testable**: Business logic in `JokeService` can be unit tested easily
- ✅ **DRY (Don't Repeat Yourself)**: Logic in one place
- ✅ **Loosely Coupled**: Endpoints depend on `IJokeService` interface, not implementation
- ✅ **Easy to Change**: Swap implementations without touching endpoints
- ✅ **Separation of Concerns**: Each layer has a single responsibility
- ✅ **Reusable**: Service can be used anywhere in the application

## What is Dependency Injection?

**Dependency Injection (DI)** is a design pattern in which an object receives (is "injected with") the other objects it depends on, rather than creating them itself.

### The Three Key Components

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  1. Service Interface (Contract)                            │
│     └─> Defines WHAT functionality is needed                │
│                                                             │
│  2. Service Implementation (Concrete Class)                 │
│     └─> Defines HOW the functionality works                 │
│                                                             │
│  3. Service Consumer (Client)                               │
│     └─> Uses the service through the interface              │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Example

```csharp
// 1. Service Interface (Contract)
public interface IJokeService
{
    IEnumerable<Joke> GetAll();
    Joke? GetById(int id);
    Joke Create(Joke joke);
}

// 2. Service Implementation (Concrete Class)
public class JokeService : IJokeService
{
    private readonly List<Joke> _jokes = new();
    
    public IEnumerable<Joke> GetAll() => _jokes;
    
    public Joke? GetById(int id) => _jokes.FirstOrDefault(j => j.Id == id);
    
    public Joke Create(Joke joke)
    {
        joke.Id = _jokes.Count > 0 ? _jokes.Max(j => j.Id) + 1 : 1;
        _jokes.Add(joke);
        return joke;
    }
}

// 3. Service Consumer (Minimal API endpoint)
app.MapGet("/api/jokes", (IJokeService service) => service.GetAll());
```

### Without DI vs. With DI

#### Without DI (Manual Instantiation)

```csharp
// Consumer creates its own dependencies ❌
app.MapGet("/api/jokes", () =>
{
    var service = new JokeService(); // Tight coupling!
    return service.GetAll();
});
```

**Problems:**
- Consumer controls dependency creation
- Hard to swap implementations
- Hard to test (can't mock the service)
- Consumer needs to know about service dependencies

#### With DI (Injection)

```csharp
// Register service
builder.Services.AddSingleton<IJokeService, JokeService>();

// Consumer receives its dependencies ✅
app.MapGet("/api/jokes", (IJokeService service) => service.GetAll());
```

**Benefits:**
- Framework controls dependency creation
- Easy to swap implementations (change one line in registration)
- Easy to test (inject a mock/fake)
- Consumer doesn't need to know about service dependencies

## Inversion of Control (IoC)

**Inversion of Control (IoC)** is the principle behind Dependency Injection. It means inverting the flow of control—instead of your code controlling dependencies, a framework (the IoC container) controls them.

### Traditional Control Flow

```csharp
// Your code controls everything ❌
public class JokeController
{
    private readonly JokeService _service;
    
    public JokeController()
    {
        var repository = new InMemoryRepository();
        var logger = new FileLogger();
        _service = new JokeService(repository, logger); // YOU create dependencies
    }
}
```

### Inverted Control Flow

```csharp
// Framework controls dependency creation ✅
public class JokeController
{
    private readonly IJokeService _service;
    
    public JokeController(IJokeService service) // Framework GIVES you dependencies
    {
        _service = service;
    }
}
```

### The IoC Container

The **IoC Container** (also called DI Container or Service Container) is responsible for:

1. **Registration**: Knowing which implementation to use for each interface
2. **Resolution**: Creating instances when needed
3. **Lifetime Management**: Deciding when to create/reuse/dispose instances
4. **Dependency Graph**: Resolving nested dependencies automatically

```
┌────────────────────────────────────────────────────────────┐
│                    IoC Container                           │
│                                                            │
│  Registration:                                             │
│    IJokeService        → JokeService                       │
│    IJokeRepository     → InMemoryJokeRepository            │
│    ILogger<T>          → Logger<T>                         │
│                                                            │
│  Resolution:                                               │
│    When JokeService is requested:                          │
│      1. Check if IJokeRepository is registered            │
│      2. Create InMemoryJokeRepository                     │
│      3. Create JokeService with repository                │
│      4. Return JokeService instance                        │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

## ASP.NET Core's Built-in DI Container

ASP.NET Core has a powerful, built-in DI container that's accessible through the `IServiceCollection` interface.

### Where Services Are Registered

Services are registered in `Program.cs` using the `WebApplicationBuilder.Services` property:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register services HERE (before building the app)
builder.Services.AddSingleton<IJokeService, JokeService>();
builder.Services.AddScoped<IJokeRepository, InMemoryJokeRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build(); // Build the app

// Configure middleware and endpoints HERE (after building)
app.MapGet("/api/jokes", (IJokeService service) => service.GetAll());

app.Run();
```

### Important Rules

1. ✅ **Always register services BEFORE calling `builder.Build()`**
2. ✅ **Always configure middleware and endpoints AFTER calling `builder.Build()`**
3. ❌ **Never call `builder.Services` after building the app**

## Service Lifetimes

Service lifetime determines when the container creates and disposes service instances. ASP.NET Core provides three lifetimes:

### 1. Transient (`AddTransient`)

**A new instance is created every time the service is requested.**

```csharp
builder.Services.AddTransient<IEmailService, EmailService>();
```

#### Lifetime Diagram

```
Request 1:                  Request 2:
  Endpoint A                  Endpoint C
     ├─ EmailService (new)       ├─ EmailService (new)
     └─ EmailService (new)       └─ EmailService (new)
  Endpoint B
     └─ EmailService (new)
```

#### When to Use

- ✅ **Stateless services** with no shared data
- ✅ **Lightweight services** (cheap to create)
- ✅ **Services that shouldn't share state** between consumers

#### Example

```csharp
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    
    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }
    
    public Task SendAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To}", to);
        // Send email logic
        return Task.CompletedTask;
    }
}

// Registration
builder.Services.AddTransient<IEmailService, EmailService>();
```

### 2. Scoped (`AddScoped`)

**One instance per HTTP request (per scope). The same instance is reused within the same request.**

```csharp
builder.Services.AddScoped<IJokeRepository, DbJokeRepository>();
```

#### Lifetime Diagram

```
Request 1:                  Request 2:
  Endpoint                    Endpoint
     ├─ JokeRepository (A)       └─ JokeRepository (C)
     │    (reused)
     └─ JokeService
          └─ JokeRepository (A) ← Same instance!
```

#### When to Use

- ✅ **Database contexts** (Entity Framework DbContext)
- ✅ **Unit of Work pattern** implementations
- ✅ **Request-specific state** that should be shared within a request
- ✅ **Most services** in a web application (default choice)

#### Example

```csharp
public interface IJokeRepository
{
    Task<IEnumerable<Joke>> GetAllAsync();
    Task<Joke?> GetByIdAsync(int id);
    Task<Joke> CreateAsync(Joke joke);
    Task SaveChangesAsync();
}

public class DbJokeRepository : IJokeRepository
{
    private readonly ApplicationDbContext _context;
    
    public DbJokeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Joke>> GetAllAsync()
    {
        return await _context.Jokes.ToListAsync();
    }
    
    public async Task<Joke?> GetByIdAsync(int id)
    {
        return await _context.Jokes.FindAsync(id);
    }
    
    public async Task<Joke> CreateAsync(Joke joke)
    {
        _context.Jokes.Add(joke);
        await SaveChangesAsync();
        return joke;
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

// Registration
builder.Services.AddScoped<IJokeRepository, DbJokeRepository>();
```

### 3. Singleton (`AddSingleton`)

**One instance for the entire application lifetime. Created on first request and reused for all subsequent requests.**

```csharp
builder.Services.AddSingleton<IJokeService, JokeService>();
```

#### Lifetime Diagram

```
Request 1:                  Request 2:                  Request 3:
  Endpoint A                  Endpoint B                  Endpoint C
     └─ JokeService (X)          └─ JokeService (X)          └─ JokeService (X)
                                                                     ↑
                                                        Same instance across ALL requests!
```

#### When to Use

- ✅ **Stateless services** with no request-specific data
- ✅ **Configuration objects**
- ✅ **Caching services**
- ✅ **Expensive-to-create services** that are thread-safe
- ✅ **In-memory data stores** (for demos/prototypes)

#### ⚠️ Warning: Thread Safety Required

Singleton services must be thread-safe because they're shared across all requests simultaneously!

#### Example

```csharp
public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiration);
    void Remove(string key);
}

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    
    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    public T? Get<T>(string key)
    {
        return _cache.TryGetValue(key, out T value) ? value : default;
    }
    
    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        _cache.Set(key, value, expiration);
    }
    
    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}

// Registration
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
```

### Lifetime Comparison Table

| Lifetime | Instance Per | Creation Time | Use Case |
|----------|--------------|---------------|----------|
| **Transient** | Request | Every injection | Stateless, lightweight services |
| **Scoped** | HTTP Request | Once per request | Database contexts, request state |
| **Singleton** | Application | First use | Configuration, caching, expensive services |

## Registering Services

There are several ways to register services with the DI container.

### Basic Registration

```csharp
// Interface → Implementation
builder.Services.AddSingleton<IJokeService, JokeService>();
builder.Services.AddScoped<IJokeRepository, DbJokeRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();
```

### Concrete Class Registration

```csharp
// Register a concrete class (no interface)
builder.Services.AddSingleton<JokeService>();

// Use it
app.MapGet("/api/jokes", (JokeService service) => service.GetAll());
```

### Factory Registration

```csharp
// Register with a factory function
builder.Services.AddSingleton<IJokeService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<JokeService>>();
    var config = sp.GetRequiredService<IConfiguration>();
    
    return new JokeService(logger, config);
});
```

### Instance Registration

```csharp
// Register an existing instance (always Singleton)
var jokes = new List<Joke>
{
    new Joke { Id = 1, Text = "Chuck Norris counted to infinity... twice." }
};
builder.Services.AddSingleton(jokes);

// Use it
app.MapGet("/api/jokes", (List<Joke> jokes) => jokes);
```

### Multiple Implementations

```csharp
// Register multiple implementations
builder.Services.AddSingleton<INotificationService, EmailNotificationService>();
builder.Services.AddSingleton<INotificationService, SmsNotificationService>();

// Resolve all implementations
app.MapPost("/api/notify", (IEnumerable<INotificationService> services) =>
{
    foreach (var service in services)
    {
        service.Notify("Hello!");
    }
    return Results.Ok();
});
```

## Constructor Injection Pattern

**Constructor Injection** is the recommended way to consume services. Dependencies are provided through the constructor.

### Basic Constructor Injection

```csharp
public class JokeService
{
    private readonly IJokeRepository _repository;
    private readonly ILogger<JokeService> _logger;
    
    // Dependencies injected through constructor
    public JokeService(IJokeRepository repository, ILogger<JokeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public IEnumerable<Joke> GetAll()
    {
        _logger.LogInformation("Getting all jokes");
        return _repository.GetAll();
    }
}
```

### Minimal API Parameter Injection

In Minimal APIs, you can inject services directly into endpoint handlers:

```csharp
// Single service injection
app.MapGet("/api/jokes", (IJokeService service) => service.GetAll());

// Multiple service injection
app.MapGet("/api/jokes/{id:int}", (int id, IJokeService service, ILogger<Program> logger) =>
{
    logger.LogInformation("Getting joke {Id}", id);
    var joke = service.GetById(id);
    return joke is null ? Results.NotFound() : Results.Ok(joke);
});

// Mix route parameters and services
app.MapPost("/api/jokes", (Joke joke, IJokeService service, IValidator<Joke> validator) =>
{
    var validation = validator.Validate(joke);
    if (!validation.IsValid)
        return Results.BadRequest(validation.Errors);
    
    var created = service.Create(joke);
    return Results.Created($"/api/jokes/{created.Id}", created);
});
```

### How ASP.NET Core Resolves Parameters

ASP.NET Core automatically determines where each parameter comes from:

```csharp
app.MapPost("/api/users/{userId}/jokes", 
    (int userId,              // From route
     Joke joke,               // From body (complex type)
     IJokeService service,    // From DI (registered service)
     ILogger<Program> logger, // From DI (registered service)
     HttpContext context) =>  // From DI (built-in service)
{
    // Handler logic
});
```

**Resolution Order:**
1. Route parameters (if name matches)
2. Query string (if simple type)
3. Request body (if complex type with `[FromBody]`)
4. DI Container (if type is registered)

## Refactoring the Chuck Norris API

Let's refactor the Chuck Norris Joke API from Lesson 02 to use proper Dependency Injection with a layered architecture.

### Step 1: Define Models

```csharp
// Models/Joke.cs
public class Joke
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Models/Result.cs
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    
    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
    
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

### Step 2: Create Repository Layer

```csharp
// Repositories/IJokeRepository.cs
public interface IJokeRepository
{
    IEnumerable<Joke> GetAll();
    Joke? GetById(int id);
    Joke? GetRandom();
    IEnumerable<Joke> Search(string searchText);
    Joke Add(Joke joke);
    bool Update(Joke joke);
    bool Delete(int id);
}

// Repositories/InMemoryJokeRepository.cs
public class InMemoryJokeRepository : IJokeRepository
{
    private readonly List<Joke> _jokes;
    private readonly ILogger<InMemoryJokeRepository> _logger;
    
    public InMemoryJokeRepository(ILogger<InMemoryJokeRepository> logger)
    {
        _logger = logger;
        _jokes = new List<Joke>
        {
            new Joke { Id = 1, Text = "Chuck Norris counted to infinity... twice.", Category = "math" },
            new Joke { Id = 2, Text = "Chuck Norris can divide by zero.", Category = "math" },
            new Joke { Id = 3, Text = "When Chuck Norris enters a room, he doesn't turn the lights on. He turns the dark off.", Category = "general" },
            new Joke { Id = 4, Text = "Chuck Norris can slam a revolving door.", Category = "general" },
            new Joke { Id = 5, Text = "Chuck Norris can kill two stones with one bird.", Category = "general" }
        };
    }
    
    public IEnumerable<Joke> GetAll()
    {
        _logger.LogInformation("Retrieving all jokes");
        return _jokes;
    }
    
    public Joke? GetById(int id)
    {
        _logger.LogInformation("Retrieving joke with ID {Id}", id);
        return _jokes.FirstOrDefault(j => j.Id == id);
    }
    
    public Joke? GetRandom()
    {
        if (_jokes.Count == 0) return null;
        var random = new Random();
        var index = random.Next(_jokes.Count);
        _logger.LogInformation("Retrieving random joke at index {Index}", index);
        return _jokes[index];
    }
    
    public IEnumerable<Joke> Search(string searchText)
    {
        _logger.LogInformation("Searching jokes with text: {SearchText}", searchText);
        return _jokes.Where(j => 
            j.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            j.Category.Contains(searchText, StringComparison.OrdinalIgnoreCase));
    }
    
    public Joke Add(Joke joke)
    {
        joke.Id = _jokes.Count > 0 ? _jokes.Max(j => j.Id) + 1 : 1;
        joke.CreatedAt = DateTime.UtcNow;
        _jokes.Add(joke);
        _logger.LogInformation("Added new joke with ID {Id}", joke.Id);
        return joke;
    }
    
    public bool Update(Joke joke)
    {
        var existing = GetById(joke.Id);
        if (existing is null)
        {
            _logger.LogWarning("Attempted to update non-existent joke with ID {Id}", joke.Id);
            return false;
        }
        
        existing.Text = joke.Text;
        existing.Category = joke.Category;
        _logger.LogInformation("Updated joke with ID {Id}", joke.Id);
        return true;
    }
    
    public bool Delete(int id)
    {
        var joke = GetById(id);
        if (joke is null)
        {
            _logger.LogWarning("Attempted to delete non-existent joke with ID {Id}", id);
            return false;
        }
        
        _jokes.Remove(joke);
        _logger.LogInformation("Deleted joke with ID {Id}", id);
        return true;
    }
}
```

### Step 3: Create Service Layer

```csharp
// Services/IJokeService.cs
public interface IJokeService
{
    IEnumerable<Joke> GetAll(string? category = null, string? sortBy = null);
    Joke? GetById(int id);
    Joke? GetRandom();
    IEnumerable<Joke> Search(string searchText);
    Result<Joke> Create(Joke joke);
    Result<Joke> Update(int id, Joke joke);
    Result<bool> Delete(int id);
}

// Services/JokeService.cs
public class JokeService : IJokeService
{
    private readonly IJokeRepository _repository;
    private readonly ILogger<JokeService> _logger;
    
    public JokeService(IJokeRepository repository, ILogger<JokeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public IEnumerable<Joke> GetAll(string? category = null, string? sortBy = null)
    {
        var jokes = _repository.GetAll();
        
        // Filter by category
        if (!string.IsNullOrWhiteSpace(category))
        {
            jokes = jokes.Where(j => j.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }
        
        // Sort
        jokes = sortBy?.ToLower() switch
        {
            "text" => jokes.OrderBy(j => j.Text),
            "category" => jokes.OrderBy(j => j.Category),
            "date" => jokes.OrderByDescending(j => j.CreatedAt),
            _ => jokes
        };
        
        return jokes;
    }
    
    public Joke? GetById(int id)
    {
        return _repository.GetById(id);
    }
    
    public Joke? GetRandom()
    {
        return _repository.GetRandom();
    }
    
    public IEnumerable<Joke> Search(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Enumerable.Empty<Joke>();
        }
        
        return _repository.Search(searchText);
    }
    
    public Result<Joke> Create(Joke joke)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(joke.Text))
        {
            return Result<Joke>.Failure("Joke text is required.");
        }
        
        if (joke.Text.Length < 10)
        {
            return Result<Joke>.Failure("Joke text must be at least 10 characters long.");
        }
        
        if (string.IsNullOrWhiteSpace(joke.Category))
        {
            joke.Category = "general";
        }
        
        var created = _repository.Add(joke);
        _logger.LogInformation("Created joke with ID {Id}", created.Id);
        
        return Result<Joke>.Success(created);
    }
    
    public Result<Joke> Update(int id, Joke joke)
    {
        var existing = _repository.GetById(id);
        if (existing is null)
        {
            return Result<Joke>.Failure($"Joke with ID {id} not found.");
        }
        
        // Validation
        if (string.IsNullOrWhiteSpace(joke.Text))
        {
            return Result<Joke>.Failure("Joke text is required.");
        }
        
        if (joke.Text.Length < 10)
        {
            return Result<Joke>.Failure("Joke text must be at least 10 characters long.");
        }
        
        joke.Id = id;
        _repository.Update(joke);
        _logger.LogInformation("Updated joke with ID {Id}", id);
        
        return Result<Joke>.Success(joke);
    }
    
    public Result<bool> Delete(int id)
    {
        var deleted = _repository.Delete(id);
        if (!deleted)
        {
            return Result<bool>.Failure($"Joke with ID {id} not found.");
        }
        
        _logger.LogInformation("Deleted joke with ID {Id}", id);
        return Result<bool>.Success(true);
    }
}
```

### Step 4: Register Services in Program.cs

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register services with appropriate lifetimes
builder.Services.AddSingleton<IJokeRepository, InMemoryJokeRepository>();
builder.Services.AddSingleton<IJokeService, JokeService>();

var app = builder.Build();

// GET all jokes (with optional filtering and sorting)
app.MapGet("/api/jokes", (IJokeService service, string? category, string? sortBy) =>
{
    var jokes = service.GetAll(category, sortBy);
    return Results.Ok(jokes);
});

// GET joke by ID
app.MapGet("/api/jokes/{id:int}", (int id, IJokeService service) =>
{
    var joke = service.GetById(id);
    return joke is null 
        ? Results.NotFound(new { message = $"Joke with ID {id} not found." })
        : Results.Ok(joke);
});

// GET random joke
app.MapGet("/api/jokes/random", (IJokeService service) =>
{
    var joke = service.GetRandom();
    return joke is null
        ? Results.NotFound(new { message = "No jokes available." })
        : Results.Ok(joke);
});

// SEARCH jokes
app.MapGet("/api/jokes/search", (string q, IJokeService service) =>
{
    if (string.IsNullOrWhiteSpace(q))
    {
        return Results.BadRequest(new { message = "Search query 'q' is required." });
    }
    
    var jokes = service.Search(q);
    return Results.Ok(jokes);
});

// POST create joke
app.MapPost("/api/jokes", (Joke joke, IJokeService service) =>
{
    var result = service.Create(joke);
    
    return result.IsSuccess
        ? Results.Created($"/api/jokes/{result.Value!.Id}", result.Value)
        : Results.BadRequest(new { message = result.Error });
});

// PUT update joke
app.MapPut("/api/jokes/{id:int}", (int id, Joke joke, IJokeService service) =>
{
    var result = service.Update(id, joke);
    
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.BadRequest(new { message = result.Error });
});

// DELETE joke
app.MapDelete("/api/jokes/{id:int}", (int id, IJokeService service) =>
{
    var result = service.Delete(id);
    
    return result.IsSuccess
        ? Results.NoContent()
        : Results.NotFound(new { message = result.Error });
});

app.Run();

// Make Program accessible for testing
public partial class Program { }
```

### Benefits of This Architecture

1. ✅ **Testability**: Each layer can be tested independently
2. ✅ **Maintainability**: Changes in one layer don't affect others
3. ✅ **Flexibility**: Easy to swap implementations (e.g., switch to a database)
4. ✅ **Reusability**: Services can be used in different contexts
5. ✅ **Separation of Concerns**: Each layer has a clear responsibility

## Best Practices

### 1. Always Depend on Abstractions (Interfaces)

```csharp
// BAD ❌
public class JokeController
{
    private readonly JokeService _service; // Depends on concrete class
    
    public JokeController(JokeService service)
    {
        _service = service;
    }
}

// GOOD ✅
public class JokeController
{
    private readonly IJokeService _service; // Depends on interface
    
    public JokeController(IJokeService service)
    {
        _service = service;
    }
}
```

### 2. Use Constructor Injection (Not Property or Method Injection)

```csharp
// BAD ❌
public class JokeService
{
    public IJokeRepository Repository { get; set; } // Property injection
}

// GOOD ✅
public class JokeService
{
    private readonly IJokeRepository _repository;
    
    public JokeService(IJokeRepository repository) // Constructor injection
    {
        _repository = repository;
    }
}
```

### 3. Store Dependencies in Readonly Fields

```csharp
public class JokeService
{
    private readonly IJokeRepository _repository; // readonly prevents reassignment
    
    public JokeService(IJokeRepository repository)
    {
        _repository = repository;
    }
}
```

### 4. Don't Use `new` for Dependencies

```csharp
// BAD ❌
public class JokeService
{
    private readonly IJokeRepository _repository = new InMemoryJokeRepository();
}

// GOOD ✅
public class JokeService
{
    private readonly IJokeRepository _repository;
    
    public JokeService(IJokeRepository repository) // Injected
    {
        _repository = repository;
    }
}
```

### 5. Register Most Services as Scoped

When in doubt, use `AddScoped`. It's safe for most scenarios:

```csharp
// Default choice for most services
builder.Services.AddScoped<IJokeService, JokeService>();
builder.Services.AddScoped<IJokeRepository, JokeRepository>();
```

### 6. Don't Inject Scoped Services into Singletons

```csharp
// BAD ❌
public class SingletonService // Registered as Singleton
{
    private readonly ScopedService _scoped; // Scoped dependency
    
    public SingletonService(ScopedService scoped)
    {
        _scoped = scoped; // This scoped instance will never be disposed!
    }
}
```

**Problem**: The scoped service becomes a "captive dependency" and is never disposed, potentially causing memory leaks.

**Solution**: Use `IServiceProvider` to resolve scoped services when needed:

```csharp
// GOOD ✅
public class SingletonService
{
    private readonly IServiceProvider _serviceProvider;
    
    public SingletonService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void DoWork()
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedService = scope.ServiceProvider.GetRequiredService<ScopedService>();
        // Use scopedService
    }
}
```

### 7. Validate Constructor Arguments

```csharp
public class JokeService
{
    private readonly IJokeRepository _repository;
    private readonly ILogger<JokeService> _logger;
    
    public JokeService(IJokeRepository repository, ILogger<JokeService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

### 8. Keep Constructors Simple

```csharp
// BAD ❌
public class JokeService
{
    private readonly IJokeRepository _repository;
    
    public JokeService(IJokeRepository repository)
    {
        _repository = repository;
        
        // Don't do work in constructor!
        var jokes = _repository.GetAll();
        ProcessJokes(jokes);
        InitializeCache();
    }
}

// GOOD ✅
public class JokeService
{
    private readonly IJokeRepository _repository;
    
    public JokeService(IJokeRepository repository)
    {
        _repository = repository; // Just store dependencies
    }
    
    public void Initialize()
    {
        // Separate method for initialization work
        var jokes = _repository.GetAll();
        ProcessJokes(jokes);
        InitializeCache();
    }
}
```

## Common Mistakes to Avoid

### 1. Service Locator Anti-Pattern

```csharp
// BAD ❌ - Service Locator
public class JokeService
{
    private readonly IServiceProvider _serviceProvider;
    
    public JokeService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void DoSomething()
    {
        var repo = _serviceProvider.GetRequiredService<IJokeRepository>();
        // Use repo
    }
}

// GOOD ✅ - Constructor Injection
public class JokeService
{
    private readonly IJokeRepository _repository;
    
    public JokeService(IJokeRepository repository)
    {
        _repository = repository;
    }
}
```

### 2. Circular Dependencies

```csharp
// BAD ❌
public class ServiceA
{
    public ServiceA(ServiceB serviceB) { }
}

public class ServiceB
{
    public ServiceB(ServiceA serviceA) { } // Circular dependency!
}
```

**Fix**: Refactor to break the circular dependency, possibly by introducing a third service or interface.

### 3. Too Many Constructor Parameters

```csharp
// BAD ❌ - Too many dependencies (code smell)
public class JokeService
{
    public JokeService(
        IJokeRepository repo,
        ILogger logger,
        IMapper mapper,
        IValidator validator,
        ICache cache,
        IEmailService email,
        ISmsService sms,
        INotificationService notification) // 8+ parameters!
    {
        // ...
    }
}
```

**Fix**: The class likely has too many responsibilities. Split it into smaller, focused classes.

### 4. Registering Services After Building

```csharp
// BAD ❌
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Too late! App is already built
builder.Services.AddSingleton<IJokeService, JokeService>(); // Exception!
```

## When to Use Which Lifetime

### Decision Flowchart

```
Is the service thread-safe and stateless?
    ├─ YES: Can it be reused across all requests?
    │   ├─ YES: Is it expensive to create?
    │   │   ├─ YES → Use SINGLETON
    │   │   └─ NO  → Use TRANSIENT or SCOPED
    │   └─ NO  → Use SCOPED
    └─ NO → Use SCOPED or TRANSIENT
```

### Common Scenarios

| Scenario | Lifetime | Reason |
|----------|----------|--------|
| Entity Framework `DbContext` | Scoped | Not thread-safe, per-request state |
| Repository (with DB) | Scoped | Depends on DbContext |
| Repository (in-memory) | Singleton | Thread-safe, shared data |
| Business logic service | Scoped | Default choice for most services |
| Logging service (`ILogger<T>`) | Singleton | Thread-safe, stateless |
| HTTP client (`IHttpClientFactory`) | Singleton | Expensive to create, thread-safe |
| Configuration (`IConfiguration`) | Singleton | Read-only, shared across app |
| Email service | Transient | Stateless, lightweight |
| Cache service | Singleton | Shared data across app |
| Validation service | Transient | Stateless, lightweight |

## Exercises

### Exercise 1: Add a Category Service

Create a `CategoryService` that manages joke categories.

**Requirements:**
1. Create `ICategory` interface with:
   - `GetAllCategories()`: Returns all unique categories
   - `GetJokesByCategory(string category)`: Returns jokes in a category
   - `GetCategoryCount()`: Returns dictionary of category names and joke counts

2. Implement `CategoryService` that uses `IJokeRepository`

3. Register the service with appropriate lifetime

4. Create endpoints:
   - `GET /api/categories`: Get all categories
   - `GET /api/categories/{name}/jokes`: Get jokes by category
   - `GET /api/categories/stats`: Get category statistics

**Bonus**: Add caching to avoid repeatedly querying the repository.

### Exercise 2: Add a Statistics Service

Create a `StatisticsService` to track API usage.

**Requirements:**
1. Create `IStatisticsService` interface with:
   - `RecordJokeView(int jokeId)`
   - `GetMostViewedJokes(int count)`
   - `GetTotalViews()`

2. Implement `StatisticsService` that tracks views in memory

3. Register with appropriate lifetime

4. Inject into joke endpoints to record views

5. Create endpoint:
   - `GET /api/statistics/top?count=10`: Get top viewed jokes

**Think about**: Which lifetime should you use? Why?

### Exercise 3: Implement a Logger Decorator

Create a logging decorator that wraps `IJokeService` and logs all method calls.

**Requirements:**
1. Create `LoggingJokeServiceDecorator` that implements `IJokeService`

2. Inject both `IJokeService` (the real implementation) and `ILogger`

3. Wrap each method call with logging:
   ```csharp
   public Joke? GetById(int id)
   {
       _logger.LogInformation("GetById called with id: {Id}", id);
       var result = _innerService.GetById(id);
       _logger.LogInformation("GetById returned: {Result}", result != null);
       return result;
   }
   ```

4. Register the decorator so it wraps the real service

**Hint**: You'll need to register the real service first, then register the decorator as `IJokeService`.

### Exercise 4: Add Validation Service

Create a validation service that validates jokes before creation.

**Requirements:**
1. Create `IJokeValidator` interface with:
   - `ValidationResult Validate(Joke joke)`

2. Create `ValidationResult` class with:
   - `bool IsValid`
   - `List<string> Errors`

3. Implement `JokeValidator` with rules:
   - Text is required
   - Text must be at least 10 characters
   - Text must contain "Chuck Norris"
   - Category must be one of: "math", "general", "physics", "technology"

4. Inject `IJokeValidator` into `JokeService`

5. Use it in `Create` and `Update` methods

**Bonus**: Make the allowed categories configurable via `IConfiguration`.

### Exercise 5: Refactor to Async

Refactor all services to use async/await.

**Requirements:**
1. Change all repository methods to async:
   - `GetAllAsync()`, `GetByIdAsync()`, etc.

2. Update `IJokeService` methods to async

3. Update all endpoint handlers to await service calls

4. Add artificial delays (for learning purposes):
   ```csharp
   public async Task<IEnumerable<Joke>> GetAllAsync()
   {
       await Task.Delay(100); // Simulate database query
       return _jokes;
   }
   ```

**Think about**: Does this change which lifetime you should use?

### Exercise 6: Service Lifetime Experiment

Create a simple experiment to understand service lifetimes.

**Requirements:**
1. Create `LifetimeService` with:
   - `Guid Id { get; }` (set in constructor)
   - `DateTime CreatedAt { get; }` (set in constructor)
   - `void LogInfo()` method

2. Register it three times with different lifetimes:
   - `AddSingleton<SingletonService>`
   - `AddScoped<ScopedService>`
   - `AddTransient<TransientService>`

3. Create an endpoint that injects all three and logs their IDs

4. Call the endpoint multiple times and observe the GUIDs

5. Document your findings: Which IDs stay the same? Which change?

### Exercise 7: Build a Joke Import Service

Create a service that imports jokes from a JSON file on startup.

**Requirements:**
1. Create `IJokeImportService` with:
   - `Task ImportFromFileAsync(string filePath)`

2. Create `jokes.json` file with sample jokes

3. Register the service

4. Use `IHostedService` or call the import method at startup

5. Inject `IJokeRepository` to add imported jokes

**Think about**: What lifetime should this service have? Why?

---

## Summary

In this lesson, you learned:

- ✅ What Dependency Injection is and why it matters
- ✅ The difference between tight and loose coupling
- ✅ How ASP.NET Core's DI container works
- ✅ Three service lifetimes: Singleton, Scoped, and Transient
- ✅ How to register services and use constructor injection
- ✅ How to refactor tightly-coupled code to use DI
- ✅ Best practices and common mistakes

**Key Takeaways:**

1. **Always depend on interfaces**, not concrete implementations
2. **Use constructor injection** for required dependencies
3. **Choose Scoped by default** unless you have a specific reason for Singleton or Transient
4. **Never inject Scoped into Singleton** (captive dependency)
5. **Keep constructors simple** and free of logic

### Next Steps

In the next lesson, you'll learn about:
- Middleware and the request pipeline
- Error handling and exception middleware
- CORS and security headers
- Request/response logging

---

**Additional Resources:**
- [Microsoft Docs: Dependency injection in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Microsoft Docs: Service lifetimes](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)
- [Martin Fowler: Inversion of Control Containers](https://martinfowler.com/articles/injection.html)
