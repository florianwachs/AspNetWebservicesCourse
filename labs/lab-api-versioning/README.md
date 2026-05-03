# Lab: Version the TechConf API

## Overview

In this lab you'll evolve a small **TechConf Minimal API** from a single unversioned contract into a properly versioned .NET 10 API using **Asp.Versioning** and the built-in **OpenAPI** stack.

The starter already exposes a flat event response for existing consumers. Your job is to introduce **URL-path versioning**, keep the existing V1 shape for backward compatibility, add a richer V2 contract, deprecate V1, and generate **one OpenAPI document per version** with **Scalar** as the UI.

## Learning Objectives

1. Configure API versioning for Minimal APIs with `Asp.Versioning`
2. Use **URL segment versioning** with `/api/v{version}/...`
3. Serve two API contracts side-by-side without breaking old clients
4. Mark an older version as deprecated and report supported versions in response headers
5. Generate separate OpenAPI documents for each version and expose them through Scalar

## Scenario

The TechConf API started with a simple event contract:

```json
{ "id": 1, "title": "TechConf 2026", "startDate": "2026-09-15T09:00:00", "location": "Main Hall, Munich" }
```

That worked for early consumers, but newer clients now need more detail:

- event description
- end date
- structured venue information
- registration counts
- publishing status

You cannot break the existing clients, so you need to keep **V1** alive while introducing **V2**.

## Getting Started

```bash
cd labs/lab-api-versioning/exercise/TechConf.Api
dotnet run
```

Open https://localhost:5001/scalar/v1 to inspect the API docs while you work.

## Tasks

### Task 1 — Configure API versioning services

Open `Program.cs` and replace the simple OpenAPI setup with the version-aware configuration:

- add `AddApiVersioning(...)`
- use `UrlSegmentApiVersionReader`
- enable `ReportApiVersions`
- add the versioned API explorer with `GroupNameFormat = "'v'VVV"`
- enable URL substitution for route templates
- register the version-aware `AddOpenApi()`

The required NuGet packages are already referenced in the project so you can focus on the implementation.

### Task 2 — Move the current API to V1

Open `Endpoints/EventEndpoints.cs` and convert the current unversioned `/api/events` endpoints into:

- `GET /api/v1/events`
- `GET /api/v1/events/{id}`

Keep the current flat V1 response shape so existing consumers can migrate with minimal surprises.

### Task 3 — Add V2 endpoints

Still in `EventEndpoints.cs`, expose a richer V2 contract:

- `GET /api/v2/events`
- `GET /api/v2/events/{id}`

Use the provided V2 DTOs so the new contract returns:

- `description`
- `endDate`
- nested `venue`
- `registeredCount`
- `status`

### Task 4 — Deprecate V1

Mark API version `1.0` as deprecated while keeping it available.

Verify that V1 responses include version-reporting headers such as:

- `api-supported-versions`
- `api-deprecated-versions`

### Task 5 — Generate one OpenAPI document per version

Update the development-only OpenAPI/Scalar setup in `Program.cs` so that:

- `/openapi/v1.json` only contains V1 endpoints
- `/openapi/v2.json` only contains V2 endpoints
- Scalar shows both documents in its version switcher

### Task 6 — Compare both versions

Use `requests.http` to verify:

1. V1 returns the flat `location` string
2. V2 returns the structured `venue` object
3. V1 is reported as deprecated
4. both OpenAPI documents are available separately

## Stretch Goals

1. Add a **version-neutral** endpoint such as `/health`
2. Add a **Sunset** policy for V1 so clients know when it will be retired
3. Support both **URL segment** and **header** versioning at the same time

## Solution

A complete reference solution is available in the [`solution/`](./solution/) directory.
