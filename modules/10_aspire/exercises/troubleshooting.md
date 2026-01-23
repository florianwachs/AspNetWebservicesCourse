# Troubleshooting Guide

Common issues and solutions for .NET Aspire exercises.

## 🐳 Docker Issues

### Problem: Docker is not running

**Error:**
```
Cannot connect to the Docker daemon
```

**Solution:**
```bash
# Windows/Mac: Start Docker Desktop

# Linux: Start Docker service
sudo systemctl start docker

# Verify Docker is running
docker ps
```

### Problem: Port conflicts

**Error:**
```
Address already in use
```

**Solution:**
```bash
# Find process using the port
# Windows
netstat -ano | findstr :5000

# Mac/Linux
lsof -i :5000

# Kill the process
# Windows
taskkill /PID <PID> /F

# Mac/Linux
kill -9 <PID>

# Or stop the conflicting container
docker ps
docker stop <container-id>
```

### Problem: Docker out of space

**Error:**
```
No space left on device
```

**Solution:**
```bash
# Clean up Docker resources
docker system prune -a --volumes

# Check disk usage
docker system df
```

## 🔧 .NET Aspire Issues

### Problem: Aspire workload not installed

**Error:**
```
Unknown project template: aspire-starter
```

**Solution:**
```bash
# Install Aspire workload
dotnet workload update
dotnet workload install aspire

# Verify installation
dotnet workload list
dotnet new list aspire
```

### Problem: ServiceDefaults not found

**Error:**
```
The type or namespace name 'ServiceDefaults' could not be found
```

**Solution:**
```bash
# Add project reference
cd YourApiProject
dotnet add reference ../YourApp.ServiceDefaults

# Restore packages
dotnet restore
```

### Problem: Dashboard not accessible

**Error:**
```
Unable to connect to https://localhost:17005
```

**Solution:**
1. Check if AppHost is running
2. Check console output for actual URL
3. Try the http endpoint (usually port 15001)
4. Check firewall settings
5. Try `https://localhost:15001` or `http://localhost:15001`

```bash
# Check if port is listening
# Windows
netstat -ano | findstr :17005

# Mac/Linux
lsof -i :17005
```

## 🗄️ Database Issues

### Problem: PostgreSQL connection failed

**Error:**
```
Npgsql.NpgsqlException: Connection refused
```

**Solution:**
```bash
# Check if PostgreSQL container is running
docker ps | grep postgres

# Check container logs
docker logs <postgres-container-id>

# Verify connection string in dashboard
# Navigate to Resources → postgres → View connection strings

# Restart container
docker restart <postgres-container-id>
```

### Problem: Database migrations fail

**Error:**
```
A connection was successfully established, but then an error occurred during login
```

**Solution:**
```bash
# Wait for PostgreSQL to be ready (startup delay)
# Add retry logic or use connection resilience

# Or use Database.EnsureCreated() instead of migrations during development
```

In your code:
```csharp
// Add retry logic
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<YourDbContext>();
    var retries = 5;
    while (retries > 0)
    {
        try
        {
            await db.Database.EnsureCreatedAsync();
            break;
        }
        catch (Exception ex)
        {
            retries--;
            if (retries == 0) throw;
            await Task.Delay(2000);
        }
    }
}
```

## 🗂️ Redis Issues

### Problem: Redis connection timeout

**Error:**
```
StackExchange.Redis.RedisConnectionException: No connection is available
```

**Solution:**
```bash
# Check if Redis container is running
docker ps | grep redis

# Check Redis logs
docker logs <redis-container-id>

# Test Redis connection
docker exec -it <redis-container-id> redis-cli ping
# Should respond with: PONG

# Restart Redis container
docker restart <redis-container-id>
```

### Problem: Redis data persistence issues

**Solution:**
Configure Redis with persistent volume in AppHost:

```csharp
var redis = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume(); // Adds persistent volume
```

## 📨 RabbitMQ Issues

### Problem: RabbitMQ connection failed

**Error:**
```
RabbitMQ.Client.Exceptions.BrokerUnreachableException
```

**Solution:**
```bash
# Check if RabbitMQ is running
docker ps | grep rabbitmq

# Check RabbitMQ logs
docker logs <rabbitmq-container-id>

# Access RabbitMQ management UI
# URL shown in Aspire dashboard (usually http://localhost:15672)
# Default credentials: guest/guest

# Restart RabbitMQ
docker restart <rabbitmq-container-id>
```

### Problem: Messages not being consumed

**Solution:**
```csharp
// Ensure consumer is registered as background service
builder.Services.AddHostedService<YourConsumerService>();

// Check queue bindings in RabbitMQ Management UI
// Verify exchange and routing keys are correct

// Add logging to consumer
_logger.LogInformation("Consumer started and listening...");
```

## 🌐 HTTP Client Issues

### Problem: Service discovery not working

**Error:**
```
HttpRequestException: Name or service not known (https+http://apiservice)
```

**Solution:**
```csharp
// Ensure ServiceDefaults are added
builder.AddServiceDefaults();

// Verify service name matches AppHost configuration
builder.Services.AddHttpClient<MyApiClient>(client =>
{
    // Name must match AddProject<T>("NAME") in AppHost
    client.BaseAddress = new("https+http://apiservice");
});
```

### Problem: HTTPS certificate errors in development

