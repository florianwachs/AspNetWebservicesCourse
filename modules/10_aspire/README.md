# Module 10: .NET Aspire Deep Dive

## Overview

Welcome to Module 10 of the Web Services with .NET course at TH Rosenheim. This module provides a comprehensive exploration of .NET Aspire, Microsoft's opinionated, cloud-ready stack for building observable, production-ready distributed applications with .NET.

.NET Aspire is designed to improve the experience of building cloud-native applications by providing a consistent, opinionated set of tools and patterns for building distributed applications. It addresses common challenges in microservices development, including service discovery, observability, resilience, and configuration management.

## Learning Objectives

By the end of this module, you will be able to:

1. **Understand .NET Aspire Architecture**: Comprehend the core architecture and design principles of .NET Aspire
2. **Create Distributed Applications**: Build multi-service applications using Aspire's orchestration capabilities
3. **Implement Service Discovery**: Enable seamless service-to-service communication in distributed systems
4. **Configure Observability**: Integrate OpenTelemetry for comprehensive monitoring, logging, and tracing
5. **Use Aspire Components**: Leverage built-in integrations for databases, message brokers, and caching
6. **Implement Resilience Patterns**: Apply circuit breakers, retries, and timeouts using Aspire defaults
7. **Monitor with Aspire Dashboard**: Utilize the development dashboard for debugging and monitoring
8. **Deploy to Production**: Deploy Aspire applications to Azure using Azure Developer CLI (azd)

## Prerequisites

Before starting this module, you should have:

- ✅ Completed modules on ASP.NET Core fundamentals (Module 2)
- ✅ Understanding of dependency injection and configuration in .NET
- ✅ Basic knowledge of Docker and containerization concepts
- ✅ Familiarity with distributed systems concepts
- ✅ .NET 10 SDK installed on your development machine
- ✅ Docker Desktop or Podman installed and running
- ✅ Visual Studio 2025 or Visual Studio Code with C# extension
- ✅ Azure subscription (optional, for deployment exercises)

## Module Structure

This module is organized into the following lessons:

### Core Concepts

1. **[Aspire Fundamentals](./01_aspire_fundamentals.md)**
   - What is .NET Aspire?
   - Cloud-native application challenges
   - Aspire architecture and design principles
   - Key components and concepts
   - When to use Aspire

2. **[AppHost Project](./02_app_host.md)**
   - Understanding the orchestration layer
   - Service registration and configuration
   - Resource dependencies and health checks
   - Environment variables and secrets
   - Development vs. production configuration

3. **[Service Defaults](./03_service_defaults.md)**
   - ServiceDefaults project explained
   - OpenTelemetry integration
   - Logging, metrics, and tracing
   - HTTP resilience patterns
   - Health checks configuration

### Components and Integration

4. **[Aspire Components](./04_components.md)**
   - Overview of Aspire component ecosystem
   - Database components (PostgreSQL, SQL Server, MongoDB)
   - Caching components (Redis, Valkey)
   - Messaging components (RabbitMQ, Azure Service Bus, Kafka)
   - Storage components (Azure Blob Storage)
   - Creating custom components

5. **[Service Discovery](./05_service_discovery.md)**
   - Service-to-service communication patterns
   - Named HTTP clients
   - Service endpoints and configuration
   - Load balancing and failover
   - Testing service discovery

### Operations and Deployment

6. **[Aspire Dashboard](./06_dashboard.md)**
   - Dashboard features and capabilities
   - Viewing logs and traces
   - Monitoring metrics and health
   - Debugging distributed applications
   - Dashboard configuration and customization

7. **[Deployment](./07_deployment.md)**
   - Deployment overview and options
   - Azure Developer CLI (azd) setup
   - Deploying to Azure Container Apps
   - Azure resource provisioning
   - CI/CD integration
   - Production considerations

### Hands-on Practice

8. **[Exercises](./exercises/)**
   - Exercise 1: Creating your first Aspire application
   - Exercise 2: Adding database and caching components
   - Exercise 3: Implementing service-to-service communication
   - Exercise 4: Observability and monitoring
   - Exercise 5: Deploying to Azure

## What is .NET Aspire?

.NET Aspire is a cloud-ready stack for building observable, production-ready distributed applications. It consists of:

### Components

