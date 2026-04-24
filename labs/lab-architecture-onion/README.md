# Lab: Refactor WorkshopPlanner to Onion Architecture

## Overview

This lab starts from a deliberately naive Minimal API where endpoints, validation, business rules, and state mutation all live in one place. Your job is to refactor that baseline into an **Onion Architecture** with explicit project boundaries and inward dependencies.

The domain is intentionally small: **WorkshopPlanner** manages workshops, sessions, and a publish step. That keeps the focus on architecture instead of infrastructure.

## Learning Objectives

- Spot where HTTP concerns and business rules are mixed together
- Move workshop invariants into a protected domain core
- Introduce application use cases that coordinate work without knowing about HTTP
- Push storage into an infrastructure project behind an application-facing abstraction
- Keep API endpoints thin and focused on transport concerns

## Reference Material

- [Architecture overview](../../extras/architecture/00-overview.md)
- [Architecture styles](../../extras/architecture/01-architecture-styles.md)
- [Repository pattern](../../extras/architecture/03-repository-pattern.md)

## Getting Started

```bash
cd labs/lab-architecture-onion/exercise/WorkshopPlanner.Api
dotnet run
```

Open the OpenAPI UI at the local Scalar endpoint and inspect the current behavior.

## Starter Shape

The starter project is intentionally naive:

- `Program.cs` wires everything into a single API project
- `Endpoints/WorkshopEndpoints.cs` mixes validation, business rules, and HTTP responses
- `Data/WorkshopStore.cs` owns the in-memory state directly
- `Models/WorkshopModels.cs` is just enough shape to make the baseline run

## Tasks

### Task 1: Map the pain in the baseline

Read the starter and identify which rules belong to the domain rather than the transport layer:

- a workshop title and city are required
- a workshop needs at least one hour of sessions before it can be published
- published workshops cannot be changed
- total workshop session time may not exceed 480 minutes

### Task 2: Create the Onion rings

Split the code into explicit projects:

- `WorkshopPlanner.Domain`
- `WorkshopPlanner.Application`
- `WorkshopPlanner.Infrastructure`
- `WorkshopPlanner.Api`

Keep dependencies pointing inward.

The starter only contains the API/AppHost baseline, so create the extra projects yourself from the exercise root:

```bash
cd labs/lab-architecture-onion/exercise
dotnet new classlib -n WorkshopPlanner.Domain
dotnet new classlib -n WorkshopPlanner.Application
dotnet new classlib -n WorkshopPlanner.Infrastructure
dotnet sln WorkshopPlanner.Onion.slnx add \
  WorkshopPlanner.Domain/WorkshopPlanner.Domain.csproj \
  WorkshopPlanner.Application/WorkshopPlanner.Application.csproj \
  WorkshopPlanner.Infrastructure/WorkshopPlanner.Infrastructure.csproj
```

Then wire references inward:

- `WorkshopPlanner.Application` → `WorkshopPlanner.Domain`
- `WorkshopPlanner.Infrastructure` → `WorkshopPlanner.Application`, `WorkshopPlanner.Domain`
- `WorkshopPlanner.Api` → `WorkshopPlanner.Application`

### Task 3: Move invariants into the domain

Create a `Workshop` aggregate in the domain and move the rules for:

- workshop creation
- adding sessions
- publishing a workshop

The goal is that the domain model can reject invalid state transitions without knowing anything about HTTP or storage.

### Task 4: Introduce application use cases

Create application handlers/use cases for:

- list workshops
- get workshop details
- create a workshop
- add a session
- publish a workshop

The application layer should define the repository abstraction that it needs.

A practical target is:

- `Abstractions/IWorkshopRepository.cs` in the application layer
- one folder per use case under `Workshops/`
- shared read models such as `WorkshopResponses.cs` kept in the application layer

### Task 5: Push storage outward

Move the in-memory store into infrastructure as an implementation of the application repository abstraction. The API should depend on the application layer, not on the storage details directly.

### Task 6: Thin the API

Refactor the API project so endpoints only:

- accept HTTP input
- call the correct application handler
- map the result to HTTP responses

That refactor should also change the endpoint style:

- inject handlers instead of the old `WorkshopStore`
- make the endpoints async
- pass `CancellationToken`
- map failures to one consistent HTTP contract such as `ErrorResponse` instead of anonymous `new { error = "..." }` objects

## Stretch Goals

1. Add a second infrastructure adapter, such as a JSON-file repository, without changing the domain or application layers.
2. Add a rule that published workshops need at least two distinct speakers and keep that rule inside the domain core.

## Solution

A complete reference solution is available in the `solution/` directory.