**Error:**
```
The SSL connection could not be established
```

**Solution:**
```bash
# Trust development certificate
dotnet dev-certs https --trust

# Or configure HttpClient to accept any certificate (DEV ONLY!)
builder.Services.AddHttpClient<MyApiClient>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });
```

## 📊 Observability Issues

### Problem: Traces not appearing in dashboard

**Solution:**
```csharp
// Ensure ServiceDefaults are added (includes OpenTelemetry)
builder.AddServiceDefaults();

// For custom ActivitySources, register them
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
{
    tracing.AddSource(MyActivitySource.Name);
});

// Verify activity is started
using var activity = MyActivitySource.StartActivity("OperationName");
// ... your code ...
```

### Problem: Metrics not showing

**Solution:**
```csharp
// Register custom meters
builder.Services.ConfigureOpenTelemetryMeterProvider(metrics =>
{
    metrics.AddMeter("MyApp.*"); // Pattern match your meters
});

// Verify meter name matches
var meter = meterFactory.Create("MyApp.Api"); // Must match pattern above
```

### Problem: Logs missing context

**Solution:**
```csharp
// Use structured logging
_logger.LogInformation("Order {OrderId} created by {UserId}", orderId, userId);

// Not: _logger.LogInformation($"Order {orderId} created by {userId}");

// Add correlation ID to all logs
Activity.Current?.SetTag("correlation_id", correlationId);
```

## 🚀 Deployment Issues

### Problem: azd command not found

**Solution:**
```bash
# Reinstall Azure Developer CLI
# Windows
winget install microsoft.azd

# Mac
brew upgrade azd

# Linux
curl -fsSL https://aka.ms/install-azd.sh | bash

# Restart terminal and verify
azd version
```

### Problem: Azure authentication failed

**Error:**
```
AADSTS50020: User account from identity provider does not exist
```

**Solution:**
```bash
# Clear cached credentials
azd auth logout
az logout

# Login again
azd auth login
az login

# Verify correct subscription
az account show
az account set --subscription "Your-Subscription-Name"
```

### Problem: Container build fails

**Error:**
```
Failed to build container image
```

**Solution:**
```bash
# Check Dockerfile exists and is valid
# Ensure all project references are correct

# Try building locally
docker build -f ApiService/Dockerfile .

# Check Azure Container Registry authentication
az acr login --name YourACRName

# Increase deployment timeout
azd deploy --debug
```

### Problem: Container Apps not starting

**Solution:**
```bash
# Check container logs
az containerapp logs show \
  --name YourAppName \
  --resource-group YourResourceGroup \
  --tail 50

# Check replica status
az containerapp replica list \
  --name YourAppName \
  --resource-group YourResourceGroup

# Verify environment variables and secrets are set
az containerapp show \
  --name YourAppName \
  --resource-group YourResourceGroup \
  --query properties.configuration
```

## 🔍 General Debugging Tips

### Enable Verbose Logging

```csharp
// In appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug",
      "Aspire": "Debug"
    }
  }
}
```

### Check Service Dependencies

```bash
# View all running containers
docker ps

# Check resource usage
docker stats

# View all Aspire logs
# In dashboard, go to Console Logs and select "All"
```

### Use Health Check Endpoints

```bash
# Check service health
curl https://localhost:7xxx/health

# Get detailed health status
curl https://localhost:7xxx/health | jq
```

### Inspect Environment Variables

```csharp
app.MapGet("/debug/env", (IConfiguration config) =>
{
    return new
    {
        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
        ConnectionStrings = config.GetSection("ConnectionStrings").GetChildren()
            .ToDictionary(x => x.Key, x => "***"), // Don't expose actual values!
        ServiceEndpoints = config.GetSection("services").GetChildren()
            .ToDictionary(x => x.Key, x => x.Value)
    };
});
```

## 🆘 Getting More Help

### Console Output
Always check the console output from `dotnet run` - it often contains helpful error messages and URLs.

### Docker Logs
```bash
docker logs <container-id> --tail 100 --follow
```

### Aspire Dashboard
The dashboard is your best debugging tool:
- Console Logs: Real-time logs from all services
- Structured Logs: Searchable, filterable logs
- Traces: Request flow visualization
- Metrics: Performance monitoring

### Enable Debug Output

```bash
# Run with verbose output
dotnet run --verbosity diagnostic

# Set environment variable for more details
export ASPIRE_ALLOW_UNSECURED_TRANSPORT=true
```

### Check GitHub Issues

Search for similar issues:
- [.NET Aspire GitHub Issues](https://github.com/dotnet/aspire/issues)
- [.NET Core GitHub Issues](https://github.com/dotnet/core/issues)

### Community Resources

- [.NET Aspire Discord](https://aka.ms/aspire/discord)
- [Stack Overflow - .NET Aspire tag](https://stackoverflow.com/questions/tagged/.net-aspire)
- [Microsoft Q&A](https://learn.microsoft.com/answers/tags/455/dotnet-aspire)

## 📝 Before Asking for Help

When seeking help, provide:

1. **Exact error message** (full stack trace)
2. **Steps to reproduce** the issue
3. **Your environment**:
   ```bash
   dotnet --info
   docker --version
   azd version
   ```
4. **Relevant code** (minimal reproducible example)
5. **Configuration files** (sanitized)
6. **Console output** or logs

This information helps others assist you more effectively.
