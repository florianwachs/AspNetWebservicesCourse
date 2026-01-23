# Module 14: Native AOT & Cloud Deployment

## 🎯 Learning Objectives

After completing this module, you will be able to:
- Understand Native AOT (Ahead-of-Time compilation) benefits and limitations
- Build Native AOT-compatible APIs
- Deploy applications to Azure using various methods
- Use Azure Container Apps for cloud-native deployment
- Implement CI/CD pipelines with GitHub Actions
- Configure application settings for different environments
- Implement blue-green and canary deployment strategies
- Monitor and troubleshoot production deployments

## �� Topics Covered

### 1. Native AOT Fundamentals
- What is Native AOT and why use it?
- Benefits: faster startup, lower memory, smaller size
- Limitations and compatibility considerations
- AOT vs JIT compilation
- Trimming and its effects

### 2. Building Native AOT APIs
- AOT-compatible API design
- Minimal APIs vs Controllers for AOT
- Source generators and AOT
- Testing AOT applications
- Troubleshooting AOT issues

### 3. Container Deployment
- Docker best practices
- Multi-stage builds
- Optimizing container images
- Container registries (ACR, Docker Hub)
- Security scanning

### 4. Azure Deployment Options
- Azure App Service
- Azure Container Apps
- Azure Kubernetes Service (AKS)
- Azure Container Instances
- Choosing the right service

### 5. .NET Aspire Azure Deployment
- Deploying Aspire apps to Azure
- Azure Developer CLI (azd)
- Infrastructure as Code with Bicep
- Aspire manifest and deployment
- Managing Aspire resources in Azure

### 6. CI/CD with GitHub Actions
- Building .NET applications in GitHub Actions
- Running tests in CI
- Container image building and pushing
- Automated deployments
- Environment-specific configurations

### 7. Production Best Practices
- Configuration management
- Secrets management with Azure Key Vault
- Logging and monitoring
- Health checks and readiness probes
- Scaling strategies
- Cost optimization

## 🛠️ Prerequisites

- Completed Module 02 (ASP.NET Core Fundamentals)
- Completed Module 10 (.NET Aspire Deep Dive)
- Understanding of containers and Docker
- Azure account (free tier available)
- GitHub account

## 📖 Key Concepts

### Native AOT Benefits

| Metric | JIT | Native AOT | Improvement |
|--------|-----|------------|-------------|
| Startup Time | ~400ms | ~10ms | 40x faster |
| Memory Usage | ~50MB | ~15MB | 3x lower |
| Disk Size | ~100MB | ~15MB | 6x smaller |

### Deployment Architecture

```
GitHub Repo → GitHub Actions → Azure Container Registry → Azure Container Apps
                     ↓
                  Run Tests
                     ↓
               Build Container
                     ↓
            Deploy to Staging → Manual Approval → Deploy to Production
```

## 💻 Hands-on Exercises

### Exercise 1: Create a Native AOT API
Build an API compatible with Native AOT:
- Create minimal API project
- Add AOT publishing configuration
- Test trimming warnings
- Publish as Native AOT binary
- Measure startup time and memory

### Exercise 2: Dockerize the Application
- Create optimized Dockerfile
- Build multi-stage container
- Test locally with Docker Compose
- Push to Azure Container Registry

### Exercise 3: Deploy to Azure Container Apps
- Set up Azure Container Apps environment
- Deploy container to Azure
- Configure environment variables
- Set up custom domains and SSL
- Implement health checks

### Exercise 4: Aspire Cloud Deployment
Deploy an Aspire application:
- Use Azure Developer CLI (azd)
- Initialize Aspire project for Azure
- Deploy to Azure with azd up
- Explore generated infrastructure
- Monitor in Azure Portal

### Exercise 5: GitHub Actions CI/CD
Create deployment pipeline:
- Build and test workflow
- Build and push Docker image
- Deploy to staging environment
- Add manual approval for production
- Implement deployment notifications

## 📝 Sample Code Structure

```
deployment-demo/
├── src/
│   ├── Api/
│   │   ├── Api.csproj
│   │   ├── Program.cs
│   │   └── Dockerfile
│   └── AppHost/
│       ├── AppHost.csproj
│       └── Program.cs
├── .github/
│   └── workflows/
│       ├── build.yml
│       └── deploy.yml
├── infra/
│   ├── main.bicep
│   └── parameters.json
└── azure.yaml (azd configuration)
```

## 🔧 Native AOT Configuration

