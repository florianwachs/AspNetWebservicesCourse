# Day 4 — Architecture, Real-time Communication & Integration Testing

## 🎯 Learning Objectives

By the end of Day 4, you will be able to:

- explain when a simple layered design is enough and when a move to Vertical Slice Architecture or Onion Architecture is justified
- structure feature-first request handling with MediatR, CQRS, validators, and pipeline behaviors
- build real-time features with SignalR using hubs, groups, `IHubContext`, reconnection handling, and authentication
- choose the right testing level for a backend change: unit, integration, or E2E
- write focused API integration tests with `WebApplicationFactory` and distributed-app integration tests with `Aspire.Hosting.Testing`
- use real infrastructure in tests with Testcontainers and reset database state reliably with Respawn

## 📖 Topics

1. **[Architecture Patterns](01-architecture.md)** — decision-making around layered architecture, Vertical Slice Architecture, Onion Architecture, MediatR, CQRS, pipeline behaviors, and when heavier patterns like repositories, event sourcing, or actors are actually worth the cost
2. **[SignalR — Real-time Communication](02-signalr.md)** — transport negotiation, strongly typed hubs, groups, `IHubContext`, JavaScript/TypeScript clients, reconnection, authentication, and scaling with a Redis backplane
3. **[Integration Testing — Confidence in Your API](03-integration-testing.md)** — testing pyramid, `WebApplicationFactory` vs `Aspire.Hosting.Testing`, open-box vs closed-box testing, real databases with Testcontainers, cleanup with Respawn, and browser-driven checks on top of the Aspire harness

## 🧪 Labs

**Architecture track — [Vertical Slice refactor](../../labs/lab-architecture-vertical-slices/), [MediatR/CQRS follow-up](../../labs/lab-mediatr-cqrs/), and [Onion refactor](../../labs/lab-architecture-onion/)**: Start from a simple API, move toward feature-first slices, then add stronger domain boundaries only when the problem really needs them.

**Real-time track — [SignalR multiplayer lab](../../labs/lab-signalr-multiplayer/)**: Practice hub-based communication, server-pushed updates, and multi-client interaction with SignalR.

**Testing track — [Integration testing lab](../../labs/lab-testing/)**: Build confidence with API integration tests and Aspire-based distributed application tests using real infrastructure.

## 📚 Key Packages Introduced Today

| Package | Purpose |
|---------|---------|
| `MediatR` | Mediator/CQRS pattern for feature-oriented request handling |
| `Microsoft.AspNetCore.SignalR` | Real-time server-side communication |
| `@microsoft/signalr` | JavaScript/TypeScript SignalR client |
| `Aspire.Hosting.Testing` | Closed-box integration testing through the AppHost |
| `Microsoft.AspNetCore.Mvc.Testing` | In-process integration testing for a single ASP.NET Core service |
| `Testcontainers.PostgreSql` | Real PostgreSQL infrastructure for integration tests |
| `Respawn` | Fast database reset between integration tests |
