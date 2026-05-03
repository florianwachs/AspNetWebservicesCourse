# Capstone Reference Implementation

This directory now contains the full **Speaker Submission Portal** reference solution for the course. It is intended as the final “put the pieces together” sample that you can inspect after working through the labs.

## What it demonstrates

- **.NET Aspire starter** with AppHost, ServiceDefaults, PostgreSQL, Redis, API, and Vite React frontend
- **ASP.NET Identity + JWT bearer auth** for a stateless SPA-to-API flow
- **Vertical Slice Architecture** with MediatR and FluentValidation
- **Policy-based authorization** with both roles and custom claims
- **REST guideline coverage**:
  - plural resource URIs
  - versioned public API (`/api/v1/speakers`, `/api/v2/speakers`)
  - ProblemDetails and validation responses
  - filtering, searching, sorting, pagination
  - `ETag` / `Last-Modified` on speaker detail
  - rate limiting on the public speaker API
- **Production-oriented features**:
  - HybridCache-backed speaker reads
  - SignalR proposal review notifications
  - background worker for acceptance processing
  - OpenTelemetry/health defaults through ServiceDefaults

## Main flows

1. Browse the **public speaker directory** through the versioned REST API.
2. Register or sign in as a speaker and maintain **your speaker profile**.
3. Create proposal drafts and **submit them for review**.
4. Sign in as an organizer to **accept or reject proposals**.
5. Watch the SPA react to **SignalR notifications** and cached read-model updates.

## Run

```bash
cd labs/capstone/reference
dotnet run --project TechConf.AppHost --launch-profile https
```

The Aspire dashboard will open on the AppHost URL and start:

- `api`
- `web`
- PostgreSQL
- Redis

## Demo users

| Role | Email | Password |
|---|---|---|
| Organizer | `organizer@techconf.dev` | `Organizer123!` |
| Speaker | `sarah.speaker@techconf.dev` | `Speaker123!` |
| Speaker | `maya.api@techconf.dev` | `Speaker123!` |
| Attendee | `attendee@techconf.dev` | `Attendee123!` |

## Suggested reading order

1. `TechConf.AppHost/Program.cs` — Aspire orchestration
2. `TechConf.Api/Program.cs` — API composition root
3. `TechConf.Api/Features/Speakers/` — versioned public speaker API + profile flow
4. `TechConf.Api/Features/Proposals/` — proposal submission and review workflow
5. `techconf-frontend/src/App.tsx` — React SPA integrating public and authenticated flows