```
┌─────────────────────────────────────────────────────────────┐
│                    .NET Aspire Stack                        │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │           Aspire Dashboard (Dev Tool)               │   │
│  │  Logs • Traces • Metrics • Health • Environment     │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │         App Host (Orchestration Layer)              │   │
│  │  Service Discovery • Configuration • Dependencies    │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │       Service Defaults (Cross-cutting Concerns)     │   │
│  │  Telemetry • Resilience • Health Checks • Logging   │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │           Aspire Components (Integrations)          │   │
│  │  Redis • PostgreSQL • RabbitMQ • SQL • Kafka • ...  │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              Your Application Services              │   │
│  │     Web API • Worker Service • Web Frontend         │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Key Features

1. **Orchestration**: The AppHost project orchestrates multiple services and resources
2. **Components**: Pre-built integrations for common dependencies (databases, caches, message brokers)
3. **Service Defaults**: Standardized configuration for telemetry, resilience, and health
4. **Dashboard**: Real-time visualization of your application's behavior during development
5. **Deployment**: Seamless deployment to production environments, especially Azure

## Why Use .NET Aspire?

### Traditional Challenges in Distributed Applications

When building distributed applications, developers face several challenges:

- **Service Discovery**: How do services find and communicate with each other?
- **Observability**: How do you debug across multiple services?
- **Configuration**: How do you manage configuration for multiple services?
- **Resilience**: How do you handle transient failures?
- **Local Development**: How do you run multiple services locally?
- **Deployment**: How do you deploy multi-service applications consistently?

### How Aspire Helps

.NET Aspire addresses these challenges by providing:

1. **Simplified Local Development**: Run entire distributed systems locally with a single command
2. **Built-in Observability**: OpenTelemetry integration out of the box
3. **Service Discovery**: Automatic service-to-service communication configuration
4. **Resilience Patterns**: Pre-configured retry policies, circuit breakers, and timeouts
5. **Component Library**: Ready-to-use integrations for common services
6. **Streamlined Deployment**: Deploy to production with minimal configuration

## Technology Stack

This module covers .NET Aspire with the following technology stack:

- **.NET 10**: Latest .NET runtime and SDK
- **Aspire 10.x**: Latest Aspire version with newest features
- **OpenTelemetry**: Industry-standard observability
- **Docker/Podman**: Container runtime for local development
- **Azure Container Apps**: Recommended production deployment target
- **Azure Developer CLI (azd)**: Deployment tooling

## Course Project: E-Commerce Microservices

Throughout this module, we'll build a realistic e-commerce system consisting of:

- **Catalog API**: Product catalog management
- **Shopping Cart API**: Shopping cart operations
- **Order API**: Order processing
- **Web Frontend**: Blazor web application
- **PostgreSQL**: Product database
- **Redis**: Caching and session storage
- **RabbitMQ**: Asynchronous messaging

This project demonstrates:
- Multi-service orchestration
- Database and cache integration
- Message-based communication
- Service-to-service calls
- Comprehensive observability
- Deployment to Azure

## Getting Started

### Installation

1. **Install .NET 10 SDK**:
   ```bash
   # Download from https://dotnet.microsoft.com/download/dotnet/10.0
   dotnet --version  # Should show 10.x.x
   ```

2. **Verify Aspire Workload**:
   ```bash
   # Check if Aspire templates are available
   dotnet new list aspire
   ```

   If not available, install the Aspire workload:
   ```bash
   dotnet workload install aspire
   ```

3. **Install Docker Desktop or Podman**:
   - Windows/Mac: [Docker Desktop](https://www.docker.com/products/docker-desktop)
   - Linux: Docker Engine or Podman

4. **Verify Installation**:
   ```bash
   docker --version
   docker ps  # Should show running containers (if any)
   ```

### Your First Aspire Application

Create a new Aspire application:

```bash
# Create a new Aspire starter application
dotnet new aspire-starter -o MyFirstAspireApp
cd MyFirstAspireApp

# Run the application
dotnet run --project MyFirstAspireApp.AppHost
```

The Aspire Dashboard will automatically open in your browser at `http://localhost:15888` (port may vary).

### Project Structure

An Aspire application typically consists of:

```
MyFirstAspireApp/
├── MyFirstAspireApp.sln
├── MyFirstAspireApp.AppHost/          # Orchestration project
│   ├── Program.cs                     # Service registration
│   └── appsettings.json              # Configuration
├── MyFirstAspireApp.ServiceDefaults/  # Shared defaults
│   └── Extensions.cs                  # Telemetry, resilience
└── MyFirstAspireApp.ApiService/       # Example API service
    ├── Program.cs
    └── appsettings.json
```

## Learning Path

We recommend following the lessons in order:

1. **Week 1**: Fundamentals, AppHost, and Service Defaults (Lessons 1-3)
2. **Week 2**: Components, Service Discovery, and Dashboard (Lessons 4-6)
3. **Week 3**: Deployment and Exercises (Lesson 7 and Exercises)

## Additional Resources

### Official Documentation
- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Aspire Samples](https://github.com/dotnet/aspire-samples)
- [Aspire GitHub Repository](https://github.com/dotnet/aspire)

### Video Resources
- [.NET Aspire Overview](https://learn.microsoft.com/shows/dotnet-aspire/)
- [Build Cloud-Native Apps with .NET Aspire](https://www.youtube.com/watch?v=DORZA_S7f9w)

### Community
- [.NET Aspire Discord](https://aka.ms/aspire/discord)
- [.NET Blog](https://devblogs.microsoft.com/dotnet/)

### Related Microsoft Learn Paths
- [Build distributed apps with .NET Aspire](https://learn.microsoft.com/training/paths/dotnet-aspire/)
- [Deploy a .NET Aspire app to Azure Container Apps](https://learn.microsoft.com/training/modules/deploy-aspire-app-azure-container-apps/)

## Assessment

This module includes:

- **Formative Assessment**: Exercises after each lesson
- **Summative Assessment**: Final project deploying a complete distributed application
- **Grading Criteria**:
  - Code quality and organization (30%)
  - Proper use of Aspire components (30%)
  - Observability implementation (20%)
  - Deployment and documentation (20%)

## Getting Help

If you encounter issues:

1. Check the [Troubleshooting Guide](./exercises/troubleshooting.md)
2. Review the [FAQ](./exercises/faq.md)
3. Post questions in the course forum
4. Attend office hours
5. Consult the [official documentation](https://learn.microsoft.com/dotnet/aspire/)

## Next Steps

Ready to begin? Start with **[Lesson 1: Aspire Fundamentals](./01_aspire_fundamentals.md)** to understand the core concepts and architecture of .NET Aspire.

---

**Course**: Web Services with .NET  
**Institution**: TH Rosenheim  
**Module**: 10 - .NET Aspire Deep Dive  
**Target Framework**: .NET 10  
**Last Updated**: 2025