### Project File Setup

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <PublishAot>true</PublishAot>
    <InvariantGlobalization>true</InvariantGlobalization>
    <JsonSerializerIsReflectionEnabledByDefault>false</JsonSerializerIsReflectionEnabledByDefault>
  </PropertyGroup>
</Project>
```

### AOT-Compatible Minimal API

```csharp
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

// Configure JSON serialization for AOT
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(
        0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/products", () => 
{
    return new[]
    {
        new Product { Id = 1, Name = "Product 1", Price = 9.99m },
        new Product { Id = 2, Name = "Product 2", Price = 19.99m }
    };
});

app.Run();

public record Product
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public decimal Price { get; init; }
}

[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Product[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
```

### Optimized Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copy csproj and restore
COPY *.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# Runtime stage for regular build
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Api.dll"]

# Native AOT stage (optional)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-aot
WORKDIR /source
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app -r linux-x64 --no-restore

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS runtime-aot
WORKDIR /app
COPY --from=build-aot /app .
EXPOSE 8080
ENTRYPOINT ["./Api"]
```

## 🚀 Azure Developer CLI (azd)

### Initialize Aspire Project

```bash
# Install azd
curl -fsSL https://aka.ms/install-azd.sh | bash

# Initialize project
azd init

# Login to Azure
azd auth login

# Provision and deploy
azd up
```

### azure.yaml Configuration

```yaml
name: myaspireapp
metadata:
  template: aspire-starter@latest
services:
  api:
    project: ./src/Api
    language: csharp
    host: containerapp
  apphost:
    project: ./src/AppHost
    language: csharp
    host: containerapp
```

## 📊 GitHub Actions Workflow

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]
  workflow_dispatch:

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Test
        run: dotnet test --no-build --verbosity normal

  build-and-push-image:
    needs: build-and-test
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write
    steps:
      - uses: actions/checkout@v4
      
      - name: Log in to Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}
      
      - name: Build and push Docker image
        uses: docker/build-push-action@v5
        with:
          context: ./src/Api
          push: true
          tags: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}

  deploy-to-azure:
    needs: build-and-push-image
    runs-on: ubuntu-latest
    environment: production
    steps:
      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      
      - name: Deploy to Azure Container Apps
        uses: azure/container-apps-deploy-action@v1
        with:
          containerAppName: my-api
          resourceGroup: my-resource-group
          imageToDeploy: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
```

## 📚 Additional Resources

- [Native AOT Documentation](https://learn.microsoft.com/dotnet/core/deploying/native-aot/)
- [Azure Container Apps](https://learn.microsoft.com/azure/container-apps/)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/)
- [GitHub Actions for .NET](https://docs.github.com/actions/automating-builds-and-tests/building-and-testing-net)
- [Aspire Deployment](https://learn.microsoft.com/dotnet/aspire/deployment/overview)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)

## 🎓 Best Practices

1. **Use Native AOT for Microservices**: Great for containers and serverless
2. **Implement Health Checks**: Essential for container orchestration
3. **Configuration Management**: Use Azure Key Vault for secrets
4. **Multi-stage Builds**: Keep images small and secure
5. **Infrastructure as Code**: Use Bicep or Terraform
6. **Automated Testing**: Test before deploying
7. **Blue-Green Deployments**: Zero-downtime deployments
8. **Monitoring**: Use Application Insights
9. **Cost Management**: Right-size your resources
10. **Security**: Regular vulnerability scanning

## ⚠️ Native AOT Limitations

- No runtime code generation (Reflection.Emit)
- Limited reflection support
- Some NuGet packages incompatible
- Longer build times
- Larger binaries for complex apps
- Limited debugging capabilities

## ✅ Module Checklist

- [ ] Understand Native AOT benefits and limitations
- [ ] Build AOT-compatible APIs
- [ ] Create optimized Docker containers
- [ ] Deploy to Azure Container Apps
- [ ] Use Azure Developer CLI
- [ ] Set up CI/CD with GitHub Actions
- [ ] Implement health checks and monitoring
- [ ] Configure environment-specific settings
- [ ] Complete all exercises

## 🎓 Course Completion

Congratulations! You've completed all modules of the Web Services with .NET course. You now have:
- ✅ Modern .NET 10 and C# 13 skills
- ✅ Cloud-native development with Aspire
- ✅ Microservices architecture knowledge
- ✅ AI/ML integration capabilities
- ✅ Performance optimization expertise
- ✅ Production deployment skills

## 🚀 Next Steps

- Build your course project
- Explore advanced topics
- Contribute to open-source .NET projects
- Stay updated with .NET community
- Share your knowledge with others

Good luck with your projects and exam! 🎉
