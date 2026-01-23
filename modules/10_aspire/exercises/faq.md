# Frequently Asked Questions (FAQ)

Common questions about .NET Aspire and the exercises.

## 🎯 General Questions

### What is .NET Aspire?

.NET Aspire is an opinionated, cloud-ready stack for building observable, production-ready, distributed applications. It provides:
- Project templates and tooling
- Component integrations (databases, caching, messaging)
- Built-in observability (logs, metrics, traces)
- Local development orchestration
- Simplified Azure deployment

### Do I need to learn Kubernetes to use Aspire?

No! Aspire abstracts away the complexity of container orchestration. During development, it uses a simple orchestrator. For production, it can deploy to Azure Container Apps (serverless) without requiring Kubernetes knowledge.

### Can I use Aspire with existing projects?

Yes! You can add Aspire to existing projects:
1. Add the AppHost project to orchestrate services
2. Add ServiceDefaults project for shared configuration
3. Add references from your existing projects to ServiceDefaults
4. Call `builder.AddServiceDefaults()` in your startup code

### Is Aspire only for microservices?

No! Aspire works well for:
- Microservices architectures
- Modular monoliths
- Simple web apps with dependencies (database, cache)
- Any distributed application

### What's the difference between Aspire and Docker Compose?

| Feature | Aspire | Docker Compose |
|---------|---------|----------------|
| Language | C# (strongly typed) | YAML |
| Service Discovery | Built-in | Requires configuration |
| Observability | Built-in dashboard | Requires setup |
| Cloud Deployment | Integrated (azd) | Manual |
| .NET Integration | Native | Generic |
| Local Dev | Optimized for .NET | General purpose |

Both can coexist - use what works best for your needs.

## 🛠️ Technical Questions

### Why use `https+http://servicename` instead of just the URL?

This special scheme enables service discovery:
- `https+http://` tells HttpClient to use service discovery
- `servicename` is resolved to the actual service endpoint
- Works locally and in production without config changes
- Handles load balancing and health checks automatically

### How does the dashboard get telemetry data?

1. ServiceDefaults configures OpenTelemetry in each service
2. Telemetry is sent to OTLP (OpenTelemetry Protocol) endpoint
3. AppHost runs an OTLP collector
4. Dashboard consumes data from the collector
5. Everything is automatic - no configuration needed!

### Can I use Aspire without Docker?

For local development, Docker is required for infrastructure components (PostgreSQL, Redis, RabbitMQ, etc.). Your .NET services can run without Docker though.

For production deployment to Azure Container Apps, Docker is used but managed for you.

### How do I connect to databases/services outside Aspire?

```csharp
// In AppHost - add connection string resource
var existingDb = builder.AddConnectionString("existingdb");

// Reference it in your service
var api = builder.AddProject<Projects.ApiService>("api")
    .WithReference(existingDb);
```

Then in your service:
```csharp
// Connection string automatically available
builder.AddNpgsqlDbContext<MyDbContext>("existingdb");
```

### Can I use Aspire with non-.NET services?

Yes! You can add containers and executables:

```csharp
// Add existing container
builder.AddContainer("redis", "redis")
    .WithEndpoint(port: 6379, targetPort: 6379);

// Add executable (Node.js, Python, etc.)
builder.AddExecutable("frontend", "npm", ".")
    .WithArgs("run", "dev");
```

### How do I handle secrets in production?

Aspire supports Azure Key Vault automatically when deployed:

```csharp
// In production, this reads from Key Vault
var secret = builder.Configuration["MySecret"];

// Configure in AppHost for deployment
builder.AddAzureKeyVault("secrets");
```

See Exercise 5 for detailed examples.

## 📊 Observability Questions

### How do I add custom metrics?

```csharp
public class MyService
{
    private readonly Counter<long> _counter;
    
    public MyService(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("MyApp.MyService");
        _counter = meter.CreateCounter<long>("my_operation_count");
    }
    
    public void DoSomething()
    {
        _counter.Add(1);
    }
}
```

No need to configure OpenTelemetry - it's automatic!

### How do I create custom traces?

```csharp
public class MyService
{
    private static readonly ActivitySource ActivitySource = new("MyApp.MyService");
    
    public async Task ProcessAsync()
    {
        using var activity = ActivitySource.StartActivity("Process");
        activity?.SetTag("custom.tag", "value");
        
        // Your code here
        
        activity?.AddEvent(new ActivityEvent("Important step completed"));
    }
}

// Register in Program.cs
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
{
    tracing.AddSource("MyApp.MyService");
});
```

### Can I export telemetry to other systems?

Yes! Configure OpenTelemetry exporters:

```csharp
builder.Services.ConfigureOpenTelemetryMeterProvider(metrics =>
{
    metrics.AddPrometheusExporter();
});

builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
{
    tracing.AddJaegerExporter();
});
```

### How long is telemetry data retained?

In local development: Data is only available while the dashboard is running (in-memory).

In production (Application Insights):
- Raw data: 90 days default
- Aggregated data: 90 days to 730 days
- Configure retention in Azure Portal

## 🚀 Deployment Questions

### Do I need to deploy to Azure?

