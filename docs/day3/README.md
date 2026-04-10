# Day 3 — .NET Aspire, Authentication & Architecture

## 🎯 Learning Objectives

By the end of Day 3, you will be able to:

- Orchestrate multi-service applications with .NET Aspire
- Choose between ASP.NET Identity for simpler apps and JWT Bearer authentication with Keycloak for advanced setups
- Apply Vertical Slice Architecture to organize feature code
- Use MediatR for CQRS pattern implementation
- Build cross-cutting concerns with pipeline behaviors

## 📅 Schedule

| Time | Topic | Type |
|------|-------|------|
| 09:00–09:15 | [Morning Warm-Up Quiz](#-morning-warm-up) | Discussion |
| 09:15–10:30 | [.NET Aspire Deep-Dive](01-aspire.md) | Lecture + Demo |
| 10:30–10:45 | ☕ Break | — |
| 10:45–12:00 | [Authentication & Authorization](02-authentication.md) | Lecture + Live Coding |
| 12:00–13:00 | 🍽️ Lunch Break | — |
| 13:00–14:15 | [Architecture Patterns (VSA, MediatR, CQRS)](03-architecture.md) | Lecture + Live Coding |
| 14:15–14:30 | ☕ Break | — |
| 14:30–16:30 | [Lab 3 — Aspire, Auth & Architecture](../../labs/lab3-aspire-auth-architecture/) | Hands-on Lab |
| 16:30–17:00 | Recap, Q&A & Day 3 Checkpoint | Discussion |

## 🌅 Morning Warm-Up

Quick review questions from Day 2:

1. What is the difference between `AddScoped` and `AddSingleton`?
2. What is the "captive dependency" problem?
3. How does `AsNoTracking()` improve EF Core performance?
4. What HTTP status code does a validation failure return?

## 📖 Topics

1. **[.NET Aspire Deep-Dive](01-aspire.md)** — AppHost, ServiceDefaults, Dashboard, integrations, service discovery, WaitFor, health-based startup, environment configuration
2. **[Authentication & Authorization](02-authentication.md)** — identity strategy selection, ASP.NET Identity vs Keycloak, OAuth2/OIDC concepts, JWT token anatomy, Keycloak setup walkthrough, policy-based & resource-based authorization, testing auth
3. **[Architecture Patterns](03-architecture.md)** — Vertical Slice Architecture vs Clean Architecture, MediatR request/handler pattern, CQRS, pipeline behaviors, refactoring walkthrough

## 🧪 Labs

**Advanced track — [Lab 3 — Aspire, Auth & Architecture](../../labs/lab3-aspire-auth-architecture/)**: Follow the advanced Keycloak/JWT track: add Aspire orchestration with PostgreSQL and Keycloak, implement JWT authentication, refactor to VSA with MediatR.

Scaffold level: **50%** (Aspire AppHost configured, implement auth and architecture refactoring)

**Simpler auth track — [Alternative Lab: ASP.NET Identity with Aspire and React](../../labs/lab-identity-react/)**: Practice the app-local authentication path with ASP.NET Identity, cookie authentication, Aspire orchestration, and a React frontend.

Scaffold level: **50–60%** (starter code provided for both backend and frontend auth flow)

## 📚 Key Packages Introduced Today

| Package | Purpose |
|---------|---------|
| `Aspire.Hosting` | App orchestration |
| `Aspire.Hosting.PostgreSQL` | PostgreSQL container integration |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | Built-in ASP.NET Identity stores |
| `Aspire.Hosting.Keycloak` | Keycloak identity provider |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT token validation |
| `MediatR` | Mediator/CQRS pattern |
