# Day 2 Persistence, Validation & Error Handling Cheat Sheet

> **Focus:** the smallest set of ideas from [`docs/day2/`](../../docs/day2/) that you need to read, run, and extend an ASP.NET Core API that persists data, validates input, and returns consistent error responses.
>
> **Companion material:**
> - SQLite quick-start: [`../../demos/day2/TechConf.Sqlite.Api/`](../../demos/day2/TechConf.Sqlite.Api/)
> - PostgreSQL variant: [`../../demos/day2/TechConf.Postgres.Api/`](../../demos/day2/TechConf.Postgres.Api/)
> - Guided lab: [`../../labs/lab2-persistence-validation/`](../../labs/lab2-persistence-validation/)

## 1. Start here

From the repository root:

```bash
dotnet build demos/day2/TechConf.Sqlite.Api/TechConf.Sqlite.Api.csproj
dotnet run --project demos/day2/TechConf.Sqlite.Api/TechConf.Sqlite.Api.csproj
```

Then open:

- Scalar UI: `http://localhost:5100/scalar/v1`
- OpenAPI JSON: `http://localhost:5100/openapi/v1.json`
- Manual request file: [`../../demos/day2/TechConf.Sqlite.Api/requests.http`](../../demos/day2/TechConf.Sqlite.Api/requests.http)

When you want the PostgreSQL path that matches the lab more closely, use:

- [`../../demos/day2/README.md`](../../demos/day2/README.md)
- [`../../labs/lab2-persistence-validation/README.md`](../../labs/lab2-persistence-validation/README.md)

## 2. Day 2 map: concept -> doc -> code

| Concept | Course material | Where to see it in the demo |
| --- | --- | --- |
| Service wiring and startup pipeline | [`README.md`](../../docs/day2/README.md), [`01-entity-framework.md`](../../docs/day2/01-entity-framework.md), [`03-validation.md`](../../docs/day2/03-validation.md), [`04-problem-details.md`](../../docs/day2/04-problem-details.md) | [`TechConf.Sqlite.Api/Program.cs`](../../demos/day2/TechConf.Sqlite.Api/Program.cs), [`TechConf.Postgres.Api/Program.cs`](../../demos/day2/TechConf.Postgres.Api/Program.cs) |
| `DbContext` + configuration scanning | [`01-entity-framework.md`](../../docs/day2/01-entity-framework.md) | [`Data/AppDbContext.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/AppDbContext.cs), [`Data/AppDbContextFactory.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/AppDbContextFactory.cs) |
| Fluent API and relationships | [`01-entity-framework.md`](../../docs/day2/01-entity-framework.md) | [`Data/Configurations/EventConfiguration.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/Configurations/EventConfiguration.cs), [`SessionConfiguration.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/Configurations/SessionConfiguration.cs), [`SpeakerConfiguration.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/Configurations/SpeakerConfiguration.cs) |
| Migrations and seed data | [`01-entity-framework.md`](../../docs/day2/01-entity-framework.md) | [`Data/Migrations/`](../../demos/day2/TechConf.Sqlite.Api/Data/Migrations/), [`Data/DbSeeder.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/DbSeeder.cs), [`demos/day2/README.md`](../../demos/day2/README.md) |
| LINQ, projection, and eager loading | [`01-entity-framework.md`](../../docs/day2/01-entity-framework.md) | [`Repositories/EventRepository.cs`](../../demos/day2/TechConf.Sqlite.Api/Repositories/EventRepository.cs), [`Endpoints/EventEndpoints.cs`](../../demos/day2/TechConf.Sqlite.Api/Endpoints/EventEndpoints.cs) |
| Built-in validation + FluentValidation | [`03-validation.md`](../../docs/day2/03-validation.md) | [`Models/Dtos.cs`](../../demos/day2/TechConf.Sqlite.Api/Models/Dtos.cs), [`Validation/CreateEventValidator.cs`](../../demos/day2/TechConf.Sqlite.Api/Validation/CreateEventValidator.cs), [`Validation/UpdateEventValidator.cs`](../../demos/day2/TechConf.Sqlite.Api/Validation/UpdateEventValidator.cs) |
| Validation filter + endpoint metadata | [`03-validation.md`](../../docs/day2/03-validation.md) | [`Validation/ValidationFilter.cs`](../../demos/day2/TechConf.Sqlite.Api/Validation/ValidationFilter.cs), [`Endpoints/EventEndpoints.cs`](../../demos/day2/TechConf.Sqlite.Api/Endpoints/EventEndpoints.cs) |
| Problem Details + exception mapping | [`04-problem-details.md`](../../docs/day2/04-problem-details.md) | [`Program.cs`](../../demos/day2/TechConf.Sqlite.Api/Program.cs), [`Exceptions/GlobalExceptionHandler.cs`](../../demos/day2/TechConf.Sqlite.Api/Exceptions/GlobalExceptionHandler.cs), [`Exceptions/NotFoundException.cs`](../../demos/day2/TechConf.Sqlite.Api/Exceptions/NotFoundException.cs), [`requests.http`](../../demos/day2/TechConf.Sqlite.Api/requests.http) |

