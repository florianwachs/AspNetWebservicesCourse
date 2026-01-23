# Module 02: ASP.NET Core Fundamentals

## Overview

Welcome to Module 02 of the Web Services with .NET course! This module introduces you to ASP.NET Core, Microsoft's modern, cross-platform framework for building web applications and APIs. You'll learn the fundamental concepts that underpin all ASP.NET Core applications, from simple REST APIs to complex microservices architectures.

This is a **foundational module** that sets you up for success throughout the entire course. Everything you learn here—from minimal APIs to dependency injection—will be used and expanded upon in later modules.

## What You'll Learn

By the end of this module, you will be able to:

- ✅ Understand HTTP fundamentals and REST principles
- ✅ Create RESTful APIs using ASP.NET Core Minimal APIs
- ✅ Implement dependency injection for clean, testable code
- ✅ Configure applications using appsettings.json, environment variables, and user secrets
- ✅ Build custom middleware and understand the request pipeline
- ✅ Handle model binding, validation, and API responses
- ✅ Integrate .NET Aspire from day one for cloud-ready applications

## Prerequisites

Before starting this module, you should have completed:

- **Module 01: C# Fundamentals** - You need solid C# knowledge including:
  - Classes, interfaces, and object-oriented programming
  - Collections (List, Dictionary, etc.)
  - LINQ basics
  - Async/await patterns
  - Records and modern C# features

You should also have:
- .NET 10 SDK installed
- Visual Studio 2022, Visual Studio Code, or JetBrains Rider
- Basic understanding of HTTP and web concepts
- Familiarity with JSON

## Module Structure

This module is organized into seven comprehensive lessons, each building on the previous:

### [01. Introduction to ASP.NET Core](01_introduction.md)
**Duration: 2 hours**

Foundation concepts you need before writing any code:
- HTTP protocol fundamentals (requests, responses, status codes, headers)
- REST architectural principles and design constraints
- Overview of ASP.NET Core architecture
- Understanding the .NET 10 web stack
- Key differences from .NET Framework and older versions

### [02. Minimal APIs](02_minimal_apis.md)
**Duration: 3 hours**

Your first ASP.NET Core applications:
- Creating your first minimal API
- Routing and route parameters
- HTTP methods (GET, POST, PUT, DELETE, PATCH)
- Route constraints and patterns
- Organizing endpoints in larger applications

**Lab Exercise**: Build a Chuck Norris Joke API with CRUD operations

### [03. Dependency Injection](03_dependency_injection.md)
**Duration: 3 hours**

The heart of ASP.NET Core architecture:
- What is dependency injection and why it matters
- The built-in DI container
- Service lifetimes: Singleton, Scoped, Transient
- Constructor injection patterns
- Best practices and common pitfalls

**Lab Exercise**: Refactor Chuck Norris API to use DI

### [04. Configuration](04_configuration.md)
**Duration: 2 hours**

Making your applications configurable and secure:
- The configuration system in ASP.NET Core
- appsettings.json and environment-specific settings
- Environment variables
- User secrets for local development
- Strongly-typed configuration with Options pattern

**Lab Exercise**: Add configurable features to your API

### [05. Middleware Pipeline](05_middleware.md)
**Duration: 3 hours**

Understanding how ASP.NET Core processes requests:
- The middleware pipeline concept
- Built-in middleware components
- Request pipeline order and why it matters
- Creating custom middleware
- Short-circuiting and terminal middleware

**Lab Exercise**: Add logging and error handling middleware

### [06. Model Binding and Validation](06_model_binding.md)
**Duration: 3 hours**

Working with data in APIs:
- Model binding from route, query, body, and headers
- Data annotations for validation
- Custom validation logic
- Response types and status codes
- Content negotiation

**Lab Exercise**: Add validation to Chuck Norris API

### [07. Aspire Integration](07_aspire_integration.md)
**Duration: 2 hours**

