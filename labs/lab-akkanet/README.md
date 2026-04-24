# Lab: Akka.NET Seat Reservations with Akka.Hosting and Aspire

## Overview

In this elective lab you build a **TechConf seat-reservation API** where one hot piece of state must stay correct while many requests try to change it. That is exactly the kind of problem where the **actor model** becomes interesting.

The lab uses:

- **Akka.NET** for the actor runtime
- **Akka.Hosting** for modern, HOCON-less configuration
- **ASP.NET Core Minimal APIs** for the HTTP layer
- **.NET Aspire 13.2.2** for orchestration and diagnostics

The starter already includes the AppHost, a Vite + React dashboard, the actor registration via `AddAkka(...)`, a validation-actor scaffold, and the seeded TechConf sessions. Your job is to finish the HTTP-to-actor flow, wire the actor-to-actor communication, and keep the reservation invariant inside the state-owning actor.

## Learning Objectives

By the end of this lab, you will:

1. Explain why actors are a good fit for shared, long-lived in-memory state under concurrency.
2. Configure an Akka.NET application with **`Akka.Hosting`** instead of standalone HOCON files.
3. Use `ActorRegistry` and `Ask(...)` to connect Minimal API endpoints to a top-level actor.
4. Use actor-to-actor communication so one actor can validate a request before another actor mutates state.
5. Keep the seat-capacity invariant inside the state-owning actor instead of spreading it across controllers or services.
6. Run and inspect the application through an Aspire AppHost.

## Project Structure

```text
exercise/
  TechConf.Akka.AppHost/
  TechConf.Akka.Api/
  TechConf.Akka.ServiceDefaults/
  TechConf.Akka.Web/
  TechConf.Akka.slnx

solution/
  TechConf.Akka.AppHost/
  TechConf.Akka.Api/
  TechConf.Akka.ServiceDefaults/
  TechConf.Akka.Web/
  TechConf.Akka.slnx
```

## Getting Started

### 1. Run with Aspire

```bash
cd labs/lab-akkanet/exercise/TechConf.Akka.AppHost
dotnet run
```

Open the Aspire dashboard and launch the `api` and `web` resources.

The Vite app is added to the AppHost with `AddViteApp(...)` and uses an Aspire-aware `/api` proxy, so the dashboard can discover the API automatically when both resources run under the AppHost.

### 2. Optional: run the API directly

```bash
cd labs/lab-akkanet/exercise/TechConf.Akka.Api
dotnet run
```

The exercise API launch settings use:

- HTTP: `http://localhost:5246`
- HTTPS: `https://localhost:7157`

### 3. Optional: run the frontend directly

```bash
cd labs/lab-akkanet/exercise/TechConf.Akka.Web
npm install
npm run dev
```

When the frontend runs outside the AppHost, the Vite proxy falls back to the exercise API HTTPS endpoint above.

Use `requests.http`, the Vite dashboard, or any HTTP client to inspect the seeded sessions.

## Lab Scenario

The API starts with three seeded TechConf sessions. Each session has a fixed capacity, and the reservation actor is the **single state owner**. A second validation actor helps decide whether a change is allowed before the reservation actor mutates state.

That means the actor collaboration decides whether:

- a seat can still be reserved,
- an attendee already has a reservation,
- or a release request is valid.

The HTTP layer stays thin while the reservation actor protects the invariant.

## Tasks

### Task 1 - Query sessions through the actor

Open `TechConf.Akka.Api/Endpoints/ReservationEndpoints.cs`.

- Implement `GET /api/sessions` by asking the actor for `ListSessions`.
- Implement `GET /api/sessions/{id}` by asking for `GetSession(id)`.
- Map `SessionNotFound` to `404 Not Found`.

### Task 2 - Build the validation actor for reservations

Open `TechConf.Akka.Api/Actors/ReservationValidationActor.cs`.

- Implement reserve validation so the second actor:
- returns `ReservationRejectedAlreadyReserved` when the attendee already holds a seat,
- returns `ReservationRejectedFull` when capacity is exhausted,
- otherwise replies with `ReservationAccepted`.

### Task 3 - Ask the validation actor before reserving

Back in `TechConf.Akka.Api/Actors/SeatReservationsActor.cs`, finish the reserve flow.

- Keep `SeatReservationsActor` as the state owner.
- Ask the validation actor for `ValidateSeatReservation(...)`.
- Map the validation response back to `SeatReserved`, `SeatAlreadyReserved`, or `SessionFull`.
- Only mutate the in-memory session state after the validation actor approves the reservation.

### Task 4 - Connect the reserve endpoint

Back in `ReservationEndpoints.cs`, finish `POST /api/sessions/{id}/reserve`.

- Send `ReserveSeat(id, attendeeId)` to the actor.
- Return:
  - `200 OK` for `SeatReserved`
  - `404 Not Found` for `SessionNotFound`
  - `409 Conflict` for `SessionFull` and `SeatAlreadyReserved`

### Task 5 - Validate release requests through the second actor

Finish the release flow inside both actors:

1. In `ReservationValidationActor.cs`, implement `ValidateSeatRelease(...)`.
2. In `SeatReservationsActor.cs`, ask the validation actor before removing the attendee from the session.

`SeatReservationsActor` should still return:

- `SessionNotFound` for unknown session ids
- `SeatNotReserved` when no reservation exists
- `SeatReleased` after a valid release

### Task 6 - Connect the release endpoint

Back in `ReservationEndpoints.cs`, finish `POST /api/sessions/{id}/release`.

Return:

- `200 OK` for `SeatReleased`
- `404 Not Found` for `SessionNotFound`
- `409 Conflict` for `SeatNotReserved`

### Task 7 - Validate the invariant

Open `TechConf.Akka.Api/requests.http` or use the Vite dashboard and exercise the flow:

1. list the seeded sessions,
2. reserve a seat,
3. reserve the same seat again,
4. fill a session to capacity,
5. release a seat and reserve again.

The important question is not just whether the endpoint returns something, but whether the **reservation actor still owns the state while the second actor participates through actor-to-actor communication**.

## Stretch Goals

1. Add a small load-generator console app and wire it into the Aspire AppHost to create visible concurrent reservation pressure.
2. Add a `POST /api/sessions/{id}/reset` endpoint that clears all reservations for one session.
3. Return richer error payloads or Problem Details while keeping the actor message contract explicit.

## Solution

A complete reference implementation, including the polished dashboard UI, is available in the `solution/` directory.
