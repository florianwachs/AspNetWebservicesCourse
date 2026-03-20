# Day 2 Demos — EF Core, DI, Validation & Error Handling

## Runnable samples

This folder now contains two finished reference samples you can run and debug:

### SQLite version

```bash
cd demos/day2/TechConf.Sqlite.Api
dotnet run
```

Useful URLs and files:

- API root: `http://localhost:5100`
- Scalar UI: `http://localhost:5100/scalar/v1`
- OpenAPI JSON: `http://localhost:5100/openapi/v1.json`
- HTTP requests: `TechConf.Sqlite.Api/requests.http`
- Migrations: `TechConf.Sqlite.Api/Data/Migrations/`

### PostgreSQL version

Start PostgreSQL in Docker with the latest image:

```bash
docker run --rm -d \
  --name techconf-day2-postgres \
  -e POSTGRES_PASSWORD=techconf \
  -e POSTGRES_DB=techconfdb \
  -p 5432:5432 \
  postgres:latest
```

Then run the sample:

```bash
cd demos/day2/TechConf.Postgres.Api
dotnet run
```

Useful URLs and files:

- API root: `http://localhost:5200`
- Scalar UI: `http://localhost:5200/scalar/v1`
- OpenAPI JSON: `http://localhost:5200/openapi/v1.json`
- HTTP requests: `TechConf.Postgres.Api/requests.http`
- Migrations: `TechConf.Postgres.Api/Data/Migrations/`

Both samples automatically apply migrations and seed demo data on startup. If you want to stop the PostgreSQL container later, run:

```bash
docker stop techconf-day2-postgres
```

## Demo 1: EF Core Setup

1. Add NuGet packages: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`
2. Create `AppDbContext` with `DbSet<Event>`
3. Show Fluent API configuration in `IEntityTypeConfiguration<Event>`
4. Register with `AddDbContext` and connection string
5. Create migration: `dotnet ef migrations add InitialCreate`
6. Apply migration: `dotnet ef database update`
7. Query with LINQ: `db.Events.ToListAsync()`

Key teaching moments:

- DbContext lifecycle (scoped by default)
- Fluent API vs Data Annotations
- Migration files and what they contain (dotnet tools)

## Demo 2: Entity Relationships

1. Add `Session` and `Speaker` entities
2. Configure one-to-many (Event → Sessions)
3. Configure many-to-many (Sessions ↔ Speakers)
4. Show `Include()` and `ThenInclude()` for eager loading
5. Demonstrate N+1 problem with and without Include

## Demo 3: Dependency Injection

1. Show lifetime differences: Transient vs Scoped vs Singleton
2. Create `IEventRepository` and `EventRepository`
3. Register with `AddScoped`
4. Inject into Minimal API endpoint parameters
5. Show what happens with wrong lifetime (Singleton DbContext!)

## Demo 4: Validation

1. Show .NET 10 built-in validation: `AddValidation()` with `[Required]`, `[StringLength]`
2. Create a FluentValidation validator with complex rules
3. Build a `ValidationFilter<T>` endpoint filter
4. Show validation error response format

## Demo 5: Problem Details

1. Add `AddProblemDetails()`, `UseExceptionHandler()`, `UseStatusCodePages()`
2. Throw an unhandled exception — show automatic 500 ProblemDetails
3. Create `NotFoundException` custom exception
4. Build `IExceptionHandler` with pattern matching
5. Show the ProblemDetails JSON response
