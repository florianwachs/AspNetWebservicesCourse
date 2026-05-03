# Lab: OData — Standardized Queryable REST API

## Overview

In this lab you will build an OData-enabled REST API for the **TechConf Event Management** domain using **Microsoft.AspNetCore.OData**. OData (Open Data Protocol) provides a standardized way to query and manipulate data through REST APIs. Instead of building custom query endpoints, OData gives your API powerful query capabilities — `$filter`, `$select`, `$expand`, `$orderby`, `$top`, `$skip` — out of the box.

You will start with a scaffolded project that includes the domain models, EF Core context with seed data, and controller stubs. Your job is to wire up the OData Entity Data Model, implement the controllers, add OData Functions and Actions, and configure query limits.

## Prerequisites

- .NET 10 SDK installed
- Basic C# and Entity Framework Core knowledge
- An HTTP client (the project includes a `requests.http` file for use with VS Code REST Client or JetBrains HTTP Client)

## Learning Objectives

1. Build an OData **Entity Data Model (EDM)** with `ODataConventionModelBuilder`
2. Implement OData controllers that inherit from `ODataController` and use `[EnableQuery]`
3. Use OData system query options: `$filter`, `$select`, `$expand`, `$orderby`, `$top`, `$skip`, `$count`
4. Apply partial updates with `Delta<T>` for PATCH operations
5. Define and implement OData **Functions** (GET) and **Actions** (POST) for custom operations
6. Configure query limits (`PageSize`, `MaxTop`, `MaxExpansionDepth`, `AllowedQueryOptions`) to protect your API

## Getting Started

```bash
cd labs/lab-odata/exercise/TechConf.OData
dotnet restore
dotnet run
```

> **Tip:** Once the OData services are configured, browse to `https://localhost:5001/odata/$metadata` to inspect the Entity Data Model. The metadata document describes all entity types, entity sets, functions, and actions available in your API.

## Tasks

### Task 1 — Configure OData EDM

Open `Program.cs` and build the OData Entity Data Model:

1. Create an `ODataConventionModelBuilder` and register entity sets for `Event`, `Session`, `Speaker`, `Attendee`, and `Registration`
2. Register OData services with `AddControllers().AddOData(...)` — enable `Select()`, `Filter()`, `OrderBy()`, `Count()`, `Expand()`, and set `MaxTop(100)`
3. Add the route component with prefix `"odata"` and the EDM model
4. Map controllers with `app.MapControllers()`

```csharp
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Event>("Events");
// ... register remaining entity sets

builder.Services.AddControllers()
    .AddOData(options => options
        .Select().Filter().OrderBy().SetMaxTop(100).Count().Expand()
        .AddRouteComponents("odata", modelBuilder.GetEdmModel()));
```

### Task 2 — Implement EventsController

Open `Controllers/EventsController.cs` and implement the CRUD operations:

- **GET all** — Return `IQueryable<Event>` with `[EnableQuery(PageSize = 50)]` so OData can apply query options
- **GET by key** — Find by `Guid` key, return `NotFound()` or `Ok(event)`
- **POST** — Add the event to the database, return `Created(ev)`
- **PATCH** — Use `Delta<Event>` to apply partial updates, return `Updated(ev)`
- **DELETE** — Remove the event, return `NoContent()`

> **Key concept:** Returning `IQueryable<T>` (not `List<T>`) is essential — it allows OData to translate query options into SQL via EF Core.

### Task 3 — Implement SessionsController

Open `Controllers/SessionsController.cs` and implement:

- **GET all** — Return `IQueryable<Session>` with `[EnableQuery]` so `$expand=Speaker` works
- **GET by key** — Find by `Guid` key, return `NotFound()` or `Ok(session)`

Test with: `GET /odata/Sessions?$expand=Speaker&$select=Title,StartTime,Room`

### Task 4 — Add OData Function (AvailableSeats)

OData **Functions** are side-effect-free operations invoked with GET.

1. In `Program.cs`, register the function on the `Event` entity type:
   ```csharp
   modelBuilder.EntityType<Event>()
       .Function("AvailableSeats")
       .Returns<int>();
   ```
2. In `EventsController.cs`, implement the `AvailableSeats` method:
   - Load the event with its registrations
   - Return `MaxAttendees - Registrations.Count`

Test with: `GET /odata/Events(aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa)/AvailableSeats`

### Task 5 — Add OData Action (Cancel)

OData **Actions** are operations with side effects, invoked with POST.

1. In `Program.cs`, register the action on the `Event` entity type:
   ```csharp
   modelBuilder.EntityType<Event>()
       .Action("Cancel")
       .ReturnsFromEntitySet<Event>("Events");
   ```
2. In `EventsController.cs`, implement the `Cancel` method:
   - Find the event, set `Status` to `"Cancelled"`, save, return `Ok(ev)`

Test with: `POST /odata/Events(bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb)/Cancel`

### Task 6 — Configure Query Limits

Protect your API from expensive queries by adding limits:

- Set `PageSize` on `[EnableQuery]` attributes (e.g., `PageSize = 50`)
- Set `MaxTop(100)` in the OData options
- Set `MaxExpansionDepth` to limit nested `$expand` depth (e.g., `MaxExpansionDepth = 3`)
- Optionally restrict `AllowedQueryOptions` or `AllowedFunctions` on specific endpoints

```csharp
[EnableQuery(PageSize = 50, MaxExpansionDepth = 3)]
public IQueryable<Event> Get() => _db.Events;
```

## Stretch Goals

1. **Add `$apply` aggregation support** — Enable groupby/aggregate to count events by status: `GET /odata/Events?$apply=groupby((Status),aggregate($count as Count))`
2. **Add `[Authorize]` per controller action** — Require authentication for POST, PATCH, DELETE while keeping GET endpoints open
3. **Implement PUT for full entity replacement** — Add a `Put` method to `EventsController` that replaces the entire entity (unlike PATCH which applies partial updates)

## Solution

A complete working solution is available in the [`solution/`](./solution/) directory.
