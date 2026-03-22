# Day 1 API Essentials Cheat Sheet

> **Focus:** the smallest set of ideas from [`docs/day1/`](../../docs/day1/) that you need to read, run, and extend a first ASP.NET Core Minimal API.
>
> **Companion demo:** [`demos/day1/TechConf.Sessions.Api/`](../../demos/day1/TechConf.Sessions.Api/)
>
> **Use this exact demo while reading:** stay inside `demos/day1/TechConf.Sessions.Api/` for the walkthrough below, then switch to [`labs/lab1-events-api/`](../../labs/lab1-events-api/) when you want hands-on practice.

## 1. Start here

Before you run anything, make sure the .NET 10 SDK is available:

```bash
dotnet --version
```

From the repository root:

```bash
dotnet build demos/day1/TechConf.Sessions.Api/TechConf.Sessions.Api.csproj
dotnet run --project demos/day1/TechConf.Sessions.Api/TechConf.Sessions.Api.csproj --launch-profile http
```

Then open:

- Scalar UI: `http://localhost:5042/scalar/v1`
- OpenAPI JSON: `http://localhost:5042/openapi/v1.json`
- Manual request file: [`../../demos/day1/TechConf.Sessions.Api/requests.http`](../../demos/day1/TechConf.Sessions.Api/requests.http)

If you want the fastest path through day 1, keep these files open side by side:

- [`Program.cs`](../../demos/day1/TechConf.Sessions.Api/Program.cs)
- [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs)
- [`Data/InMemorySessionRepository.cs`](../../demos/day1/TechConf.Sessions.Api/Data/InMemorySessionRepository.cs)
- [`requests.http`](../../demos/day1/TechConf.Sessions.Api/requests.http)

## 2. Day 1 map: concept → doc → code

| Concept | Course material | Where to see it in the demo |
| --- | --- | --- |
| Project anatomy | [`01-dotnet10-overview.md`](../../docs/day1/01-dotnet10-overview.md) | [`TechConf.Sessions.Api.csproj`](../../demos/day1/TechConf.Sessions.Api/TechConf.Sessions.Api.csproj), [`Program.cs`](../../demos/day1/TechConf.Sessions.Api/Program.cs) |
| Modern C# basics | [`00-csharp-crash-course.md`](../../docs/day1/00-csharp-crash-course.md) | [`Models/Session.cs`](../../demos/day1/TechConf.Sessions.Api/Models/Session.cs), [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs) |
| LINQ for filtering and lookup | [`02-minimal-apis.md`](../../docs/day1/02-minimal-apis.md) | [`Data/InMemorySessionRepository.cs`](../../demos/day1/TechConf.Sessions.Api/Data/InMemorySessionRepository.cs), [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs) |
| Minimal API wiring | [`02-minimal-apis.md`](../../docs/day1/02-minimal-apis.md) | [`Program.cs`](../../demos/day1/TechConf.Sessions.Api/Program.cs), [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs) |
| Dependency injection | [`02a-dependency-injection.md`](../../docs/day1/02a-dependency-injection.md) | [`Program.cs`](../../demos/day1/TechConf.Sessions.Api/Program.cs), [`Data/ISessionRepository.cs`](../../demos/day1/TechConf.Sessions.Api/Data/ISessionRepository.cs), [`Data/InMemorySessionRepository.cs`](../../demos/day1/TechConf.Sessions.Api/Data/InMemorySessionRepository.cs) |
| HTTP + REST rules | [`03-http-fundamentals.md`](../../docs/day1/03-http-fundamentals.md) | [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs), [`requests.http`](../../demos/day1/TechConf.Sessions.Api/requests.http) |
| OpenAPI + Scalar | [`04-openapi-scalar.md`](../../docs/day1/04-openapi-scalar.md) | [`Program.cs`](../../demos/day1/TechConf.Sessions.Api/Program.cs), [`requests.http`](../../demos/day1/TechConf.Sessions.Api/requests.http) |

