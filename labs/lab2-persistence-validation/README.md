# Lab 2: Persist and Validate the TechConf Domain

## Overview

In this lab, you'll add data persistence with **Entity Framework Core 10** and **PostgreSQL**, implement input validation using **FluentValidation**, and add standardized error handling with **Problem Details (RFC 9457)**.


## Learning Objectives

- Configure Entity Framework Core with PostgreSQL
- Define entity relationships (one-to-many, many-to-many)
- Run database migrations
- Implement input validation with FluentValidation and endpoint filters
- Handle errors with Problem Details
- Use dependency injection effectively

## Getting Started

```bash
cd labs/lab2-persistence-validation/exercise/TechConf.Api
# Start PostgreSQL container
docker run -d --name techconf-db -e POSTGRES_PASSWORD=techconf -e POSTGRES_DB=techconfdb -p 5432:5432 postgres:latest
# Run the application
dotnet run
```

## Tasks

### Task 1: Configure Entity Relationships
Open `Data/Configurations/` and implement the entity configurations:
- `Event` has many `Sessions` (one-to-many)
- `Session` has many `Speakers` (many-to-many via join table)
- Configure required properties, max lengths, and cascade delete behavior

### Task 2: Register DbContext and Run Migrations
In `Program.cs`, register `AppDbContext` with the PostgreSQL connection string. Then create and apply the initial migration.

If the EF Core CLI is not installed yet, install it once:

```bash
dotnet tool install --global dotnet-ef
```

From `labs/lab2-persistence-validation/exercise/TechConf.Api`, create and apply the migration with:

```bash
dotnet ef migrations add Initial
dotnet ef database update
```

This generates the migration files in `Migrations/` and applies the schema to your PostgreSQL database.

### Task 3: Implement Repository Methods
Open `Repositories/EventRepository.cs` and complete the repository methods using async EF Core queries. Use `Include`/`ThenInclude` for eager loading and avoid N+1 queries.

### Task 4: Refactor Endpoints to Use the Repository
Update `Program.cs` and `Endpoints/EventEndpoints.cs` to register and inject `IEventRepository`, then use async repository calls instead of direct in-memory data access.

### Task 5: Add Validation
Implement `CreateEventValidator` using FluentValidation:
- Name is required, max 200 chars
- Date must be in the future
- City is required, max 100 chars
- Wire up the validation endpoint filter

### Task 6: Add Problem Details Error Handling
Implement `GlobalExceptionHandler` and wire it up in `Program.cs`:
- Register `GlobalExceptionHandler` and `ProblemDetails`
- Add the exception handler and status code pages middleware
- Map `NotFoundException` and other failures to appropriate Problem Details responses

## Stretch Goals

1. **Pagination**: Add `?page=1&pageSize=20` to GET /api/events with total count in response
2. **Data Seeding**: Add initial seed data using a dedicated `DbSeeder` (or equivalent startup seeding)

## Solution

A complete reference solution is available in the `solution/` directory.
