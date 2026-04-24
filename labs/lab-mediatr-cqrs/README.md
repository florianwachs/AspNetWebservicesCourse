# Lab: MediatR, CQRS, and Pipeline Behaviors with Aspire and React

## Overview

This optional Day 3 follow-up lab turns the `MediatR, CQRS, and pipeline behaviors` architecture notes into a small **full-stack WorkshopPlanner** application.

You will work with:

- a **Minimal API** backend organized around MediatR requests and handlers,
- an **Aspire AppHost** that wires the API and a Vite React frontend together with `AddViteApp(...)`,
- and a **React UI** that lets you inspect queries and trigger commands without falling back to raw HTTP requests.

The starter is intentionally guided:

- **queries** and one **command** are already complete so you can inspect the pattern,
- the UI already shows meaningful data,
- and the missing pieces focus on the next MediatR/CQRS steps instead of setup noise.

## Learning Objectives

1. Use `ISender` to keep Minimal API endpoints thin.
2. Separate **queries** from **commands** with one handler per use case.
3. Use **FluentValidation** validators with a centralized **validation behavior**.
4. Add a **logging behavior** as cross-cutting request pipeline logic.
5. Understand how a React frontend consumes the query side and triggers the command side in an Aspire app.

## Reference Material

- [Architecture overview](../../extras/architecture/00-overview.md)
- [Architecture styles](../../extras/architecture/01-architecture-styles.md)
- [MediatR, CQRS, and pipeline behaviors](../../extras/architecture/02-mediatr-cqrs-and-pipeline-behaviors.md)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js LTS](https://nodejs.org/)

> This lab uses an in-memory store, so it does **not** require PostgreSQL or Docker.

## Getting Started

### 1. Install frontend dependencies

```bash
cd exercise/WorkshopPlanner.Web
npm install
```

### 2. Start the Aspire AppHost

```bash
cd ../
dotnet run --project WorkshopPlanner.AppHost
```

Open the Aspire Dashboard and launch the **web** resource. The starter UI already lets you:

- browse the workshop board,
- inspect the detail projection for a selected workshop,
- and create new workshops through the running app.

## Suggested Task Flow

### Task 1 — Inspect the existing request pipeline

Start from the parts that are already complete:

- `GetWorkshops`
- `GetWorkshopById`
- `CreateWorkshop`
- `WorkshopPlanner.Web/src/api/workshops.ts`
- `WorkshopPlanner.Web/src/components/CreateWorkshopForm.tsx`

Trace one complete flow from:

1. React form submission
2. Minimal API endpoint
3. `ISender.Send(...)`
4. handler
5. response DTO
6. UI refresh via query endpoints

### Task 2 — Inspect the Aspire wiring

Open `exercise/WorkshopPlanner.AppHost/AppHost.cs` and verify that the frontend is connected through:

```csharp
builder.AddViteApp("web", "../WorkshopPlanner.Web")
    .WithReference(api)
    .WaitFor(api);
```

This is the bridge that makes the React frontend part of the Aspire app instead of a separate manual process.

### Task 3 — Complete `AddSessionHandler`

Open `exercise/WorkshopPlanner.Api/Features/Workshops/AddSession/AddSessionHandler.cs`.

Use the existing `CreateWorkshopHandler` as your example and implement the missing command logic:

- reject duplicate session titles inside the same workshop,
- reject agendas above 480 total minutes,
- create the `SessionState`,
- and return `SessionCreatedResponse`.

Then use the UI to add sessions to a draft workshop.

### Task 4 — Complete `PublishWorkshopHandler`

Open `exercise/WorkshopPlanner.Api/Features/Workshops/PublishWorkshop/PublishWorkshopHandler.cs`.

Implement the missing publish rules:

- at least one session must exist,
- total session duration must be at least 60 minutes,
- and publishing should stamp `PublishedOnUtc`.

Then publish a workshop from the UI and watch the query side update.

### Task 5 — Register the logging behavior

Open `exercise/WorkshopPlanner.Api/Program.cs` and complete the TODO in the MediatR registration:

```csharp
cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
```

Register logging **before** validation so failed validation attempts still show up in the logs.

### Task 6 — Re-run the full flow from the frontend

Once the missing handlers and behavior registration are in place:

1. create a draft workshop,
2. add sessions,
3. publish the workshop,
4. and refresh the board to confirm the read model reflects your commands.

## Stretch Goals

1. Add a search or city filter query and surface it in the React UI.
2. Surface validation errors inline in the forms instead of only showing the API error banner.
3. Add a second behavior that measures execution time or tags command/query requests differently in logs.

## Solution

A complete reference implementation is available in the `solution/` directory.
