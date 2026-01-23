# Module 11: Microservices & YARP Reverse Proxy

## 🎯 Learning Objectives

After completing this module, you will be able to:
- Understand microservices architecture principles
- Implement API Gateway patterns using YARP (Yet Another Reverse Proxy)
- Configure service-to-service communication
- Implement load balancing and health checks
- Use .NET Aspire for microservices orchestration
- Apply resilience patterns (circuit breakers, retries)

## 📚 Topics Covered

### 1. Microservices Architecture Overview
- What are microservices?
- When to use microservices vs monolithic architecture
- Microservices design principles
- Domain-Driven Design (DDD) basics
- Service boundaries and data ownership

### 2. YARP - Yet Another Reverse Proxy
- Introduction to API Gateways
- Why YARP over alternatives?
- YARP configuration and routing
- Request/Response transformation
- Load balancing strategies
- Authentication and authorization at the gateway

### 3. Service Communication
- Synchronous vs asynchronous communication
- HTTP/REST communication between services
- gRPC for inter-service communication
- Message queuing patterns

### 4. Resilience and Fault Tolerance
- Circuit breaker pattern
- Retry policies with exponential backoff
- Timeout handling
- Bulkhead isolation
- Using Polly for resilience

### 5. Service Discovery and Configuration
- Dynamic service discovery
- Configuration management in distributed systems
- Environment-specific settings
- Secrets management

### 6. Observability in Microservices
- Distributed tracing with OpenTelemetry
- Centralized logging
- Metrics and monitoring
- Health checks and readiness probes
- Using Aspire Dashboard for monitoring

## 🛠️ Prerequisites

- Completed Module 02 (ASP.NET Core Fundamentals)
- Completed Module 07 (gRPC Services)
- Completed Module 10 (.NET Aspire Deep Dive)
- Understanding of REST APIs
- Docker and containerization basics

## 📖 Key Concepts

### Microservices Principles
1. **Single Responsibility**: Each service owns a specific business capability
2. **Autonomy**: Services can be developed, deployed, and scaled independently
3. **Decentralization**: Distributed data management and decision-making
4. **Resilience**: Design for failure and graceful degradation
5. **Observable**: Built-in monitoring and diagnostics

### YARP Features
- **High Performance**: Built on ASP.NET Core's performance stack
- **Flexible Configuration**: JSON, code, or dynamic configuration
- **Extensibility**: Custom middleware and transformations
- **Modern Protocols**: HTTP/1.1, HTTP/2, HTTP/3, and gRPC support

## 💻 Hands-on Exercises

### Exercise 1: Build a Simple Microservices System
Create a small e-commerce system with:
- Product Catalog Service (manages products)
- Order Service (handles orders)
- API Gateway (YARP) to route requests

### Exercise 2: Implement Service-to-Service Communication
Extend Exercise 1:
- Order Service calls Product Catalog to validate products
- Implement both HTTP/REST and gRPC communication
- Add error handling and timeouts

### Exercise 3: Add Resilience with Polly
- Implement circuit breaker for service calls
- Add retry logic with exponential backoff
- Test behavior when services are down

### Exercise 4: Configure YARP Load Balancing
- Set up multiple instances of a service
- Configure YARP for round-robin load balancing
- Implement health checks
- Test failover scenarios

### Exercise 5: Orchestrate with Aspire
- Use .NET Aspire to orchestrate all services
- Configure service discovery
- Implement distributed tracing
- Monitor services in Aspire Dashboard

## 📝 Sample Code Structure

```
microservices-demo/
├── src/
│   ├── Gateway/
│   │   ├── Gateway.csproj
│   │   ├── Program.cs
│   │   └── appsettings.json (YARP configuration)
│   ├── ProductCatalog/
│   │   ├── ProductCatalog.Api/
│   │   └── ProductCatalog.Domain/
│   ├── OrderService/
│   │   ├── OrderService.Api/
│   │   └── OrderService.Domain/
│   └── ServiceDefaults/
│       └── Extensions.cs (shared configurations)
├── AppHost/
│   ├── AppHost.csproj
│   └── Program.cs (Aspire orchestration)
└── docker-compose.yml
```

## 🔧 YARP Configuration Example

```json
{
  "ReverseProxy": {
    "Routes": {
      "products-route": {
        "ClusterId": "product-catalog",
        "Match": {
          "Path": "/api/products/{**catch-all}"
        }
      },
      "orders-route": {
        "ClusterId": "order-service",
        "Match": {
          "Path": "/api/orders/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "product-catalog": {
        "Destinations": {
          "destination1": {
            "Address": "http://localhost:5001"
          }
        }
      },
      "order-service": {
        "Destinations": {
          "destination1": {
            "Address": "http://localhost:5002"
          }
        },
        "HealthCheck": {
          "Active": {
            "Enabled": true,
            "Interval": "00:00:10",
            "Timeout": "00:00:05",
            "Policy": "ConsecutiveFailures",
            "Path": "/health"
          }
        }
      }
    }
  }
}
```

## 📚 Additional Resources

- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [Microservices Patterns by Chris Richardson](https://microservices.io/patterns/)
- [.NET Microservices Architecture eBook](https://docs.microsoft.com/dotnet/architecture/microservices/)
- [Polly Resilience Framework](https://github.com/App-vNext/Polly)
- [Martin Fowler on Microservices](https://martinfowler.com/articles/microservices.html)

## 🎓 Best Practices

1. **Start with a Monolith**: Don't begin with microservices unless you have a clear need
2. **Define Clear Boundaries**: Use domain-driven design to identify service boundaries
3. **API Contracts**: Use versioning and maintain backward compatibility
4. **Avoid Distributed Transactions**: Design for eventual consistency
5. **Centralize Cross-Cutting Concerns**: Use API Gateway for auth, logging, etc.
6. **Test Resilience**: Chaos engineering and failure injection testing
7. **Monitor Everything**: Distributed tracing is essential for debugging

## ✅ Module Checklist

- [ ] Understand microservices principles and trade-offs
- [ ] Set up YARP as an API Gateway
- [ ] Implement service-to-service communication
- [ ] Add resilience patterns with Polly
- [ ] Configure load balancing and health checks
- [ ] Orchestrate services with .NET Aspire
- [ ] Implement distributed tracing
- [ ] Complete all exercises

## 🚀 Next Module

Continue to [Module 12: AI/ML Integration](../12_ai_integration/) to learn about integrating AI services into your applications.
