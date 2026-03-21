# Lab 2: Persist and Validate the TechConf Domain

## Overview

In this lab, you'll add data persistence with **Entity Framework Core 10** and **PostgreSQL**, implement input validation using **DataAnnotations** and **FluentValidation**, and add standardized error handling with **Problem Details (RFC 9457)**.


## Learning Objectives

- Configure Entity Framework Core with PostgreSQL
- Define entity relationships (one-to-many, many-to-many)
- Run database migrations
- Implement input validation
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

### Task 3: Implement Repository Methods
Complete the `EventRepository` methods using async EF Core queries. Use `Include` for eager loading, avoid N+1 queries.

### Task 4: Refactor Endpoints to Use Database
Update `EventEndpoints.cs` to inject `AppDbContext` (or `IEventRepository`) and use async database operations instead of the in-memory list.

### Task 5: Add Validation
Implement `CreateEventValidator` using FluentValidation:
- Name is required, max 200 chars
- Date must be in the future
- City is required, max 100 chars
- Wire up the validation endpoint filter

## Stretch Goals

1. **Pagination**: Add `?page=1&pageSize=20` to GET /api/events with total count in response
2. **Data Seeding**: Add initial seed data in `AppDbContext.OnModelCreating`

## Solution

A complete reference solution is available in the `solution/` directory.