## 3. Project anatomy in 60 seconds

These are the files that matter most in a first web API:

| File | Why it exists |
| --- | --- |
| [`TechConf.Sessions.Api.csproj`](../../demos/day1/TechConf.Sessions.Api/TechConf.Sessions.Api.csproj) | Defines the project type, target framework, nullable settings, implicit usings, and NuGet packages. |
| [`Program.cs`](../../demos/day1/TechConf.Sessions.Api/Program.cs) | App startup: register services, enable OpenAPI, map endpoints, run the app. |
| [`Models/Session.cs`](../../demos/day1/TechConf.Sessions.Api/Models/Session.cs) | Record types for your resource and request bodies. |
| [`Data/ISessionRepository.cs`](../../demos/day1/TechConf.Sessions.Api/Data/ISessionRepository.cs) | Abstraction used by the endpoints. |
| [`Data/InMemorySessionRepository.cs`](../../demos/day1/TechConf.Sessions.Api/Data/InMemorySessionRepository.cs) | Simple in-memory implementation used on day 1, including LINQ-based sorting, lookup, and duplicate checks. |
| [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs) | The HTTP surface of the API. |
| [`requests.http`](../../demos/day1/TechConf.Sessions.Api/requests.http) | A human-readable way to exercise the API without writing a frontend. |

**Mental model:** `Program.cs` wires the app together, `Endpoints/` exposes HTTP routes, `Models/` shapes JSON, and `Data/` holds the current implementation behind DI.

## 4. Modern C# features used by the demo

Day 1 does not need all of C#. It needs the parts that make API code readable.

### Top-level statements

`Program.cs` contains executable code directly, without a `Program` class or `Main` method.

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run();
```

See: [`Program.cs`](../../demos/day1/TechConf.Sessions.Api/Program.cs)

### Records for API data

Records are a great fit for request/response models because they are concise and immutable by default.

Beginner-friendly mental model:

- `record` = mostly **the data**
- `class` = mostly **the behavior and changing state**

Choose a `record` when the type mainly represents **data**:

- request/response models
- values you want to compare by content
- immutable objects you want to copy with `with`

Choose a `class` when the type mainly represents **behavior or identity**:

- services
- repositories
- entities that change over time
- objects with mutable state and methods

In practice, a `class` is usually the better choice when you care more about **methods, fields, and mutable state** than about simple data transport.

Both `record` and `class` can have properties and methods, but their usual purpose is different:

- `record`: mainly for carrying data
- `class`: mainly for behavior, lifecycle, and state changes

Quick beginner note:

- **properties** are the normal public way to expose data
- **fields** are usually private internal storage inside a class
- **methods** define what an object can do

**Rule of thumb:** if you are in doubt, just use a `class`. It is the safer default because it is the more general-purpose option.

```csharp
public record Session(
    int Id,
    string Title,
    string Speaker,
    string Track,
    DateTime StartsAt,
    int DurationMinutes,
    bool IsPublished);
```

Short example:

```csharp
public record CreateSessionRequest(string Title, string Speaker);

public class SessionService
{
    private int _publishedCount; // field

    public int PublishedCount => _publishedCount; // property

    public void Publish(int id) // method
    {
        _publishedCount++;
        // business logic goes here
    }
}
```

Here the `record` is just data for the API request, while the `class` stores state and contains behavior.

See: [`Models/Session.cs`](../../demos/day1/TechConf.Sessions.Api/Models/Session.cs)

### `with` expressions for safe updates

The patch endpoint creates a new value based on the old one instead of mutating properties one by one.

```csharp
var updatedSession = existingSession with
{
    Track = request.Track ?? existingSession.Track,
    IsPublished = request.IsPublished ?? existingSession.IsPublished
};
```

See: [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs)

### Nullable types for optional input

`PatchSessionRequest` uses nullable properties because a partial update may omit fields.

```csharp
public record PatchSessionRequest(
    string? Title,
    string? Speaker,
    string? Track,
    DateTime? StartsAt,
    int? DurationMinutes,
    bool? IsPublished);
