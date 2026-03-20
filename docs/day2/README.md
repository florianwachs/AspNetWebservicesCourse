# Day 2 — Data Persistence, Validation & Error Handling

## 🎯 Learning Objectives

By the end of Day 2, you will be able to:

- Configure Entity Framework Core 10 with PostgreSQL
- Design entity relationships using Fluent API configuration
- Implement request validation using FluentValidation and built-in .NET 10 validation
- Return standardized error responses using RFC 9457 Problem Details


## 🌅 Morning Warm-Up

Quick review questions from Day 1:

1. What is the difference between `Results.Ok()` and `TypedResults.Ok()`?
2. How do route groups help organize a Minimal API?
3. What replaced Swashbuckle in .NET 10?
4. Name three HTTP status codes and when to use them.

## 📖 Topics

1. **[Entity Framework Core 10](01-entity-framework.md)** — DbContext, Fluent API, relationships, migrations, LINQ queries, N+1 prevention, new EF Core 10 features (LeftJoin, JSON columns)
2. **[Validation](03-validation.md)** — .NET 10 built-in validation, FluentValidation, endpoint filters, ValidationProblemDetails
3. **[Problem Details & Error Handling](04-problem-details.md)** — RFC 9457, IExceptionHandler, global error handling strategies

## 🧪 Lab

**[Lab 2 — Persistence & Validation](../../labs/lab2-persistence-validation/)**: Add PostgreSQL persistence with EF Core, implement FluentValidation rules, and handle errors with Problem Details.

Scaffold level: **60%** (DbContext skeleton provided, write configurations and validation)

## 📚 Key Packages Introduced Today

| Package | Purpose |
|---------|---------|
| `Microsoft.EntityFrameworkCore` | ORM framework |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | PostgreSQL provider |
| `Microsoft.EntityFrameworkCore.Design` | Migration tooling |
| `FluentValidation.DependencyInjectionExtensions` | Validation with DI |