## 3. Project anatomy in 60 seconds

| File | Why it exists |
| --- | --- |
| [`Program.cs`](../../demos/day2/TechConf.Sqlite.Api/Program.cs) | Registers EF Core, validation, Problem Details, the repository, and the endpoint surface. |
| [`Data/AppDbContext.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/AppDbContext.cs) | The EF Core boundary: exposes `DbSet<>` properties and applies all model configurations. |
| [`Data/Configurations/*.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/Configurations/) | Holds Fluent API rules for keys, lengths, relationships, and delete behavior. |
| [`Data/Migrations/`](../../demos/day2/TechConf.Sqlite.Api/Data/Migrations/) | Tracks how the database schema evolves over time. |
| [`Data/DbSeeder.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/DbSeeder.cs) | Applies migrations and inserts demo data at startup. |
| [`Repositories/IEventRepository.cs`](../../demos/day2/TechConf.Sqlite.Api/Repositories/IEventRepository.cs) | Defines the async data-access contract used by the endpoints. |
| [`Repositories/EventRepository.cs`](../../demos/day2/TechConf.Sqlite.Api/Repositories/EventRepository.cs) | Contains the actual EF queries, projections, eager loading, and persistence code. |
| [`Models/Dtos.cs`](../../demos/day2/TechConf.Sqlite.Api/Models/Dtos.cs) | Defines request/response models, including Data Annotation rules used by built-in validation. |
| [`Validation/*.cs`](../../demos/day2/TechConf.Sqlite.Api/Validation/) | Adds FluentValidation rules and the reusable validation endpoint filter. |
| [`Exceptions/*.cs`](../../demos/day2/TechConf.Sqlite.Api/Exceptions/) | Centralizes domain exceptions and the global Problem Details handler. |
| [`requests.http`](../../demos/day2/TechConf.Sqlite.Api/requests.http) | Gives you quick success and failure requests without writing a frontend. |

**Mental model:** `Program.cs` wires the application, `Data/` shapes persistence, `Repositories/` ask questions of the database, `Validation/` rejects bad input, `Exceptions/` standardize failures, and `Endpoints/` keep the HTTP layer small.

## 4. `Program.cs`: the startup flow to memorize

The day 2 sample adds persistence and global error handling without changing the Minimal API feel:

```csharp
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEventRepository, EventRepository>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

await DbSeeder.MigrateAndSeedAsync(app.Services);
app.MapEventEndpoints();
```

What changed compared to day 1:

1. `AddDbContext<AppDbContext>(...)` brings EF Core into DI with a scoped lifetime.
2. `AddValidatorsFromAssemblyContaining<Program>()` discovers every `AbstractValidator<T>`.
3. `AddProblemDetails()` and `AddExceptionHandler<GlobalExceptionHandler>()` create a single error format for the whole API.
4. `UseExceptionHandler()` and `UseStatusCodePages()` make those error responses visible at runtime.
5. `DbSeeder.MigrateAndSeedAsync(...)` keeps the demo database ready to use.

**Important comparison:** the PostgreSQL sample is nearly identical. The key swap is `UseSqlite(...)` vs `UseNpgsql(...)`.

See:

- [`TechConf.Sqlite.Api/Program.cs`](../../demos/day2/TechConf.Sqlite.Api/Program.cs)
- [`TechConf.Postgres.Api/Program.cs`](../../demos/day2/TechConf.Postgres.Api/Program.cs)

## 5. EF Core essentials: `DbContext`, Fluent API, relationships

### `DbContext` is the center of persistence

The demo keeps the context intentionally small:

```csharp
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Speaker> Speakers => Set<Speaker>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

This is the pattern to remember:

- expose a `DbSet<>` for each aggregate you query directly
- keep `OnModelCreating` small by delegating rules to configuration classes
- let DI create one context per request

### Fluent API belongs in configuration classes

The sample uses one file per entity instead of stacking mapping rules inside `AppDbContext`.

One-to-many: `Event -> Sessions`

```csharp
builder.HasMany(e => e.Sessions)
    .WithOne(s => s.Event)
    .HasForeignKey(s => s.EventId)
    .OnDelete(DeleteBehavior.Cascade);
```

Many-to-many: `Session <-> Speakers`

```csharp
builder.HasMany(s => s.Speakers)
    .WithMany(sp => sp.Sessions)
    .UsingEntity(j => j.ToTable("SessionSpeakers"));
```

This keeps schema rules explicit and easy to find.

See:

- [`Data/Configurations/EventConfiguration.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/Configurations/EventConfiguration.cs)
- [`Data/Configurations/SessionConfiguration.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/Configurations/SessionConfiguration.cs)
- [`Data/Configurations/SpeakerConfiguration.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/Configurations/SpeakerConfiguration.cs)

**Scope note:** [`01-entity-framework.md`](../../docs/day2/01-entity-framework.md) goes further than the sample. It also covers pooling, split queries, pagination, JSON columns, `LeftJoin`, bulk updates, and seeding strategies. The runnable demo focuses on the subset you need first: context setup, relationships, migrations, LINQ, and N+1 avoidance.

## 6. Migrations and database workflow

The essential workflow is still short:

```bash
cd demos/day2/TechConf.Sqlite.Api
dotnet ef migrations add AddVenueSupport
dotnet ef database update
```

What to connect mentally:

- model change -> create a migration
- migration file -> versioned schema change in source control
- `database update` -> apply those changes to the actual database
- `DbSeeder.MigrateAndSeedAsync(...)` -> convenient startup hook for demos and workshops

The sample also includes [`AppDbContextFactory.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/AppDbContextFactory.cs) so EF CLI tooling can create the context outside the running app.

If you want the PostgreSQL lab workflow instead of SQLite, use:

- [`../../demos/day2/README.md`](../../demos/day2/README.md)
- [`../../labs/lab2-persistence-validation/README.md`](../../labs/lab2-persistence-validation/README.md)

## 7. Querying with LINQ without N+1 surprises

Day 2 is where query shape starts to matter.

The repository shows three habits worth copying:

### Use `AsNoTracking()` for read-only endpoints

```csharp
var query = db.Events.AsNoTracking().AsQueryable();
```

This avoids change-tracking overhead when you are just reading data.

### Load related data intentionally

```csharp
var evt = await db.Events
    .AsNoTracking()
    .Include(e => e.Sessions)
        .ThenInclude(s => s.Speakers)
    .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
```

This is the direct answer to the N+1 problem for the detail endpoints in the sample.

### Project or map to the shape your API actually needs

```csharp
return await query
    .OrderBy(e => e.Date)
    .Select(e => new EventListItemDto(
        e.Id,
        e.Name,
        e.Date,
        e.City,
        e.Description,
        e.Sessions.Count))
    .ToListAsync(cancellationToken);
```

Why this matters:

- list endpoints stay small and fast
- detail endpoints load the full graph on purpose
- every async call accepts a `CancellationToken`
- the code advertises exactly which data an endpoint needs

See:

- [`Repositories/EventRepository.cs`](../../demos/day2/TechConf.Sqlite.Api/Repositories/EventRepository.cs)
- [`Endpoints/EventEndpoints.cs`](../../demos/day2/TechConf.Sqlite.Api/Endpoints/EventEndpoints.cs)

## 8. Validation: annotations, FluentValidation, and endpoint filters

Day 2 uses two layers of validation together.

### Layer 1: built-in `.NET 10` validation

`builder.Services.AddValidation();` turns on automatic validation for Data Annotations, and the request models in [`Models/Dtos.cs`](../../demos/day2/TechConf.Sqlite.Api/Models/Dtos.cs) already use attributes like:

- `[Required]`
- `[StringLength(...)]`

That is the fast path for simple property rules.

### Layer 2: FluentValidation for business rules

The sample validators add extra rules that do not belong directly on the DTO type:

```csharp
RuleFor(x => x.Date)
    .Must(date => date.Date > DateTime.Today)
    .WithMessage("Event date must be in the future");
```

See:

- [`Validation/CreateEventValidator.cs`](../../demos/day2/TechConf.Sqlite.Api/Validation/CreateEventValidator.cs)
- [`Validation/UpdateEventValidator.cs`](../../demos/day2/TechConf.Sqlite.Api/Validation/UpdateEventValidator.cs)

### Make validation automatic with a filter

Instead of validating manually in every handler, the demo attaches a reusable filter to the endpoints that accept request bodies:

```csharp
group.MapPost("/", CreateEvent)
    .AddEndpointFilter<ValidationFilter<CreateEventRequest>>();

group.MapPut("/{id:int}", UpdateEvent)
    .AddEndpointFilter<ValidationFilter<UpdateEventRequest>>();
```

The filter resolves `IValidator<T>`, validates the request, and returns:

```csharp
TypedResults.ValidationProblem(validationResult.ToDictionary())
```

before the handler executes.

Also notice the endpoint metadata:

- `.ProducesValidationProblem()`
- `.ProducesProblem(...)`

That keeps OpenAPI and Scalar honest about failure shapes.

See:

- [`Validation/ValidationFilter.cs`](../../demos/day2/TechConf.Sqlite.Api/Validation/ValidationFilter.cs)
- [`Endpoints/EventEndpoints.cs`](../../demos/day2/TechConf.Sqlite.Api/Endpoints/EventEndpoints.cs)
- [`requests.http`](../../demos/day2/TechConf.Sqlite.Api/requests.http)

## 9. Problem Details and exception handling

Day 2 error handling is about consistency, not just catching exceptions.

The demo registers:

```csharp
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

app.UseExceptionHandler();
app.UseStatusCodePages();
```

The `GlobalExceptionHandler` maps known exceptions to HTTP status codes:

```csharp
var (statusCode, title) = exception switch
{
    NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
    ValidationException => (StatusCodes.Status400BadRequest, "Validation Error"),
    _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
};
```

and writes a `ProblemDetails` response with the request path as `instance`.

Why this pattern is worth memorizing:

- endpoint code stays focused on success paths
- clients see one predictable error format
- `NotFoundException` becomes a real `404`
- unexpected failures still become structured `500` responses

Use these demo requests to see the behavior quickly:

- `GET /api/events/999` -> 404 Problem Details
- `GET /api/debug/fail` -> 500 Problem Details
- invalid `POST /api/events` -> validation problem response

See:

- [`Exceptions/GlobalExceptionHandler.cs`](../../demos/day2/TechConf.Sqlite.Api/Exceptions/GlobalExceptionHandler.cs)
- [`Exceptions/NotFoundException.cs`](../../demos/day2/TechConf.Sqlite.Api/Exceptions/NotFoundException.cs)
- [`Program.cs`](../../demos/day2/TechConf.Sqlite.Api/Program.cs)
- [`requests.http`](../../demos/day2/TechConf.Sqlite.Api/requests.http)

## 10. Lab bridge: where the scaffolded TODOs live

The lab is intentionally not identical to the demo. Use the demo as the working reference, then implement the missing pieces in the exercise.

| Lab task | Where the TODO lives | Best reference |
| --- | --- | --- |
| Configure entity mappings | [`exercise/TechConf.Api/Data/Configurations/EventConfiguration.cs`](../../labs/lab2-persistence-validation/exercise/TechConf.Api/Data/Configurations/EventConfiguration.cs) | [`demos/day2/TechConf.Sqlite.Api/Data/Configurations/`](../../demos/day2/TechConf.Sqlite.Api/Data/Configurations/) |
| Register EF Core + repository + error handling | [`exercise/TechConf.Api/Program.cs`](../../labs/lab2-persistence-validation/exercise/TechConf.Api/Program.cs) | [`TechConf.Postgres.Api/Program.cs`](../../demos/day2/TechConf.Postgres.Api/Program.cs), [`TechConf.Sqlite.Api/Program.cs`](../../demos/day2/TechConf.Sqlite.Api/Program.cs) |
| Implement repository queries | [`exercise/TechConf.Api/Repositories/EventRepository.cs`](../../labs/lab2-persistence-validation/exercise/TechConf.Api/Repositories/EventRepository.cs) | [`demos/day2/TechConf.Sqlite.Api/Repositories/EventRepository.cs`](../../demos/day2/TechConf.Sqlite.Api/Repositories/EventRepository.cs) |
| Map entities to DTOs in endpoints | [`exercise/TechConf.Api/Endpoints/EventEndpoints.cs`](../../labs/lab2-persistence-validation/exercise/TechConf.Api/Endpoints/EventEndpoints.cs) | [`solution/TechConf.Api/Endpoints/EventEndpoints.cs`](../../labs/lab2-persistence-validation/solution/TechConf.Api/Endpoints/EventEndpoints.cs) |
| Add FluentValidation rules | [`exercise/TechConf.Api/Validation/CreateEventValidator.cs`](../../labs/lab2-persistence-validation/exercise/TechConf.Api/Validation/CreateEventValidator.cs) | [`demos/day2/TechConf.Sqlite.Api/Validation/`](../../demos/day2/TechConf.Sqlite.Api/Validation/) |
| Implement the exception handler | [`exercise/TechConf.Api/Exceptions/GlobalExceptionHandler.cs`](../../labs/lab2-persistence-validation/exercise/TechConf.Api/Exceptions/GlobalExceptionHandler.cs) | [`demos/day2/TechConf.Sqlite.Api/Exceptions/GlobalExceptionHandler.cs`](../../demos/day2/TechConf.Sqlite.Api/Exceptions/GlobalExceptionHandler.cs) |

**One subtle difference:** the finished demo projects return DTO-shaped data from the repository, while the lab deliberately asks you to map entities to DTOs in `Endpoints/EventEndpoints.cs`. That difference exists so students can practice the mapping layer explicitly.

## 11. What to remember after day 2

If you remember only a few things, remember these:

1. `DbContext` is short-lived and scoped to a request.
2. Put schema rules and relationships in Fluent API configuration classes.
3. Migrations are part of your codebase, not a throwaway tool artifact.
4. `AsNoTracking()`, `Include()`, `ThenInclude()`, and projection shape both performance and clarity.
5. Validation should fail before your handler logic does real work.
6. `ProblemDetails` gives clients one consistent error format.
7. `CancellationToken` belongs in async data access from the endpoint down.

## 12. Suggested reading order

If you want the shortest route from concept to working code:

1. [`docs/day2/README.md`](../../docs/day2/README.md)
2. [`demos/day2/TechConf.Sqlite.Api/Program.cs`](../../demos/day2/TechConf.Sqlite.Api/Program.cs)
3. [`demos/day2/TechConf.Sqlite.Api/Data/AppDbContext.cs`](../../demos/day2/TechConf.Sqlite.Api/Data/AppDbContext.cs)
4. [`demos/day2/TechConf.Sqlite.Api/Data/Configurations/`](../../demos/day2/TechConf.Sqlite.Api/Data/Configurations/)
5. [`demos/day2/TechConf.Sqlite.Api/Repositories/EventRepository.cs`](../../demos/day2/TechConf.Sqlite.Api/Repositories/EventRepository.cs)
6. [`docs/day2/03-validation.md`](../../docs/day2/03-validation.md)
7. [`demos/day2/TechConf.Sqlite.Api/Validation/ValidationFilter.cs`](../../demos/day2/TechConf.Sqlite.Api/Validation/ValidationFilter.cs)
8. [`docs/day2/04-problem-details.md`](../../docs/day2/04-problem-details.md)
9. [`demos/day2/TechConf.Sqlite.Api/Exceptions/GlobalExceptionHandler.cs`](../../demos/day2/TechConf.Sqlite.Api/Exceptions/GlobalExceptionHandler.cs)
10. [`demos/day2/TechConf.Sqlite.Api/requests.http`](../../demos/day2/TechConf.Sqlite.Api/requests.http)
11. [`labs/lab2-persistence-validation/README.md`](../../labs/lab2-persistence-validation/README.md)