```

See: [`Models/Session.cs`](../../demos/day1/TechConf.Sessions.Api/Models/Session.cs)

## 5. `Program.cs`: the startup flow to memorize

Minimal APIs keep startup small:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ISessionRepository, InMemorySessionRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapSessionEndpoints();
app.Run();
```

What each part does:

1. `CreateBuilder` prepares configuration, logging, and DI.
2. `builder.Services...` registers services in the container.
3. `Build()` creates the `WebApplication`.
4. `MapOpenApi()` and `MapScalarApiReference()` expose API documentation.
5. `MapSessionEndpoints()` wires the route group from `Endpoints/`.
6. `Run()` starts Kestrel.

See: [`Program.cs`](../../demos/day1/TechConf.Sessions.Api/Program.cs)

## 6. Minimal API endpoint mapping

The route group keeps related routes together:

```csharp
var group = app.MapGroup("/api/sessions")
    .WithTags("Sessions");

group.MapGet("/", GetAllSessions);
group.MapGet("/{id:int}", GetSessionById);
group.MapPost("/", CreateSession);
group.MapPut("/{id:int}", ReplaceSession);
group.MapPatch("/{id:int}", PatchSession);
group.MapDelete("/{id:int}", DeleteSession);
```

### HTTP method quick reference

| Operation | Map method | Example route in the demo |
| --- | --- | --- |
| Read collection | `MapGet` | `GET /api/sessions` |
| Read one item | `MapGet` | `GET /api/sessions/{id:int}` |
| Create | `MapPost` | `POST /api/sessions` |
| Full replace | `MapPut` | `PUT /api/sessions/{id:int}` |
| Partial update | `MapPatch` | `PATCH /api/sessions/{id:int}` |
| Delete | `MapDelete` | `DELETE /api/sessions/{id:int}` |

### Metadata matters

Use endpoint metadata so OpenAPI and Scalar become useful:

- `.WithName(...)`
- `.WithSummary(...)`
- `.Produces(...)`
- `.ProducesProblem(...)`
- `.ProducesValidationProblem(...)`

See: [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs)

## 7. Parameter binding: where values come from

ASP.NET Core inspects handler parameters and binds them automatically.

| Source | Demo example | Where to look |
| --- | --- | --- |
| Route | `int id` with `/{id:int}` | `GetSessionById`, `ReplaceSession`, `PatchSession`, `DeleteSession` |
| Query string | `string? track`, `string? speaker`, `bool? published`, `string? search` | `GetAllSessions` |
| JSON body | `CreateSessionRequest`, `UpdateSessionRequest`, `PatchSessionRequest` | `CreateSession`, `ReplaceSession`, `PatchSession` |
| Dependency injection | `ISessionRepository repository` | every endpoint handler |

Useful rules from day 1:

- Route parameter names must match the route template.
- Query parameters are great for filtering and searching.
- POST/PUT/PATCH usually bind one complex object from JSON.
- Registered services can be injected directly into handlers.

See: [`02-minimal-apis.md`](../../docs/day1/02-minimal-apis.md) and [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs)

## 8. LINQ: query collections without manual loops

LINQ lets you filter, sort, and search objects with small composable operators instead of writing manual loops. On day 1, LINQ runs over the in-memory `List<Session>` used by the repository.

In the repository, LINQ keeps the list ordering logic concise:

```csharp
return _sessions
    .OrderBy(session => session.StartsAt)
    .ThenBy(session => session.Title)
    .ToList();
```

The endpoints build another LINQ pipeline for query-string filtering:

```csharp
var query = repository.List().AsEnumerable();

if (!string.IsNullOrWhiteSpace(track))
{
    query = query.Where(session => session.Track.Equals(track, StringComparison.OrdinalIgnoreCase));
}

if (!string.IsNullOrWhiteSpace(search))
{
    query = query.Where(session =>
        session.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
        session.Speaker.Contains(search, StringComparison.OrdinalIgnoreCase) ||
        session.Track.Contains(search, StringComparison.OrdinalIgnoreCase));
}

return TypedResults.Ok(query.ToList());
```

