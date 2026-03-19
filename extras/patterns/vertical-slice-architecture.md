# Vertical Slice Architecture (VSA)

## What Is Vertical Slice Architecture?

Vertical Slice Architecture organizes code by **feature** rather than by **technical layer**. Instead of grouping all controllers, services, and repositories into separate folders, each feature is self-contained — owning its endpoint, business logic, data access, and validation in one place.

The term was popularized by Jimmy Bogard (creator of MediatR and AutoMapper) as a reaction to the complexity that traditional layered architectures introduce when applications grow.

## Why It Matters

In a traditional layered architecture, adding a new feature means touching multiple layers:

```
Controllers/  →  Services/  →  Repositories/  →  Models/
```

Each layer adds abstractions that are often passed through unchanged. A simple "create an event" feature requires changes in 4+ files across 4+ folders.

**Vertical Slice Architecture** flips this:

```
Features/
  CreateEvent/    →  All code for creating an event lives here
  GetEventById/   →  All code for fetching a single event lives here
  ListEvents/     →  All code for listing events lives here
```

**Benefits:**
- **High cohesion** — everything related to a feature is in one place
- **Low coupling** — features do not depend on each other
- **Easier to understand** — a new developer can read one folder to understand one feature
- **Safer refactoring** — changing one feature does not break others
- **Scales with team size** — teams can work on different features without merge conflicts

## Comparison with Layered Architecture

| Aspect | Layered Architecture | Vertical Slice Architecture |
|--------|---------------------|-----------------------------|
| Organization | By technical concern (Controllers, Services, Repos) | By feature (CreateEvent, GetEvent) |
| Coupling | Layers depend on each other vertically | Slices are independent of each other |
| Adding a feature | Touch 4+ files in 4+ folders | Add 1 folder with all needed files |
| Shared abstractions | Many (generic repos, base services) | Few — duplication is preferred over wrong abstraction |
| When it shines | Small apps with uniform CRUD | Medium to large apps with diverse features |

## Feature Folder Structure (TechConf Domain)

```
src/
  TechConf.Api/
    Features/
      Events/
        CreateEvent/
          CreateEventCommand.cs
          CreateEventHandler.cs
          CreateEventValidator.cs
          CreateEventEndpoint.cs
        GetEventById/
          GetEventByIdQuery.cs
          GetEventByIdHandler.cs
          GetEventByIdEndpoint.cs
        ListEvents/
          ListEventsQuery.cs
          ListEventsHandler.cs
          ListEventsEndpoint.cs
        CancelEvent/
          CancelEventCommand.cs
          CancelEventHandler.cs
          CancelEventValidator.cs
          CancelEventEndpoint.cs
      Sessions/
        SubmitSession/
          SubmitSessionCommand.cs
          SubmitSessionHandler.cs
          SubmitSessionValidator.cs
          SubmitSessionEndpoint.cs
      Speakers/
        ...
    Common/
      Behaviors/
        ValidationBehavior.cs
        LoggingBehavior.cs
      Middleware/
        ExceptionHandlingMiddleware.cs
    Program.cs
```

> Shared concerns (cross-cutting behaviors, middleware) live in a `Common/` folder. Feature-specific code never goes there.

## A Complete Vertical Slice: Create Event

### 1. Command

```csharp
public record CreateEventCommand(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    int MaxAttendees) : IRequest<CreateEventResult>;

public record CreateEventResult(Guid Id, string Title, DateTime StartDate);
```

### 2. Validator (FluentValidation)

```csharp
public class CreateEventValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Start date must be in the future");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.MaxAttendees)
            .GreaterThan(0);
    }
}
```

### 3. Handler

```csharp
public class CreateEventHandler : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly TechConfDbContext _db;

    public CreateEventHandler(TechConfDbContext db)
    {
        _db = db;
    }

    public async Task<CreateEventResult> Handle(
        CreateEventCommand request, CancellationToken cancellationToken)
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
        await _db.SaveChangesAsync(cancellationToken);

        return new CreateEventResult(ev.Id, ev.Title, ev.StartDate);
    }
}
```

### 4. Endpoint (Minimal API)

```csharp
public static class CreateEventEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/events", async (
            CreateEventCommand command,
            IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/events/{result.Id}", result);
        })
        .WithName("CreateEvent")
        .WithTags("Events")
        .Produces<CreateEventResult>(StatusCodes.Status201Created)
        .ProducesValidationProblem();
    }
}
```

### 5. Registration in Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TechConfDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Register pipeline behaviors for cross-cutting concerns
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

var app = builder.Build();

// Map feature endpoints
CreateEventEndpoint.Map(app);
GetEventByIdEndpoint.Map(app);
ListEventsEndpoint.Map(app);
CancelEventEndpoint.Map(app);

app.Run();
```

## MediatR Integration

MediatR acts as an **in-process message bus** that decouples the "what" (Command/Query) from the "how" (Handler). It is the backbone of most VSA implementations.

**Key concepts:**
- `IRequest<TResponse>` — defines a command or query with an expected response
- `IRequestHandler<TRequest, TResponse>` — handles the request
- `IPipelineBehavior<TRequest, TResponse>` — middleware that wraps every request (cross-cutting concerns)

### Validation Pipeline Behavior

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
        CancellationToken cancellationToken)
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

## When to Use VSA vs. Clean Architecture vs. Simple Layered

| Criteria | Simple Layered | Clean Architecture | Vertical Slice |
|----------|---------------|-------------------|----------------|
| **App size** | Small / CRUD-heavy | Large / complex domain | Medium to large / diverse features |
| **Team size** | 1–3 developers | 3+ developers | Any size |
| **Feature diversity** | Uniform operations | Complex domain rules | Mixed complexity per feature |
| **Abstractions** | Few (controller → service → repo) | Many (entities, use cases, interfaces, DTOs) | Minimal — only what the feature needs |
| **Learning curve** | Low | High | Medium |
| **Refactoring cost** | Low (small app) / High (large app) | Medium | Low — features are isolated |
| **Testing** | Integration tests dominate | Unit tests via dependency inversion | Unit tests per slice |

### Rules of Thumb

- **Start with simple layered** for small apps or prototypes — do not over-engineer.
- **Consider VSA** when you notice that adding features means touching many files across many layers, or when different features have very different complexity levels.
- **Consider Clean Architecture** when you have a rich domain model with complex invariants that justify the abstraction cost.
- **VSA and Clean Architecture are not mutually exclusive** — you can organize slices around Clean Architecture's use-case layer.

> **The best architecture is the one your team can understand and maintain.** Start simple, refactor toward more structure when the pain of the current approach exceeds the cost of the refactoring.
