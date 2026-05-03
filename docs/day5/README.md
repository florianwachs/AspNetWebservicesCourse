# Day 5 — Testing, Versioning, Observability & Deployment

## 🎯 Learning Objectives

By the end of Day 5, you will be able to:

- Write integration tests with WebApplicationFactory and Testcontainers
- Version APIs safely with Asp.Versioning
- Implement health checks for production readiness
- Add custom observability (traces, metrics, structured logging)
- Set up CI/CD with GitHub Actions

## 📅 Schedule

| Time | Topic | Type |
|------|-------|------|
| 09:00–09:15 | [Morning Warm-Up Quiz](#-morning-warm-up) | Discussion |
| 09:15–10:30 | [Integration Testing](01-integration-testing.md) | Lecture + Live Coding |
| 10:30–10:45 | ☕ Break | — |
| 10:45–11:15 | [API Versioning](02-api-versioning.md) | Lecture |
| 11:15–11:45 | [Health Checks](03-health-checks.md) | Lecture + Demo |
| 11:45–12:00 | [Observability](04-observability.md) (intro) | Lecture |
| 12:00–13:00 | 🍽️ Lunch Break | — |
| 13:00–13:30 | [Observability](04-observability.md) (deep-dive) | Live Coding |
| 13:30–14:00 | [CI/CD with GitHub Actions](05-cicd.md) | Lecture |
| 14:00–14:15 | ☕ Break | — |
| 14:15–16:30 | [Capstone Project](../../labs/capstone/) | Hands-on Lab |
| 16:30–17:00 | Capstone Presentations, Course Recap & Next Steps | Discussion |

## 🌅 Morning Warm-Up

Quick review questions from Day 4:

1. What problem does HybridCache's stampede protection solve?
2. What are the three SignalR transport types?
3. What happens when a circuit breaker transitions to "Open" state?
4. Why do background services need `IServiceScopeFactory` for scoped dependencies?

## 📖 Topics

1. **[Integration Testing](01-integration-testing.md)** — Test pyramid, WebApplicationFactory, Testcontainers, Respawn, testing auth, testing SignalR, test data builders
2. **[API Versioning](02-api-versioning.md)** — Breaking changes, versioning strategies, Asp.Versioning, multiple OpenAPI docs, deprecation
3. **[Health Checks](03-health-checks.md)** — Liveness vs readiness probes, custom checks, Aspire integration, Kubernetes readiness
4. **[Observability](04-observability.md)** — Three pillars, OpenTelemetry, custom traces & metrics, structured logging, Aspire Dashboard
5. **[CI/CD with GitHub Actions](05-cicd.md)** — Workflow anatomy, .NET CI best practices, Docker publishing, environments, secrets

## 🧪 Lab

**[Capstone Project — Speaker Submission Portal](../../labs/capstone/)**: Independent project combining all 5 days of skills. Requirements-only starter with 6-feature backlog.

**Optional follow-up: [Version the TechConf API](../../labs/lab-api-versioning/)**: Guided Minimal API exercise for `Asp.Versioning`, URL-path versioning, deprecation, and one OpenAPI document per version.


## 📚 Key Packages Introduced Today

| Package | Purpose |
|---------|---------|
| `Microsoft.AspNetCore.Mvc.Testing` | Integration test host |
| `Testcontainers.PostgreSql` | Real DB in tests |
| `Respawn` | Fast test DB cleanup |
| `Asp.Versioning.Http` | API versioning |
| `AspNetCore.HealthChecks.*` | Health check extensions |
| `OpenTelemetry.Extensions.Hosting` | Telemetry pipeline |
