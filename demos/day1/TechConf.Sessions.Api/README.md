# TechConf Sessions API

This project is the **companion demo for day 1** of the course.

Use it together with:

- [Day 1 docs](../../../docs/day1/README.md)
- [Day 1 API Essentials Cheat Sheet](../../../cheatsheets/day1/README.md)

If you only keep one project open while reading the day 1 materials, use this one.

## Run it

From the repository root:

```bash
dotnet build demos/day1/TechConf.Sessions.Api/TechConf.Sessions.Api.csproj
dotnet run --project demos/day1/TechConf.Sessions.Api/TechConf.Sessions.Api.csproj --launch-profile http
```

Then open:

- Scalar UI: `http://localhost:5042/scalar/v1`
- OpenAPI JSON: `http://localhost:5042/openapi/v1.json`
- Manual request file: [`requests.http`](requests.http)

## Key files

- [`Program.cs`](Program.cs) — startup, DI registration, OpenAPI, and Scalar
- [`Endpoints/SessionEndpoints.cs`](Endpoints/SessionEndpoints.cs) — Minimal API routing, parameter binding, TypedResults, and validation
- [`Data/InMemorySessionRepository.cs`](Data/InMemorySessionRepository.cs) — in-memory storage plus LINQ-based sorting and lookup
- [`requests.http`](requests.http) — repeatable HTTP requests to exercise the API

## Next step

After you can run and explain this demo, move to [Lab 1 — Events API](../../../labs/lab1-events-api/README.md) to implement the same ideas yourself.