No! Aspire apps are just .NET apps. You can deploy to:
- Azure Container Apps (easiest with `azd`)
- Kubernetes (via manifests)
- Docker/Docker Compose
- Any hosting platform

### How much does Azure deployment cost?

Azure Container Apps pricing (as of 2024):
- Consumption plan: Pay per second of execution
- Free tier: 180,000 vCPU-seconds, 360,000 GiB-seconds per month
- Typical small app: $5-20/month
- Can scale to zero when not in use

Use [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/) for estimates.

### Can I deploy to on-premises?

Yes! Options include:
1. **Docker Compose**: Generate from Aspire manifest
2. **Kubernetes**: Use Aspirate or similar tools
3. **Manual deployment**: Deploy as regular .NET apps

### How do I implement CI/CD?

See Exercise 5 for GitHub Actions. For other platforms:

**Azure DevOps:**
```yaml
steps:
- task: AzureCLI@2
  inputs:
    azureSubscription: 'YourSubscription'
    scriptType: 'bash'
    scriptLocation: 'inlineScript'
    inlineScript: |
      azd deploy --no-prompt
```

**GitLab CI:**
```yaml
deploy:
  script:
    - curl -fsSL https://aka.ms/install-azd.sh | bash
    - azd deploy --no-prompt
```

### Can I use Terraform or Pulumi instead of Bicep?

Yes! Aspire generates manifests that can be consumed by any IaC tool:

```bash
# Generate manifest
dotnet run --project AppHost -- --publisher manifest --output-path manifest.json

# Convert to your preferred IaC format
# Use tools like CDK or custom converters
```

## 🎓 Learning Questions

### What should I learn before starting Aspire?

Prerequisites:
- ✅ C# and .NET basics
- ✅ ASP.NET Core fundamentals
- ✅ Basic HTTP/REST concepts
- ⚠️ Docker basics (helpful but not required)
- ⚠️ Cloud concepts (for deployment only)

You don't need:
- ❌ Kubernetes expertise
- ❌ Infrastructure-as-Code knowledge
- ❌ Advanced distributed systems knowledge

### In what order should I do the exercises?

Follow the recommended order:
1. **Exercise 1**: First Aspire App (essential)
2. **Exercise 2**: Adding Components (essential)
3. **Exercise 3**: Multi-Service App (recommended)
4. **Exercise 4**: Observability (recommended)
5. **Exercise 5**: Deployment (optional, requires Azure)

### How long does each exercise take?

| Exercise | Time | Difficulty |
|----------|------|------------|
| Exercise 1 | 45-60 min | Beginner |
| Exercise 2 | 60-90 min | Intermediate |
| Exercise 3 | 90-120 min | Intermediate-Advanced |
| Exercise 4 | 60-90 min | Intermediate |
| Exercise 5 | 90-120 min | Advanced |

Total: 6-8 hours for all exercises

### Where can I find more examples?

