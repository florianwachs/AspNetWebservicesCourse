# Lab: GraphQL API with Hot Chocolate

## Overview

In this lab you will build a **GraphQL API** using [Hot Chocolate](https://chillicream.com/docs/hotchocolate) for the TechConf event-management domain. You will define queries with filtering, sorting, and projections, implement mutations, solve the N+1 problem with DataLoaders, and add cursor-based pagination — all on top of Entity Framework Core and SQLite.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed
- Basic knowledge of C# and Entity Framework Core

## Learning Objectives

1. Register and configure a Hot Chocolate GraphQL server in an ASP.NET Core application.
2. Create queries that expose EF Core `IQueryable` with filtering, sorting, and projection attributes.
3. Look up a single entity by ID using `[UseFirstOrDefault]`.
4. Implement mutations that accept input types and persist data.
5. Solve the N+1 problem with a `BatchDataLoader`.
6. Add cursor-based pagination with `[UsePaging]`.

## Getting Started

```bash
cd exercise/TechConf.GraphQL
dotnet run
```

Open a browser and navigate to **<https://localhost:5001/graphql>** to launch **Banana Cake Pop**, the built-in GraphQL IDE.

## Tasks

### Task 1 — Register Hot Chocolate in `Program.cs`

Open `Program.cs` and register the GraphQL server on the service collection:

```csharp
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .RegisterDbContextFactory<AppDbContext>();
```

Then map the GraphQL endpoint:

```csharp
app.MapGraphQL();
```

### Task 2 — Implement `GetEvents` query

In `GraphQL/Query.cs`, implement the `GetEvents` method so it returns `db.Events` as `IQueryable<Event>`. Decorate it with:

- `[UseProjection]`
- `[UseFiltering]`
- `[UseSorting]`

Inject `AppDbContext` via `[Service]` parameter injection.

### Task 3 — Implement `GetEventById` query

In the same file, implement `GetEventById`. Use `[UseFirstOrDefault]` so the schema returns a nullable single object instead of a list. Apply `[UseProjection]` for field selection.

### Task 4 — Implement `CreateEvent` mutation

In `GraphQL/Mutation.cs`, implement the `CreateEvent` method. Accept a `CreateEventInput` record, create a new `Event` with `Status = EventStatus.Draft`, persist it, and return it.

### Task 5 — Implement `SpeakerByIdDataLoader`

In `GraphQL/DataLoaders/SpeakerByIdDataLoader.cs`:

1. Make the class inherit `BatchDataLoader<Guid, Speaker>`.
2. Inject `IDbContextFactory<AppDbContext>` (DataLoaders outlive a single request scope).
3. Override `LoadBatchAsync` to query speakers whose IDs are in the batch.
4. Create a `SessionResolvers` type extension (`[ExtendObjectType(typeof(Session))]`) that resolves the speaker through the DataLoader.
5. Register the type extension in `Program.cs` with `.AddTypeExtension<SessionResolvers>()`.
6. Also register `IDbContextFactory<AppDbContext>` in `Program.cs`.

### Task 6 — Add cursor-based pagination to `GetSessions`

In `GraphQL/Query.cs`, implement `GetSessions` returning `db.Sessions`. Decorate it with:

- `[UsePaging(IncludeTotalCount = true)]`
- `[UseProjection]`
- `[UseFiltering]`
- `[UseSorting]`

## Stretch Goals

1. **Subscriptions** — Add a `Subscription` type so clients can receive real-time updates when a new event is created.
2. **Authorization** — Protect mutations with `[Authorize]` and configure a JWT bearer scheme.
3. **Custom filter conventions** — Restrict which fields are filterable by creating a custom `FilterInputType<Event>`.

## Solution

A fully working solution is available in the `solution/` directory.
