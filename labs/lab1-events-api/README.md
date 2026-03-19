# Lab 1: Build the Events API

## Overview

In this lab, you'll build your first modern web API using **Minimal APIs** in .NET 10. You'll create a complete CRUD API for managing tech conference events, organized with route groups and using TypedResults for proper HTTP responses.
Í
## Learning Objectives

By the end of this lab, you will:

- Create a .NET 10 web API project
- Implement CRUD endpoints using Minimal APIs
- Organize endpoints with route groups
- Use TypedResults for type-safe HTTP responses
- Generate API documentation with built-in OpenAPI and Scalar
- Test your API using .http files

## Getting Started

```bash
cd labs/lab1-events-api/exercise/TechConf.Api
dotnet run
```

Open https://localhost:5001/scalar/v1 to view the API documentation.

## Tasks

### Task 1: Implement GET /api/events

Open `EventEndpoints.cs` and implement the `GetAllEvents` method to return all events from the in-memory list.

### Task 2: Implement GET /api/events/{id}

Implement the `GetEventById` method. Return `404 Not Found` if the event doesn't exist.

### Task 3: Implement POST /api/events

Implement the `CreateEvent` method. Generate a new ID, add the event to the list, and return `201 Created` with a Location header.

### Task 4: Implement PUT /api/events/{id}

Implement the `UpdateEvent` method. Return `404 Not Found` if the event doesn't exist, or `204 No Content` on success.

### Task 5: Implement DELETE /api/events/{id}

Implement the `DeleteEvent` method. Return `404 Not Found` if the event doesn't exist, or `204 No Content` on success.

### Task 6: Test with .http file

Open `requests.http` and run each request to verify your endpoints work correctly.

## Stretch Goals

1. **Query Filtering**: Add optional query parameters to `GET /api/events`:
   - `?city=Munich` — filter by city
   - `?from=2026-06-01` — filter events after a date
   - `?name=tech` — search by name (case-insensitive contains)

2. **Summary Endpoint**: Add `GET /api/events/{id}/summary` returning only `name`, `date`, and `city`.

## Solution

A complete reference solution is available in the `solution/` directory.