Official resources:
- [Aspire Samples](https://github.com/dotnet/aspire-samples)
- [eShop Sample](https://github.com/dotnet/eshop) - Full e-commerce app
- [Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)

Community resources:
- [Awesome Aspire](https://github.com/thinktecture-labs/awesome-dotnet-aspire)
- YouTube tutorials
- Blog posts tagged with #dotnetaspire

## 🔧 Component Questions

### What components are available?

Built-in components include:
- **Databases**: PostgreSQL, SQL Server, MySQL, MongoDB, Oracle
- **Caching**: Redis, Garnet
- **Messaging**: RabbitMQ, Azure Service Bus, Kafka
- **Storage**: Azure Blob, AWS S3
- **Search**: Elasticsearch
- **APIs**: Azure OpenAI, Milvus

See [Aspire Components Catalog](https://learn.microsoft.com/dotnet/aspire/fundamentals/components-overview).

### How do I create a custom component?

```csharp
// Define hosting extension
public static class MyComponentExtensions
{
    public static IResourceBuilder<ContainerResource> AddMyService(
        this IDistributedApplicationBuilder builder,
        string name)
    {
        return builder.AddContainer(name, "myimage")
            .WithEndpoint(port: 8080, targetPort: 8080);
    }
}

// Usage in AppHost
builder.AddMyService("myservice");
```

### Can I use multiple databases?

Yes! Add as many as you need:

```csharp
var postgres = builder.AddPostgres("postgres");
var catalogDb = postgres.AddDatabase("catalog");
var identityDb = postgres.AddDatabase("identity");

var sqlserver = builder.AddSqlServer("sqlserver");
var ordersDb = sqlserver.AddDatabase("orders");

builder.AddProject<Projects.CatalogApi>("catalogapi")
    .WithReference(catalogDb);

builder.AddProject<Projects.OrderApi>("orderapi")
    .WithReference(ordersDb);
```

### How do I upgrade Aspire versions?

```bash
# Update workload
dotnet workload update

# Update NuGet packages in all projects
dotnet list package --outdated
dotnet add package Aspire.Hosting --version 8.x.x

# Check for breaking changes in release notes
# https://github.com/dotnet/aspire/releases
```

## 🎨 Architecture Questions

### Should I use Aspire for a simple CRUD app?

Yes! Even simple apps benefit from:
- Easy database integration
- Built-in health checks
- Automatic observability
- Simplified deployment

Start simple and add complexity as needed.

### When should I split into multiple services?

Consider multiple services when:
- Different deployment cadences
- Different scaling requirements  
- Clear bounded contexts
- Different technology needs
- Team organization boundaries

Don't split too early - start with a modular monolith.

### How do I handle shared code?

Options:
1. **Shared library**: Create a class library referenced by services
2. **NuGet packages**: For stable, versioned shared code
3. **Duplication**: For truly independent services (preferred in some cases)

```bash
# Create shared library
dotnet new classlib -n SharedLibrary
dotnet sln add SharedLibrary

# Reference from services
cd ApiService
dotnet add reference ../SharedLibrary
```

### How do I implement authentication?

Use standard ASP.NET Core authentication:

```csharp
builder.Services.AddAuthentication()
    .AddJwtBearer();

// Aspire doesn't change authentication patterns
// Use OAuth, JWT, cookies, etc. as normal
```

For identity service:
```csharp
// In AppHost
var identityDb = postgres.AddDatabase("identity");
var identityApi = builder.AddProject<Projects.IdentityApi>("identity")
    .WithReference(identityDb);

// Other services reference identity
var catalogApi = builder.AddProject<Projects.CatalogApi>("catalog")
    .WithReference(identityApi);
```

## 🔍 Debugging Questions

### How do I debug services?

**Option 1: Debug in IDE**
- Set breakpoints in service code
- Run AppHost with F5
- Debugger attaches automatically

**Option 2: Attach to running process**
- Run with `dotnet run`
- Debug → Attach to Process
- Select your service process

**Option 3: Remote debugging**
- Set `DOTNET_MODIFIABLE_ASSEMBLIES=debug` for hot reload
- Use `dotnet watch` for automatic reloading

### Can I run one service at a time?

Yes, but you'll lose orchestration benefits:

```bash
# Run just the API (without AppHost)
cd ApiService
dotnet run
```

You'll need to:
- Manually start dependencies (Docker containers)
- Configure connection strings manually
- Lose dashboard functionality

Better approach: Use launch profiles in AppHost to control which services start.

### How do I view raw HTTP traffic?

**Option 1: Dashboard traces**
- Traces tab shows all HTTP calls
- Expand spans to see request/response details

**Option 2: Fiddler/Charles Proxy**
- Configure HTTP client to use proxy

```csharp
builder.Services.AddHttpClient()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        Proxy = new WebProxy("http://localhost:8888")
    });
```

**Option 3: Logging**
```csharp
// Enable HTTP client logging
builder.Services.AddHttpClient("MyClient")
    .AddLogger(); // Logs all requests/responses
```

## 💡 Best Practices Questions

### Should I use `EnsureCreated()` or Migrations?

**Development:**
```csharp
db.Database.EnsureCreated(); // Quick and easy
```

**Production:**
```csharp
db.Database.Migrate(); // Use migrations for production
```

Better approach: Use migrations everywhere:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### How do I handle configuration?

Use standard ASP.NET Core configuration:

```csharp
// appsettings.json
{
  "MySettings": {
    "Value": "default"
  }
}

// appsettings.Development.json
{
  "MySettings": {
    "Value": "development"
  }
}

// Access in code
var value = builder.Configuration["MySettings:Value"];
```

Aspire adds connection strings automatically - don't hardcode them!

### Should I test locally or in Azure?

**Develop locally:**
- Fast iteration
- No cloud costs
- Work offline
- Use Aspire dashboard

**Test in Azure:**
- Before production deployment
- Test Azure-specific features
- Performance testing
- Integration testing with Azure services

Use both: Local for development, Azure for final validation.

## 🎯 Next Steps Questions

### What should I build next?

Project ideas:
1. **Personal dashboard**: Aggregate data from multiple APIs
2. **Todo app with sync**: Multi-user with real-time updates
3. **E-commerce backend**: Practice exercise 3 concepts
4. **API gateway**: Route requests to multiple services
5. **Monitoring system**: Collect and visualize metrics

### How do I stay updated?

Follow:
- [.NET Blog](https://devblogs.microsoft.com/dotnet/)
- [Aspire GitHub](https://github.com/dotnet/aspire)
- [.NET YouTube](https://youtube.com/@dotnet)
- [#dotnetaspire on Twitter/X](https://twitter.com/search?q=%23dotnetaspire)
- Weekly .NET newsletters

### Where can I get help?

Community:
- [.NET Discord](https://aka.ms/aspire/discord) - Active community
- [Stack Overflow](https://stackoverflow.com/questions/tagged/.net-aspire)
- [Microsoft Q&A](https://learn.microsoft.com/answers/tags/455/dotnet-aspire)
- [Reddit r/dotnet](https://reddit.com/r/dotnet)

Official:
- [Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [GitHub Issues](https://github.com/dotnet/aspire/issues)
- [Microsoft Learn](https://learn.microsoft.com/training/browse/?terms=aspire)

---

**Have a question not answered here?**

Please open an issue or contribute to this FAQ!