Building cloud-ready applications from day one:
- What is .NET Aspire and why use it?
- Adding Aspire to your projects
- ServiceDefaults and common configurations
- Observability: logging, metrics, and tracing
- Local development with Aspire Dashboard

**Lab Exercise**: Convert your API to an Aspire-enabled application

## Learning Path

This module follows a **progressive learning approach**:

```
Week 1: Foundation
├─ Day 1-2: Introduction + HTTP/REST concepts
├─ Day 3-4: Minimal APIs + First CRUD API
└─ Day 5: Review and exercises

Week 2: Core Concepts
├─ Day 1-2: Dependency Injection
├─ Day 3: Configuration
├─ Day 4: Middleware Pipeline
└─ Day 5: Review and exercises

Week 3: Advanced Topics
├─ Day 1-2: Model Binding and Validation
├─ Day 3-4: Aspire Integration
└─ Day 5: Final project and review
```

## Hands-On Exercises

This module includes progressive exercises that build on each other:

### Exercise 1: Hello World API (30 minutes)
Create your first minimal API that responds to HTTP requests.

### Exercise 2: Chuck Norris Joke API - Basic CRUD (2 hours)
Build a RESTful API for managing Chuck Norris jokes with:
- GET all jokes
- GET joke by ID
- POST new joke
- PUT update joke
- DELETE joke

### Exercise 3: Chuck Norris API - Dependency Injection (1 hour)
Refactor your API to use proper dependency injection and service layers.

### Exercise 4: Chuck Norris API - Configuration (1 hour)
Add configurable features like pagination limits and API keys.

### Exercise 5: Chuck Norris API - Middleware (1.5 hours)
Add request logging, error handling, and response time tracking.

### Exercise 6: Chuck Norris API - Validation (1.5 hours)
Implement robust validation and proper error responses.

### Exercise 7: Chuck Norris API - Aspire (2 hours)
Convert your API to use .NET Aspire with full observability.

## Project Structure

Throughout this module, you'll work with a consistent project structure:

```
ChuckNorrisApi/
├── ChuckNorrisApi.AppHost/              # Aspire orchestration (Lesson 7)
│   ├── Program.cs
│   └── ChuckNorrisApi.AppHost.csproj
├── ChuckNorrisApi.ServiceDefaults/      # Shared configuration (Lesson 7)
│   ├── Extensions.cs
│   └── ChuckNorrisApi.ServiceDefaults.csproj
├── ChuckNorrisApi/                      # Main API project
│   ├── Program.cs                       # Application entry point
│   ├── Models/                          # Data models
│   │   └── Joke.cs
│   ├── Services/                        # Business logic (Lesson 3)
│   │   ├── IJokeService.cs
│   │   └── JokeService.cs
│   ├── Middleware/                      # Custom middleware (Lesson 5)
│   │   └── RequestTimingMiddleware.cs
│   ├── Validators/                      # Validation logic (Lesson 6)
│   │   └── JokeValidator.cs
│   ├── appsettings.json                 # Configuration (Lesson 4)
│   ├── appsettings.Development.json
│   └── ChuckNorrisApi.csproj
└── ChuckNorrisApi.sln
```

## Technologies Covered

This module focuses on:

- **ASP.NET Core 10.0** - The latest version of ASP.NET Core
- **Minimal APIs** - Modern, streamlined API development
- **.NET Aspire** - Cloud-ready orchestration and observability
- **C# 13** - Latest language features
- **HTTP/REST** - Web protocol fundamentals

## Tools You'll Use

- **Visual Studio 2022** (17.12+) or **Visual Studio Code** with C# Dev Kit
- **JetBrains Rider** (alternative IDE)
- **.NET CLI** - Command-line interface for .NET
- **Postman** or **Thunder Client** - API testing
- **Aspire Dashboard** - Observability and monitoring
- **Git** - Version control

## Best Practices Emphasized

Throughout this module, we emphasize:

