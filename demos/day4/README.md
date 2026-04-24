# Day 4 Demos - Monolith vs Clean Architecture

## Runnable samples

This folder contains two runnable WorkshopPlanner samples with the same expanded HTTP surface and seed data, but very different internal structure.

### Ball of mud monolith

```bash
cd demos/day4/ball-of-mud/WorkshopPlanner.Api
dotnet run
```

Useful URLs and files:

- API root: `http://localhost:5401`
- Scalar UI: `http://localhost:5401/scalar/v1`
- OpenAPI JSON: `http://localhost:5401/openapi/v1.json`
- HTTP requests: `ball-of-mud/WorkshopPlanner.Api/requests.http`
- Solution: `ball-of-mud/WorkshopPlanner.BallOfMud.slnx`

What to show:

- `Program.cs` wires one API project directly
- `Endpoints/WorkshopEndpoints.cs` now owns a much broader set of responsibilities:
  - workshop lifecycle
  - workshop administration
  - session maintenance
  - registrations
  - attendee check-in
- The same file mixes HTTP, validation, business rules, and persistence concerns
- `Data/WorkshopStore.cs` owns mutable in-memory state directly
- `Models/WorkshopModels.cs` contains both internal state and API contract types

### Clean sample: Onion Architecture + Vertical Slices

```bash
cd demos/day4/clean/WorkshopPlanner.Api
dotnet run
```

Useful URLs and files:

- API root: `http://localhost:5402`
- Scalar UI: `http://localhost:5402/scalar/v1`
- OpenAPI JSON: `http://localhost:5402/openapi/v1.json`
- HTTP requests: `clean/WorkshopPlanner.Api/requests.http`
- Solution: `clean/WorkshopPlanner.Clean.slnx`

What to show:

- `WorkshopPlanner.Domain` protects workshop invariants
- `WorkshopPlanner.Application/Features/Workshops/` organizes one folder per use case
- `WorkshopPlanner.Infrastructure/Repositories/InMemoryWorkshopRepository.cs` keeps storage behind an application abstraction
- `WorkshopPlanner.Api/Features/Workshops/` keeps HTTP endpoints thin and delegates through MediatR
- The expanded slices now cover workshop admin, session maintenance, registrations, and check-in without collapsing back into one giant file

## Suggested flow

1. Start the monolith sample and walk through the giant endpoint file.
2. Show how validation, business rules, transport mapping, and data mutation are all coupled together across many route shapes.
3. Use the `.http` files to jump through these capability areas in both demos:
   - workshop lifecycle and admin
   - session maintenance
   - registrations
   - attendee check-in
4. Start the clean sample and compare the same use cases slice by slice.
5. Contrast project boundaries from Onion Architecture with feature boundaries from Vertical Slices.
6. Emphasize that both samples solve the same broader problem, but the clean sample keeps change boundaries local while the monolith keeps growing sideways.
