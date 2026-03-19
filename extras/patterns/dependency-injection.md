# Dependency Injection (DI) in ASP.NET Core

Since ASP.NET Core relies heavily on **Dependency Injection**, it is worth taking the time to understand the pattern and how it is used in practice.

## What Is Dependency Injection?

Dependency Injection is a design pattern where an object's dependencies are **provided from the outside** rather than created internally. This promotes loose coupling, testability, and separation of concerns.

In ASP.NET Core, DI is a **first-class citizen** — the built-in IoC (Inversion of Control) container manages the creation and lifetime of services throughout the application.

## Registering Services

Services are registered in `Program.cs` using the `IServiceCollection`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register services with the DI container
builder.Services.AddSingleton<ITimeService, DefaultTimeService>();
builder.Services.AddScoped<IEventRepository, EfEventRepository>();
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

var app = builder.Build();
app.Run();
```

## Service Lifetimes

Three lifetimes are available when registering a service:

| Method                            | Lifetime      | Behavior                                                                                                                                              |
| --------------------------------- | ------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| `AddSingleton<TService, TImpl>()` | **Singleton** | A single instance is created and shared across all requests and all consumers for the entire application lifetime.                                    |
| `AddScoped<TService, TImpl>()`    | **Scoped**    | One instance is created per HTTP request (scope). All consumers within the same request receive the same instance. A new request gets a new instance. |
| `AddTransient<TService, TImpl>()` | **Transient** | A new instance is created every time the service is requested from the container.                                                                     |

> **Choosing the right lifetime:**
>
> - Use `Singleton` for stateless services or shared caches
> - Use `Scoped` for services tied to a request (e.g., `DbContext`, repositories)
> - Use `Transient` for lightweight, stateless services where sharing is unnecessary

## Constructor Injection

The most common pattern — dependencies are injected via the class constructor:

```csharp
public class EventService
{
    private readonly IEventRepository _repository;
    private readonly ILogger<EventService> _logger;

    public EventService(IEventRepository repository, ILogger<EventService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
    {
        _logger.LogInformation("Fetching upcoming events");
        return await _repository.GetUpcomingAsync();
    }
}
```

## Minimal API Parameter Injection

In Minimal APIs, dependencies are injected directly as **endpoint handler parameters** — no constructor needed:

```csharp
app.MapGet("/api/events", async (IEventRepository repo) =>
{
    var events = await repo.GetAllAsync();
    return Results.Ok(events);
});

app.MapGet("/api/events/{id:guid}", async (
    Guid id,
    IEventRepository repo,
    ILogger<Program> logger) =>
{
    logger.LogInformation("Fetching event {EventId}", id);
    var ev = await repo.GetByIdAsync(id);
    return ev is not null ? Results.Ok(ev) : Results.NotFound();
});

app.MapPost("/api/events", async (
    CreateEventRequest request,
    IEventRepository repo,
    IValidator<CreateEventRequest> validator) =>
{
    var validation = validator.Validate(request);
    if (!validation.IsValid)
        return Results.ValidationProblem(validation.ToDictionary());

    var created = await repo.AddAsync(request.ToEvent());
    return Results.Created($"/api/events/{created.Id}", created);
});
```

> Minimal API parameter injection uses the **`[FromServices]`** attribute implicitly for registered services. You can also use `[AsParameters]` to group multiple parameters into a single object.

## Clean Code: Extension Method Pattern

When many services need to be registered, `Program.cs` can become cluttered. Use C# **extension methods** to group related registrations:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTechConfDataAccess(this IServiceCollection services)
    {
        services.AddScoped<IEventRepository, EfEventRepository>();
        services.AddScoped<ISpeakerRepository, EfSpeakerRepository>();
        services.AddScoped<ISessionRepository, EfSessionRepository>();
        services.AddScoped<IAttendeeRepository, EfAttendeeRepository>();

        return services;
    }

    public static IServiceCollection AddTechConfDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddSingleton<ISlugGenerator, SlugGenerator>();

        return services;
    }
}
```

Now `Program.cs` stays clean and readable:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTechConfDataAccess();
builder.Services.AddTechConfDomainServices();

var app = builder.Build();
app.Run();
```

## Keyed Services (.NET 8+)

Starting with .NET 8, you can register multiple implementations of the same interface using **keyed services**:

```csharp
builder.Services.AddKeyedScoped<INotificationService, EmailNotificationService>("email");
builder.Services.AddKeyedScoped<INotificationService, SmsNotificationService>("sms");

app.MapPost("/api/events/{id}/notify", async (
    Guid id,
    [FromKeyedServices("email")] INotificationService notifier) =>
{
    await notifier.SendAsync(id, "Event updated!");
    return Results.Ok();
});
```

## Pros and Cons

| Pros                                                 | Cons                                                                          |
| ---------------------------------------------------- | ----------------------------------------------------------------------------- |
| Easy to swap implementations (e.g., for testing)     | Unregistered dependencies cause runtime errors, not compile-time errors       |
| Promotes loose coupling and separation of concerns   | Can make it harder to trace where instances are created                       |
| Built-in to ASP.NET Core — no extra libraries needed | Overuse can lead to "constructor over-injection"                              |
| Supports different lifetimes for different needs     | Incorrect lifetime choices can cause subtle bugs (e.g., captive dependencies) |

> **Tip:** Enable `ValidateScopes` and `ValidateOnBuild` in development to catch common DI mistakes early:
>
> ```csharp
> builder.Host.UseDefaultServiceProvider(options =>
> {
>     options.ValidateScopes = true;
>     options.ValidateOnBuild = true;
> });
> ```
