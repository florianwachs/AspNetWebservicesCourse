# Building Modern Web Services with ASP.NET Core and .NET 10

Welcome to **Building Modern Web Services with ASP.NET Core and .NET 10** at [TH Rosenheim](https://www.th-rosenheim.de/).

## 🚀 Course Overview

This hands-on course teaches modern web service development using the latest .NET technology stack. Over 5 intensive workshop days, we will cover the ground of many relevant topics while developing webservices. The knowledge that you will gain is not limited to the .NET world.

### What You'll Learn

- **Minimal APIs** with route groups, TypedResults, and built-in OpenAPI
- **Entity Framework Core 10** with PostgreSQL
- **.NET Aspire** for cloud-native orchestration and observability
- **Authentication & Authorization** with Keycloak and JWT
- **HybridCache**, **SignalR**, **Polly v8 Resilience**, and **Background Services**
- **Integration Testing** with Testcontainers
- **Vertical Slice Architecture** with MediatR/CQRS
- **Actor Frameworks** like Akka.NET

## 📅 Schedule

|                   | Date             | Topic                                                          |
| ----------------- | ---------------- | -------------------------------------------------------------- |
| ✅ Workshop Day 1 | 2026-03-20, 8:30 | [Foundations: Minimal APIs & Dependency Injection](docs/day1/) |
| ✅ Workshop Day 2 | 2026-03-21, 8:30 | [Data Access, Validation & Error Handling](docs/day2/)         |
| ✅ Workshop Day 3 | 2026-04-11, 8:30 | [.NET Aspire, Authentication & Architecture](docs/day3/)       |
| 👉 Workshop Day 4 | 2026-04-25, 8:30 | [Architecture, SignalR (realtime), Testing](docs/day4/)        |
| ⏳ Workshop Day 5 | 2026-05-09, 8:30 | TBD                                                            |
| 👀 Exam           | 2026-06-20, 8:30 | [Exam Information](course/exam/readme.md)                      |

## 🔬 Labs

Each workshop day includes a hands-on lab with starter code and a reference solution:

| Lab                                                                            | Day   | Topic                                         |
| ------------------------------------------------------------------------------ | ----- | --------------------------------------------- |
| [Lab: Build the Events API](labs/lab1-events-api/)                             | Day 1 | Minimal APIs, OpenAPI, Scalar                 |
| [Lab: Persist & Validate TechConf](labs/lab2-persistence-validation/)          | Day 2 | EF Core 10, FluentValidation, Problem Details |
| [Lab: ASP.NET Identity with Aspire and React](labs/lab-identity-react/)        | Day 3 | Aspire, ASP.NET Identity, Cookies, React      |
| [Lab: Keycloak Authentication with Aspire and React](labs/lab-keycloak-react/) | Day 3 | Aspire, Keycloak, JWT, React                  |

## 🔧 Environment Setup

### Prerequisites

- **.NET 10 SDK** ([download](https://dotnet.microsoft.com/download/dotnet/10.0))
- **Visual Studio 2025** or **Visual Studio Code** with [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- **Docker Desktop** ([download](https://www.docker.com/products/docker-desktop/)) — required for .NET Aspire, PostgreSQL, Redis, Keycloak
- **Git** for version control

### Quick Start

```bash
# Clone the repository
git clone https://github.com/florianwachs/AspNetWebservicesCourse.git
cd AspNetWebservicesCourse

# Verify .NET 10 SDK
dotnet --version

# Verify Docker
docker --version
```

> **💡 Tip**: This repository includes a [Dev Container](.devcontainer/devcontainer.json) configuration. Open in VS Code or GitHub Codespaces for a pre-configured environment.

If you use the dev container, it will automatically install `dotnet-ef`. You can verify with:

```bash
dotnet ef --version
```

When a lab or demo needs PostgreSQL, start it manually from inside the dev container:

```bash
docker run --rm -d \
  --name techconf-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=techconf \
  -e POSTGRES_DB=techconfdb \
  -p 5432:5432 \
  postgres:latest
```

Stop it again with `docker stop techconf-db`.

## 📖 Additional Material

Working through preparation material is **not required** — we'll learn the necessary C# knowledge together. But if you'd like to learn at your own pace:

| Resource                             | Link                                                                                                                                                                                                    |
| ------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| C# Course (Microsoft + FreeCodeCamp) | https://www.freecodecamp.org/learn/foundational-c-sharp-with-microsoft/                                                                                                                                 |
| Official C# Documentation            | https://docs.microsoft.com/en-us/dotnet/csharp/                                                                                                                                                         |
| C# Cheat Sheet                       | [cheatsheets/csharp-cheat-sheet.md](cheatsheets/csharp-cheat-sheet.md)                                                                                                                                  |
| Video Courses                        | [C# for Beginners](https://learn.microsoft.com/en-us/shows/csharp-for-beginners/), [Nick Chapsas](https://www.youtube.com/@nickchapsas), [Milan Jovanović](https://www.youtube.com/@MilanJovanovicTech) |
| Microsoft Learn C# Path              | https://docs.microsoft.com/en-us/learn/paths/csharp-first-steps/                                                                                                                                        |

## 📚 Reference Material

https://github.com/dodyg/practical-aspnetcore

### Cheatsheets

- [C# 13 Cheat Sheet](cheatsheets/csharp-cheat-sheet.md)
- [Day 1 API Essentials Cheat Sheet](cheatsheets/day1/README.md)
- [Day 2 Persistence, Validation & Error Handling Cheat Sheet](cheatsheets/day2/README.md)

### Patterns & Guidelines

- [Repository Pattern](extras/patterns/repository-pattern.md)
- [Dependency Injection](extras/patterns/dependency-injection.md)
- [Vertical Slice Architecture](extras/patterns/vertical-slice-architecture.md)
- [CQRS Pattern](extras/patterns/cqrs.md)
- [REST API Guidelines](extras/guidelines/rest-guidelines.md)

## 📝 Exam

The exam consists of a project with multiple components. Find details 👉 [here](course/exam/readme.md).

> [!IMPORTANT]
> Be present at the workshop days where progress has to be shown.

## 🎯 About This Course

Welcome! I'm excited to guide you through modern web service development with .NET. Whether you're coming from other technologies like Java or Node.js, or this is completely new to you, my goal is that everyone can learn and grow from this course.

I highly encourage your active participation — ask questions, provide feedback, and share your ideas. There are no "stupid questions". If you get stuck on a problem, need help with your project, or need any support, please let me know.

## 🤝 Contributing

Feel free to point out anything I can do to improve your learning journey!

## 📄 License

This course material is provided for educational purposes at TH Rosenheim.
