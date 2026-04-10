# Day 3 — .NET Aspire, Authentication & Architecture

## 🎯 Learning Objectives

By the end of Day 3, you will be able to:

- Orchestrate multi-service applications with .NET Aspire
- Choose between ASP.NET Identity for simpler apps and JWT Bearer authentication with Keycloak for advanced setups
- Apply Vertical Slice Architecture to organize feature code

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
**[Lab 3: ASP.NET Identity with Aspire and React](../../labs/lab-identity-react/)**: Practice the app-local authentication path with ASP.NET Identity, cookie authentication, Aspire orchestration, and a React frontend.

**[Lab 3: Keycloak with Aspire and React](../../labs/lab-keycloak-react/)**: Keycloak, JWT and React

## 📚 Key Packages Introduced Today

| Package | Purpose |
|---------|---------|
| `Aspire.Hosting` | App orchestration |
| `Aspire.Hosting.PostgreSQL` | PostgreSQL container integration |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | Built-in ASP.NET Identity stores |
| `Aspire.Hosting.Keycloak` | Keycloak identity provider |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT token validation |
| `MediatR` | Mediator/CQRS pattern |