### Operators you should recognize on day 1

| Operator | What it means here | Demo example |
| --- | --- | --- |
| `Where(...)` | Keep only matching items | filter by `track`, `speaker`, `published`, or `search` |
| `OrderBy(...)` | Sort by the primary key | order sessions by `StartsAt` |
| `ThenBy(...)` | Add a secondary sort key | break ties with `Title` |
| `FirstOrDefault(...)` | Return the first match or `null` | find one session by `Id` |
| `Any(...)` | Check whether anything matches | detect duplicate titles |
| `ToList()` | Materialize the current query as a list | return the final response payload |
| `AsEnumerable()` | Start composing an `IEnumerable<T>` query pipeline | build up optional filters in `GetAllSessions` |

**Day 1 boundary:** this is still just C# over in-memory objects. On day 2 you reuse many of the same LINQ ideas with EF Core queries against a real database.

For broader coverage of operators like `Select`, `GroupBy`, and `Join`, see the shared [`../csharp-cheat-sheet.md`](../csharp-cheat-sheet.md).

See:

- [`Data/InMemorySessionRepository.cs`](../../demos/day1/TechConf.Sessions.Api/Data/InMemorySessionRepository.cs)
- [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs)
- [`02-minimal-apis.md`](../../docs/day1/02-minimal-apis.md)

## 9. REST and HTTP rules to keep

This is where [`03-http-fundamentals.md`](../../docs/day1/03-http-fundamentals.md) should actively shape your endpoints.

### Resource naming

- Use **nouns**, not verbs.
- Prefer **plural** resource names.
- Use **query parameters** for filtering instead of inventing extra action routes.

The demo follows this:

- `GET /api/sessions`
- `GET /api/sessions/1`
- `GET /api/sessions?track=HTTP&published=true`

### Status code quick guide

| Situation | Status code | Demo endpoint |
| --- | --- | --- |
| Read succeeded | `200 OK` | `GET /api/sessions`, `GET /api/sessions/{id}` |
| Create succeeded | `201 Created` + `Location` | `POST /api/sessions` |
| Replace/delete with no response body | `204 No Content` | `PUT /api/sessions/{id}`, `DELETE /api/sessions/{id}` |
| Resource missing | `404 Not Found` | `GET /api/sessions/999` |
| Business conflict | `409 Conflict` | duplicate session title |
| Validation failed | `422 Unprocessable Entity` | invalid session payload |

### Verb semantics

| Verb | Meaning | Demo behavior |
| --- | --- | --- |
| `GET` | Safe, read-only | returns data, never changes state |
| `POST` | Create a new server-managed resource | returns `201 Created` and the new representation |
| `PUT` | Full replacement | replaces the complete session and returns `204` |
| `PATCH` | Partial update | updates only selected fields and returns the updated session |
| `DELETE` | Remove a resource | returns `204` when deletion succeeds |

**Avoid this anti-pattern:** returning `200 OK` with an error message inside the body. Use real HTTP status codes so clients can react correctly.

See: [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs) and [`requests.http`](../../demos/day1/TechConf.Sessions.Api/requests.http)

## 10. TypedResults: make success and failure explicit

Typed results are one of the biggest day 1 wins:

```csharp
private static Results<Ok<Session>, NotFound> GetSessionById(int id, ISessionRepository repository)
{
    var session = repository.GetById(id);

    return session is not null
        ? TypedResults.Ok(session)
        : TypedResults.NotFound();
}
```

Why this is better than plain `IResult` for a first API:

- the method advertises all allowed outcomes
- the compiler helps you stay consistent
- OpenAPI can infer more metadata automatically
- readers can understand the endpoint contract without guessing

