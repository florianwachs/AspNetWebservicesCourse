# Lab: Explore OpenTelemetry with Aspire

## Overview

In this lab, you will work with a **fully functional .NET Aspire application** that already emits **logs, traces, and metrics**.

Instead of building the app from scratch, your goal is to **explore how the existing telemetry behaves** and then **tinker with the application** by adding a small endpoint and a few extra logging statements.

Think of this lab as a safe sandbox for observability: poke it, change it, run it again, and watch the dashboard light up.

## Learning Objectives

By the end of this lab, you will:

- Run a .NET Aspire application with a backend API, PostgreSQL, and a frontend
- Inspect **logs, traces, and metrics** in the Aspire dashboard
- Add a simple **Minimal API endpoint**
- Add **structured logging** with `ILogger`
- Correlate code changes with what you see in the observability tools

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) running
- [Node.js LTS](https://nodejs.org/) installed

## Getting Started

From the lab folder, open the Aspire solution:

```bash
cd labs/lab-opentelemetry/AspireOpenTelemetry
```

If needed, install the frontend dependencies once:

```bash
cd frontend
npm install
cd ../AspireOpenTelemetry.AppHost
```

Start the Aspire AppHost:

```bash
dotnet run
```

Once the app is running:

- Open the **Aspire Dashboard**
- Open the **web frontend** from the dashboard
- Click around and trigger a few actions so the app produces telemetry
- Inspect the **server** logs, traces, and metrics in the dashboard

## Where to Tinker

The most useful files for this lab are:

- `AspireOpenTelemetry/AspireOpenTelemetry.AppHost/AppHost.cs` — Aspire orchestration for PostgreSQL, server, and frontend
- `AspireOpenTelemetry/AspireOpenTelemetry.Server/Program.cs` — Minimal API endpoints
- `AspireOpenTelemetry/AspireOpenTelemetry.Server/Tariffs/TariffService.cs` — business logic and structured logging
- `AspireOpenTelemetry/AspireOpenTelemetry.Server/Tariffs/TariffTelemetry.cs` — custom metrics and activity source definitions

## Tasks

### Task 1: Explore the Existing Telemetry

Run the application and trigger a few existing actions in the UI.

While doing that, inspect the Aspire dashboard and answer questions like:

- Which requests create traces?
- Which actions write logs?
- Which custom metrics are already recorded?
- Which endpoint appears to generate the most interesting telemetry?

You do not need to write formal answers, but you should be able to point out at least one example of each signal type: **log**, **trace**, and **metric**.

### Task 2: Add a Small Endpoint

Open `AspireOpenTelemetry.Server/Program.cs` and add one small endpoint of your own.

Keep it simple. For example, you could add:

- `GET /api/tariffs/ping`
- `GET /api/tariffs/summary`
- `GET /api/tariffs/telemetry-hint`

Your endpoint should return a small JSON response and be easy to call repeatedly.

Good candidates for the response are:

- the current UTC time
- a short status message
- the current scenario name
- the average tariff or absurdity index from the latest batch

### Task 3: Add Structured Logging

Open `AspireOpenTelemetry.Server/Tariffs/TariffService.cs` and add a few extra logging statements.

Try to add logs that make the app easier to understand when reading the dashboard output. For example:

- log when your new endpoint is called
- log when a request falls back to a default value
- log a warning for an unusual or suspicious case

Use **structured logging** with named placeholders rather than string concatenation.

Examples of useful log levels:

- `LogInformation(...)`
- `LogWarning(...)`

### Task 4: Run It Again and Observe the Difference

Restart the app if needed and trigger your new endpoint a few times.

Then inspect the Aspire dashboard again and look for:

- your new endpoint in traces
- your new log messages in the server logs
- correlation between the request and the logs it produced

If your endpoint uses existing service code, check whether it also affects the metrics already defined in `TariffTelemetry.cs`.

## Stretch Goals

1. **Add a custom metric** — define a new counter in `TariffTelemetry.cs` and increment it from your new endpoint.
2. **Update the `.http` file** — replace the old sample request in `AspireOpenTelemetry.Server.http` with requests that match the tariff API.
3. **Add a query parameter** — let your new endpoint accept a simple query value such as a country code or a message.
4. **Create an intentional warning scenario** — add a code path that logs a warning only when a specific condition is met, then trigger it and find it in the dashboard.

## Notes

This lab already starts from a working baseline, so there is no separate “build the whole thing” exercise here.

The goal is to get comfortable with observability by making **small, low-risk changes** and immediately seeing their effect in the running system.
