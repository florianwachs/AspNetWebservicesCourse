# Lab: Dependency Injection with Application Builder

## Overview

In this small optional lab you use `Host.CreateApplicationBuilder(...)` in a console app to understand how the .NET dependency injection container works outside ASP.NET Core.

You will register services, build the host, create manual scopes, and observe how `Singleton`, `Scoped`, and `Transient` lifetimes behave.

## Learning Objectives

By the end of this lab, you will:

1. Register services with `builder.Services` in a console application.
2. Use constructor injection without controllers or Minimal API endpoints.
3. Create scopes manually with `app.Services.CreateScope()`.
4. Observe the difference between transient and scoped services across multiple resolves.

## Project Structure

```text
exercise/
  TechConf.ConsoleDi/

solution/
  TechConf.ConsoleDi/
```

## Getting Started

```bash
cd labs/lab-di-console/exercise/TechConf.ConsoleDi
dotnet run
```

## Tasks

### Task 1: Register the services

Open `Program.cs` and register these services:

- `OperationMarker` as **transient**
- `WorkshopScope` as **scoped**
- `AgendaPrinter` as **transient**

`TimeProvider.System` is already registered for you as a singleton.

### Task 2: Build the host and create scope #1

After `builder.Build()`, create a first scope and resolve `AgendaPrinter` **twice** from the same scope. Call `Print(...)` for:

- `"Minimal APIs"`
- `"Dependency Injection"`

Observe that the scoped marker stays the same while the transient marker changes.

### Task 3: Create scope #2

Create a second scope and resolve `AgendaPrinter` once more. Call `Print(...)` for:

- `"OpenAPI & Scalar"`

The scoped marker should now be different from scope #1.

## Stretch Goal

Replace `TimeProvider.System` with a fake `TimeProvider` so the console output becomes deterministic for tests or demos.

## Solution

A complete reference solution is available in the `solution/` directory.