Look at these return types in the demo:

- `Results<Ok<Session>, NotFound>`
- `Results<CreatedAtRoute<Session>, UnprocessableEntity<HttpValidationProblemDetails>, ProblemHttpResult>`
- `Results<NoContent, NotFound>`

See: [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs)

## 11. Dependency injection without overthinking it

From [`02a-dependency-injection.md`](../../docs/day1/02a-dependency-injection.md):

- **Transient**: new instance every time
- **Scoped**: one instance per HTTP request
- **Singleton**: one instance for the whole application lifetime

The demo intentionally uses a singleton repository:

```csharp
builder.Services.AddSingleton<ISessionRepository, InMemorySessionRepository>();
```

Why this is okay here:

- the app is using an in-memory store for teaching purposes
- there is no database context yet
- the repository is made thread-safe with `lock`

When you move to day 2 and introduce EF Core, repository-like services normally become **scoped**.

See:

- [`Program.cs`](../../demos/day1/TechConf.Sessions.Api/Program.cs)
- [`Data/ISessionRepository.cs`](../../demos/day1/TechConf.Sessions.Api/Data/ISessionRepository.cs)
- [`Data/InMemorySessionRepository.cs`](../../demos/day1/TechConf.Sessions.Api/Data/InMemorySessionRepository.cs)

## 12. OpenAPI, Scalar, and `.http` files

The shortest useful setup is:

```csharp
builder.Services.AddOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
```

This gives you:

- machine-readable API docs at `/openapi/v1.json`
- interactive docs in Scalar at `/scalar/v1`
- a contract that stays close to your endpoint code

Use [`requests.http`](../../demos/day1/TechConf.Sessions.Api/requests.http) alongside Scalar:

- Scalar helps you explore the API
- `.http` files help you repeat exact requests
- both reinforce that HTTP is the public contract

## 13. What to remember after day 1

If you remember only a few things, remember these:

1. A Minimal API starts with `CreateBuilder()`, `Build()`, `Map...()`, and `Run()`.
2. Records are a clean default for request/response models.
3. LINQ lets you filter, sort, and search in-memory collections without manual loops.
4. Route groups keep endpoint organization simple.
5. DI lets handlers depend on abstractions instead of concrete classes.
6. `201`, `204`, `404`, `409`, and `422` are not trivia — they are part of your API design.
7. `TypedResults` make endpoint contracts easier to read and document.
8. OpenAPI + Scalar + `.http` files are your fastest feedback loop.

## 14. Suggested reading order

If you want to move between the docs and the code efficiently:

1. [`docs/day1/README.md`](../../docs/day1/README.md)
2. [`Program.cs`](../../demos/day1/TechConf.Sessions.Api/Program.cs)
3. [`Endpoints/SessionEndpoints.cs`](../../demos/day1/TechConf.Sessions.Api/Endpoints/SessionEndpoints.cs)
4. [`Data/InMemorySessionRepository.cs`](../../demos/day1/TechConf.Sessions.Api/Data/InMemorySessionRepository.cs)
5. [`02-minimal-apis.md`](../../docs/day1/02-minimal-apis.md)
6. [`03-http-fundamentals.md`](../../docs/day1/03-http-fundamentals.md)
7. [`02a-dependency-injection.md`](../../docs/day1/02a-dependency-injection.md)
8. [`04-openapi-scalar.md`](../../docs/day1/04-openapi-scalar.md)
9. [`requests.http`](../../demos/day1/TechConf.Sessions.Api/requests.http)

## 15. Next step: practice it yourself

Once the reference demo feels familiar, move to [`labs/lab1-events-api/`](../../labs/lab1-events-api/).

That lab asks you to implement the same core ideas yourself instead of just reading the finished version:

- Minimal API routing
- Typed results and status codes
- In-memory storage
- OpenAPI, Scalar, and `.http` testing

Use this cheatsheet and the `TechConf.Sessions.Api` demo as your reference while you work through the lab.