- ✅ **Clean Code**: Readable, maintainable code with clear naming
- ✅ **SOLID Principles**: Especially dependency injection and single responsibility
- ✅ **Security**: Never hardcode secrets, validate inputs
- ✅ **Testing**: Write testable code (even though testing is Module 09)
- ✅ **Observability**: Logging, metrics, and tracing from the start
- ✅ **REST Standards**: Follow HTTP semantics and status codes
- ✅ **Documentation**: Self-documenting code and API documentation

## Common Mistakes to Avoid

We'll explicitly call out these common beginner mistakes:

- ❌ Not understanding HTTP status codes
- ❌ Misusing service lifetimes (especially Singleton)
- ❌ Hardcoding configuration values
- ❌ Ignoring the middleware order
- ❌ Poor error handling and validation
- ❌ Not using dependency injection properly
- ❌ Returning 200 OK for every response
- ❌ Not securing sensitive configuration

## Assessment

Your understanding will be evaluated through:

1. **Hands-on exercises** (70%): Building the Chuck Norris API progressively
2. **Code review** (20%): Quality, best practices, and patterns
3. **Conceptual questions** (10%): Understanding of key concepts

## Resources

### Official Documentation
- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core/)
- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Minimal APIs Overview](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)

### Recommended Reading
- [HTTP: The Definitive Guide](https://www.oreilly.com/library/view/http-the-definitive/1565925092/) (O'Reilly)
- [REST API Design Rulebook](https://www.oreilly.com/library/view/rest-api-design/9781449317904/) (O'Reilly)

### Video Resources
- [ASP.NET Core Series on .NET YouTube](https://www.youtube.com/@dotnet)
- [.NET Aspire Overview](https://www.youtube.com/watch?v=DORZA_S7f9w)

### Community
- [ASP.NET Core GitHub](https://github.com/dotnet/aspnetcore)
- [Stack Overflow - ASP.NET Core Tag](https://stackoverflow.com/questions/tagged/asp.net-core)
- [.NET Discord Community](https://aka.ms/dotnet-discord)

## Getting Help

During this module, if you get stuck:

1. **Check the documentation** - Each lesson has detailed explanations
2. **Review the code examples** - Every concept has working code
3. **Ask in the course forum** - Your classmates and instructors are here to help
4. **Office hours** - Schedule time with instructors
5. **Debug systematically** - Use the debugger, not just Console.WriteLine

## Next Steps

After completing this module, you'll be ready for:

- **Module 03: Logging and Monitoring** - Deep dive into observability
- **Module 04: Entity Framework Core** - Database integration
- **Module 05: OpenAPI and Documentation** - API documentation and Swagger
- **Module 06: Security** - Authentication and authorization

## Let's Get Started!

Ready to build modern web APIs with ASP.NET Core? Let's begin with [Lesson 01: Introduction to ASP.NET Core](01_introduction.md)!

---

## Quick Reference

### Key Commands

```bash
# Create new minimal API
dotnet new web -n MyApi

# Add Aspire orchestration
dotnet new aspire -n MyApi

# Run the application
dotnet run

# Watch mode (auto-restart on changes)
dotnet watch

# Build the project
dotnet build

# Run tests
dotnet test
```

### Key Concepts Checklist

By the end of this module, make sure you understand:

- [ ] HTTP methods and status codes
- [ ] REST principles and constraints
- [ ] Minimal API syntax and routing
- [ ] Dependency injection container
- [ ] Service lifetimes (Singleton, Scoped, Transient)
- [ ] Configuration sources and precedence
- [ ] Options pattern for typed configuration
- [ ] Middleware pipeline and execution order
- [ ] Model binding from different sources
- [ ] Data validation with attributes
- [ ] .NET Aspire project structure
- [ ] ServiceDefaults pattern

### Estimated Time

- **Total module time**: 18-20 hours
- **Lectures**: 8 hours
- **Hands-on exercises**: 10-12 hours
- **Self-study**: Variable based on your pace

Good luck, and enjoy learning ASP.NET Core! 🚀
