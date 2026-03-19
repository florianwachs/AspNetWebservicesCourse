# Day 1 — .NET 10, Minimal APIs, Dependency Injection & OpenAPI

## 🎯 Learning Objectives

By the end of Day 1, you will be able to:

- Explain the .NET 10 platform architecture and project structure
- Apply modern idiomatic C# syntax and patterns used in this course
- Build RESTful APIs using the Minimal API pattern
- Explain how dependency injection powers Minimal API parameter injection
- Configure OpenAPI documentation with Scalar UI
- Apply HTTP fundamentals (methods, status codes, content negotiation)
- Use TypedResults for compile-time-safe API responses

## 📖 Topics

1. **[C# Crash Course](00-csharp-crash-course.md)** — beginner-friendly console apps, modern idiomatic C#, NuGet basics, async/await, null safety, records, and core base classes like `HttpClient`
2. **[.NET 10 Platform Overview](01-dotnet10-overview.md)** — SDK, CLI, project anatomy, Program.cs evolution, global usings, hot reload
3. **[Minimal APIs Deep-Dive](02-minimal-apis.md)** — Endpoint mapping, parameter binding, TypedResults, route groups, endpoint filters, organizing large APIs
4. **[Dependency Injection for Minimal APIs](02a-dependency-injection.md)** — Container basics, service lifetimes, parameter injection, and manual scopes in console apps
5. **[HTTP Fundamentals](03-http-fundamentals.md)** — REST principles, HTTP methods, status codes, content negotiation, CORS
6. **[OpenAPI & Scalar](04-openapi-scalar.md)** — Built-in OpenAPI support, Scalar documentation UI, .http files, document customization

## 🧪 Lab

**[Lab 1 — Events API](../../labs/lab1-events-api/)**: Build a complete CRUD API for TechConf events using Minimal APIs with in-memory storage, TypedResults, and Scalar documentation.
