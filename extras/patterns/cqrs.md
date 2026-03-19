# CQRS — Command-Query Responsibility Segregation

## What Is CQRS?

CQRS is a pattern that **separates read operations (Queries) from write operations (Commands)** into distinct models. Think of it as splitting your application into a **Write Service** and a **Read Service**.

This separation is closer to how domain experts describe their requirements — they rarely think in terms of Create-Read-Update-Delete. Instead, they say things like _"submit a talk proposal"_, _"cancel an event"_, or _"show me upcoming sessions"_.

The pattern itself is simple, but it enables powerful architectural extensions.

## Commands vs. Queries

| Aspect           | Command                                         | Query                                                            |
| ---------------- | ----------------------------------------------- | ---------------------------------------------------------------- |
| **Purpose**      | Changes state (write)                           | Reads state (read)                                               |
| **Side effects** | Yes — modifies data                             | No — pure read, no side effects                                  |
| **Return value** | Confirmation / created ID                       | Data (DTOs, projections)                                         |
| **Naming**       | Imperative verb: `CreateEvent`, `CancelSession` | Descriptive: `GetEventById`, `ListUpcomingSessions`              |
| **Validation**   | Often required                                  | Rarely needed                                                    |
| **Optimization** | Optimized for consistency and business rules    | Optimized for read performance (denormalized views, projections) |

### The Key Insight

Because reads and writes have fundamentally different requirements, they benefit from **different models**:

- **Write model:** Enforces business rules, validates invariants, uses a rich domain model
- **Read model:** Optimized for display, can be denormalized, may use a different data store (SQL views, NoSQL, cache)

In many applications, there are multiple read models tailored to different use cases (list views, detail views, reports).

## MediatR Implementation with TechConf Domain

[MediatR](https://github.com/jbogard/MediatR) is a lightweight in-process message bus that makes implementing CQRS straightforward. It decouples the request (Command/Query) from its handler.

### Setup

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Register pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

var app = builder.Build();
```

### Command Example: Create an Event

```csharp
// Command — describes the intent
public record CreateEventCommand(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int MaxAttendees) : IRequest<Guid>;

// Handler — executes the intent
public class CreateEventHandler : IRequestHandler<CreateEventCommand, Guid>
{
    private readonly TechConfDbContext _db;

    public CreateEventHandler(TechConfDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateEventCommand request, CancellationToken ct)
    {
        var ev = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Location = request.Location,
            MaxAttendees = request.MaxAttendees,
            Status = EventStatus.Draft
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync(ct);

        return ev.Id;
    }
}
```

### Query Example: Get Event by ID

```csharp
// Query — describes what data is needed
public record GetEventByIdQuery(Guid Id) : IRequest<EventDetailDto?>;

// Read DTO — optimized for display, not for writes
public record EventDetailDto(
    Guid Id,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int MaxAttendees,
    int CurrentAttendees,
    string Status);

// Handler — fetches and projects data
public class GetEventByIdHandler : IRequestHandler<GetEventByIdQuery, EventDetailDto?>
{
    private readonly TechConfDbContext _db;

    public GetEventByIdHandler(TechConfDbContext db)
    {
        _db = db;
    }

    public async Task<EventDetailDto?> Handle(GetEventByIdQuery request, CancellationToken ct)
    {
        return await _db.Events
            .Where(e => e.Id == request.Id)
            .Select(e => new EventDetailDto(
                e.Id,
                e.Title,
                e.Description,
                e.StartDate,
                e.EndDate,
                e.Location,
                e.MaxAttendees,
                e.Registrations.Count,
                e.Status.ToString()))
            .FirstOrDefaultAsync(ct);
    }
}
```

### Minimal API Endpoints

```csharp
app.MapPost("/api/events", async (CreateEventCommand command, IMediator mediator) =>
{
    var id = await mediator.Send(command);
    return Results.Created($"/api/events/{id}", new { id });
})
.WithName("CreateEvent")
.WithTags("Events");

app.MapGet("/api/events/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetEventByIdQuery(id));
    return result is not null ? Results.Ok(result) : Results.NotFound();
})
.WithName("GetEventById")
.WithTags("Events");

app.MapGet("/api/events", async (
    [AsParameters] ListEventsQuery query,
    IMediator mediator) =>
{
    var result = await mediator.Send(query);
    return Results.Ok(result);
})
.WithName("ListEvents")
.WithTags("Events");
```

## Pipeline Behaviors

Pipeline behaviors are **middleware for your commands and queries**. They wrap every request, enabling cross-cutting concerns without polluting handler logic.

### Validation Behavior

Automatically validates every command/query that has a registered `IValidator<T>`:

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

### Logging Behavior

Logs every request with execution time for observability:

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Handling {RequestName}: {@Request}", requestName, request);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        _logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms",
            requestName, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
```

### Validator for CreateEventCommand

```csharp
public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Event must start in the future");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.MaxAttendees)
            .InclusiveBetween(1, 10000);
    }
}
```

## When Is CQRS Overkill vs. Valuable?

### CQRS Is Overkill When…

- Your app is a **simple CRUD** application where reads and writes look nearly identical
- You have a **small team** and adding the pattern increases onboarding time without clear benefit
- There is **no meaningful difference** between your read and write models
- You are building a **prototype or MVP** — optimize for speed of development first

### CQRS Is Valuable When…

- **Read and write workloads differ significantly** — e.g., many more reads than writes, or complex business rules on writes
- You need **different read models** for different consumers (API, dashboard, reports)
- You want to **scale reads and writes independently** (separate databases, read replicas)
- Your domain has **complex business logic** that benefits from a rich write model separated from simple read projections
- You want clean **pipeline behaviors** for validation, authorization, logging, and caching applied uniformly
- You are evolving toward **event sourcing** (CQRS is a natural stepping stone)

### Pragmatic Adoption

You do not have to go all-in. A pragmatic approach:

1. **Start simple** — use the same database and the same `DbContext` for reads and writes
2. **Separate the models** — use Commands/Queries with MediatR, but read and write from the same tables
3. **Optimize reads later** — introduce read-optimized views, projections, or a separate read store only when performance demands it
4. **Add event sourcing only if needed** — most applications never reach this stage, and that is perfectly fine

```
Level 0:  Controller → Service → Repository  (no CQRS)
Level 1:  Commands & Queries → Handlers → Same DB  (simple CQRS)
Level 2:  Separate Read/Write Models → Same DB  (model separation)
Level 3:  Separate Read/Write Databases  (full CQRS)
Level 4:  Event Sourcing + Projections  (advanced)
```

> **Most applications benefit from Level 1–2.** Do not jump to Level 3–4 unless you have a clear, measurable need.

## Resources

- [Martin Fowler — CQRS](https://martinfowler.com/bliki/CQRS.html)
- [MediatR Wiki](https://github.com/jbogard/MediatR/wiki)
- [Microsoft — CQRS Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Jimmy Bogard — Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/)
