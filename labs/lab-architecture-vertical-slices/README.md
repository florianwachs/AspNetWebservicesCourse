# Lab: Refactor WorkshopPlanner to Vertical Slice Architecture

## Overview

This lab starts from the same naive **WorkshopPlanner** Minimal API as the Onion lab, but the refactor target is different: instead of creating project rings, you will reorganize the API into **vertical slices** grouped by feature and use case.

The point is to move from "one endpoint file that knows everything" to "one feature slice that owns one use case top-to-bottom".

## Learning Objectives

- Spot the pain of noun-based endpoint files with clumped business logic
- Organize the code by feature instead of by technical layer
- Introduce commands and queries with thin endpoints
- Use MediatR to route requests to slice-specific handlers
- Keep shared code small so it does not turn into a hidden service layer

## Reference Material

- [Architecture overview](../../extras/architecture/00-overview.md)
- [Architecture styles](../../extras/architecture/01-architecture-styles.md)
- [MediatR, CQRS, and pipeline behaviors](../../extras/architecture/02-mediatr-cqrs-and-pipeline-behaviors.md)

## Getting Started

```bash
cd labs/lab-architecture-vertical-slices/exercise/WorkshopPlanner.Api
dotnet run
```

Open the OpenAPI UI at the local Scalar endpoint and inspect the current behavior.

## Starter Shape

The starter project is intentionally naive:

- `Program.cs` wires a single API project
- `Endpoints/WorkshopEndpoints.cs` contains reads, writes, validation, and business rules together
- `Data/WorkshopStore.cs` stores the in-memory state directly
- `Models/WorkshopModels.cs` keeps only the minimal state shape

## Tasks

### Task 1: Find the slice boundaries

Look at the starter endpoints and identify the actual use cases:

- list workshops
- get workshop details
- create a workshop
- add a session
- publish a workshop

Each of those should become its own slice.

### Task 2: Create the feature structure

Add a `Features/Workshops/` folder and organize the code by use case, not by controller/service/repository layers.

A good target is:

- one folder per use case
- a small `Features/Workshops/Shared/` folder for store-backed state and response shapes reused across multiple slices

If you move the mutable in-memory models into that shared folder, renaming them to `WorkshopState` and `SessionState` helps separate internal state from public request/response DTOs.

### Task 3: Refactor the reads

Extract the read endpoints into query slices:

- `GetWorkshops`
- `GetWorkshopById`

Keep response shaping close to the query that needs it.

For example, the list query can keep using a lightweight summary response, while the detail query should usually return a richer `WorkshopDetailResponse` that includes the session data.

### Task 4: Refactor the writes

Extract the write endpoints into command slices:

- `CreateWorkshop`
- `AddSession`
- `PublishWorkshop`

Each slice should own its request, handler, endpoint, and validation.

Use `AbstractValidator<TRequest>` for structural checks such as required fields and numeric ranges. Keep state-dependent rules such as uniqueness or "already published" checks in the handler where the store is available.

### Task 5: Introduce MediatR

Replace direct endpoint logic with `ISender.Send(...)` so endpoint code stays thin and the use-case flow is easy to follow.

Before the handlers compile, add the required packages to `WorkshopPlanner.Api.csproj` and register them in `Program.cs`:

```xml
<PackageReference Include="FluentValidation" Version="11.*" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.*" />
<PackageReference Include="MediatR" Version="12.*" />
```

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
```

A minimal first slice can look like this:

```csharp
public sealed record GetWorkshopsQuery() : IRequest<IReadOnlyList<WorkshopSummaryResponse>>;

group.MapGet("/", async (ISender sender, CancellationToken ct) =>
    TypedResults.Ok(await sender.Send(new GetWorkshopsQuery(), ct)));
```

### Task 6: Keep shared code earned

You will still need a little shared code, but keep it small:

- shared state/store access
- shared result mapping
- shared response shapes only where reuse is real

A tiny `Common/` folder for things like `Result<T>` and HTTP result mapping is fine. Do not recreate a broad service layer inside `Common/`.

## Stretch Goals

1. Add a logging or validation pipeline behavior so every request gets the same cross-cutting treatment.
2. Split the `Features/Workshops/` slices further into one-folder-per-use-case if the project grows.

## Solution

A complete reference solution is available in the `solution/` directory.
