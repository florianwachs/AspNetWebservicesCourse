# Day 1 Demos — Minimal APIs & OpenAPI

## Demo 1: Hello World API (10 min)

Create a new .NET 10 web API from scratch:

```bash
dotnet new web -n HelloApi
cd HelloApi
dotnet run
```

Show:

- `Program.cs` with top-level statements
- `app.MapGet("/", () => "Hello, World!");`
- How `WebApplication.CreateBuilder()` works
- The `.csproj` file structure

## Demo 2: Building Events CRUD

Build up the Events API step by step:

1. Start with a simple `Event` record
2. Add an in-memory `List<Event>` store
3. Implement `MapGet` for all events
4. Add route parameter with `MapGet("/{id:int}")`
5. Show `Results.Ok()` vs `TypedResults.Ok()` difference
6. Add `MapPost` with request body binding
7. Show `TypedResults.Created()` with location header
8. Add `MapPut` and `MapDelete`

Key teaching moments:

- Parameter binding (route, query, body)
- TypedResults and OpenAPI metadata generation
- Record types for immutability

## Demo 3: Route Groups & Organization

Refactor the inline endpoints into:

- A separate `EventEndpoints.cs` file
- Extension method pattern
- `MapGroup("/api/events").WithTags("Events")`

## Demo 4: OpenAPI & Scalar

1. Add `builder.Services.AddOpenApi()` and `app.MapOpenApi()`
2. Show the raw JSON at `/openapi/v1.json`
3. Add Scalar: `dotnet add package Scalar.AspNetCore`
4. Show `app.MapScalarApiReference()` and the UI
5. Execute requests directly from Scalar

## Demo 5: .http Files

Create a `requests.http` file and show:

- GET, POST, PUT, DELETE requests
- Variables (`@baseUrl`)
- Running requests from VS Code
