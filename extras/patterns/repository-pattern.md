# Repository Pattern

> Software patterns provide proven solutions for common problems encountered during application development and architecture.
> They are not meant to be applied preemptively — every advantage comes with trade-offs such as increased complexity.
> Always evaluate whether applying a pattern genuinely simplifies your task or adds unnecessary overhead.

## The Problem

Most applications need to read from or write to a data source at some point. When you rely on direct database access — for example raw SQL queries via Dapper — the resulting code becomes tightly coupled to a specific database technology and third-party library.

This is especially problematic for **(unit) testability**: tests now require a live database connection. Even with SQLite or a local database this adds significant setup overhead — each test run typically needs a clean database with seed data. A side effect is slower test execution due to this infrastructure dependency.

## Solution: The Repository Pattern

We identified two core challenges:

- **Tight coupling** to a specific data-access technology
- **Reduced testability** with unit tests

The Repository pattern addresses both by separating **data access** from the **data-access technology**. An interface defines the required **Create-Read-Update-Delete (CRUD)** methods:

```csharp
public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(Guid id);
    Task<Event> AddAsync(Event newEvent);
    Task<Event> UpdateAsync(Event updatedEvent);
    Task DeleteAsync(Guid id);
}
```

A concrete implementation then uses the desired technology — for example Entity Framework Core:

```csharp
public class EfEventRepository : IEventRepository
{
    private readonly TechConfDbContext _db;

    public EfEventRepository(TechConfDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
        => await _db.Events.ToListAsync();

    public async Task<Event?> GetByIdAsync(Guid id)
        => await _db.Events.FindAsync(id);

    public async Task<Event> AddAsync(Event newEvent)
    {
        _db.Events.Add(newEvent);
        await _db.SaveChangesAsync();
        return newEvent;
    }

    public async Task<Event> UpdateAsync(Event updatedEvent)
    {
        _db.Events.Update(updatedEvent);
        await _db.SaveChangesAsync();
        return updatedEvent;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _db.Events.FindAsync(id);
        if (entity is not null)
        {
            _db.Events.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
```

Code that previously interacted directly with the database now accesses data only through the interface. Here is an example using **Minimal APIs**:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IEventRepository, EfEventRepository>();

var app = builder.Build();

app.MapGet("/api/events", async (IEventRepository repo) =>
{
    var events = await repo.GetAllAsync();
    return Results.Ok(events);
});

app.MapGet("/api/events/{id:guid}", async (Guid id, IEventRepository repo) =>
{
    var ev = await repo.GetByIdAsync(id);
    return ev is not null ? Results.Ok(ev) : Results.NotFound();
});

app.MapPost("/api/events", async (Event newEvent, IEventRepository repo) =>
{
    var created = await repo.AddAsync(newEvent);
    return Results.Created($"/api/events/{created.Id}", created);
});

app.MapPut("/api/events/{id:guid}", async (Guid id, Event updated, IEventRepository repo) =>
{
    updated.Id = id;
    var result = await repo.UpdateAsync(updated);
    return Results.Ok(result);
});

app.MapDelete("/api/events/{id:guid}", async (Guid id, IEventRepository repo) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
});

app.Run();
```

For tests you can now create an **in-memory repository**, or use a mocking framework such as [Moq](https://github.com/devlooped/moq) or [NSubstitute](https://nsubstitute.github.io/) to stub individual interface methods and return fixed data for your test cases.

The interface may also define additional methods for filtering or searching. However, a repository should **not** contain business logic — it is concerned only with CRUD operations.

## Criticism

There has been growing criticism of the pattern, particularly when used with **Entity Framework Core**. The core argument is that EF Core's `DbContext` already acts as a Unit of Work and its `DbSet<T>` already acts as a repository. Wrapping it in another repository layer adds an abstraction that:

- Limits access to powerful EF Core features (e.g., `IQueryable`, change tracking, projections)
- Will likely never be swapped for a different ORM in practice
- Adds maintenance overhead without proportional benefit

A well-known critique: [Is the Repository Pattern Useful with Entity Framework Core?](https://www.thereformedprogrammer.net/is-the-repository-pattern-useful-with-entity-framework-core/)

## When to Use a Repository vs. Direct DbContext Injection

| Scenario | Recommendation |
|---|---|
| Small to medium apps with straightforward data access | **Inject `DbContext` directly** — simpler, full EF Core power |
| Application already heavily uses EF Core features (projections, `IQueryable` composition) | **Inject `DbContext` directly** — a repository layer would limit these capabilities |
| Need to support multiple data sources or swap storage technologies | **Use the Repository pattern** — the abstraction pays for itself |
| Complex domain logic that benefits from explicit data-access contracts | **Use the Repository pattern** — it clarifies boundaries |
| High unit-test coverage without integration-test infrastructure | **Use the Repository pattern** — easy to mock |
| CQRS architecture with separate read/write paths | **Consider thin query services** instead of full CRUD repositories |

> **Pragmatic advice:** Start without the repository layer. If you find that testability or technology coupling becomes a real pain point, introduce the abstraction where it is needed — not everywhere preemptively.
