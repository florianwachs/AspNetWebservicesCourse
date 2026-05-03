# postgres-extra

This image behaves like the official `postgres` image and adds these extensions:

- `vector`
- `vectorscale`
- `pg_textsearch`
- `age`

It also enables:

```conf
shared_preload_libraries = 'pg_textsearch,age'
```

On first initialization of a fresh data directory, it runs:

```sql
CREATE EXTENSION IF NOT EXISTS vector;
CREATE EXTENSION IF NOT EXISTS vectorscale CASCADE;
CREATE EXTENSION IF NOT EXISTS pg_textsearch;
CREATE EXTENSION IF NOT EXISTS age;
```

Apache AGE is ready on fresh database initialization, but AGE queries still need the usual per-session setup before calling `cypher(...)`:

```sql
LOAD 'age';
SET search_path = ag_catalog, "$user", public;
```

Quick smoke test:

```sql
LOAD 'age';
SET search_path = ag_catalog, "$user", public;
SELECT create_graph('demo_graph');
SELECT *
FROM cypher('demo_graph', $$
    CREATE (:Node {name: 'A'})
    RETURN 'ok'
$$) AS (result agtype);
```

## Use it like a regular Postgres container

Build the image:

```bash
docker build -t postgres-extra .
```

Run it like a normal Postgres container:

```bash
docker run --name postgres-extra \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=app \
  -p 5432:5432 \
  -v pgdata:/var/lib/postgresql/data \
  postgres-extra
```

Connect tools with:

- Host: `localhost`
- Port: `5432`
- Database: `app`
- User: `postgres`
- Password: `postgres`

Connection string:

```text
postgresql://postgres:postgres@localhost:5432/app
```

Notes:

- If the client runs in another container on the same Docker network, use the container name instead of `localhost`.
- The init SQL only runs when the data directory is created for the first time. Reusing an old volume will not rerun it.
- If you reuse an existing volume, run `CREATE EXTENSION age;` manually in the target database before using AGE there.
- Preloading `age` makes the server ready for AGE, but most clients should still issue `LOAD 'age'` and set the AGE search path per connection.

## Use with .NET Aspire 13.x and EF Core

Install the packages:

```bash
dotnet add <AppHost.csproj> package Aspire.Hosting.PostgreSQL --version 13.*
dotnet add <Api.csproj> package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL --version 13.*
```

In `AppHost.cs`, point the PostgreSQL resource at this Dockerfile, pin a host port for external tools, and set `POSTGRES_DB` to the database EF Core should use:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var user = builder.AddParameter("postgres-user", "postgres");
var password = builder.AddParameter("postgres-password", "postgres", secret: true);

var postgres = builder.AddPostgres("postgres", user, password, port: 5432)
    .WithDockerfile("../path/to/this/repo")
    .WithEnvironment("POSTGRES_DB", "app")
    .WithDataVolume();

var appdb = postgres.AddDatabase("appdb", "app");

builder.AddProject<Projects.Api>("api")
    .WithReference(appdb)
    .WaitFor(appdb);

builder.Build().Run();
```

In the consuming app:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "appdb");
```

When the app runs under Aspire, `WithReference(appdb)` injects the runtime connection automatically.

For design-time EF Core commands, add a normal local connection string named `appdb`, for example with user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:appdb" "Host=localhost;Port=5432;Database=app;Username=postgres;Password=postgres"
dotnet ef migrations add InitialCreate
dotnet ef database update
```

If you already built or pushed the image, use it directly instead of `WithDockerfile(...)`:

```csharp
var postgres = builder.AddPostgres("postgres", user, password, port: 5432)
    .WithImage("postgres-extra", "latest")
    .WithEnvironment("POSTGRES_DB", "app")
    .WithDataVolume();
```

If you already have manual EF Core registration, keep it and enrich it with Aspire:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("appdb")));

builder.EnrichNpgsqlDbContext<AppDbContext>();
```

Important:

- `connectionName: "appdb"` matches the Aspire database resource name, not the physical PostgreSQL database name.
- Pinning `port: 5432` lets desktop tools connect to the Aspire-managed container through `localhost:5432`.
- The example uses fixed dev credentials (`postgres` / `postgres`) so host tools and design-time EF Core commands can connect predictably.
- `AddDatabase(...)` creates databases after the server starts, but `/docker-entrypoint-initdb.d` only runs during first cluster initialization.
- Setting `POSTGRES_DB=app` keeps the extension setup aligned with the database EF Core uses.
- If you use a different database later, create the extensions there with a migration or manual SQL.
- For Apache AGE usage from application code, initialize each session with `LOAD 'age'` and `SET search_path = ag_catalog, "$user", public;` before issuing graph queries.
