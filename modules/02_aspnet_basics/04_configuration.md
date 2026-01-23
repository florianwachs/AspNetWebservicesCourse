# Lesson 04: Configuration in ASP.NET Core

## Table of Contents

1. [Introduction](#introduction)
2. [The Problem: Hard-Coded Values](#the-problem-hard-coded-values)
3. [Configuration Sources and Precedence](#configuration-sources-and-precedence)
4. [Working with appsettings.json](#working-with-appsettingsjson)
5. [Environment-Specific Configuration](#environment-specific-configuration)
6. [Environment Variables and User Secrets](#environment-variables-and-user-secrets)
7. [The Options Pattern](#the-options-pattern)
8. [Best Practices and Security](#best-practices-and-security)
9. [Exercises](#exercises)

## Introduction

Configuration allows you to change application behavior without recompiling, manage environment-specific settings, and keep sensitive data secure.

### Learning Objectives

- Understand ASP.NET Core's configuration system
- Use `appsettings.json` and environment-specific files
- Work with environment variables and User Secrets
- Implement the Options pattern with strongly-typed classes
- Follow configuration security best practices

### What You'll Build

Enhance the Chuck Norris Joke API with:
- ✅ Configurable pagination
- ✅ Feature flags
- ✅ Secure API key management
- ✅ Environment-specific settings

## The Problem: Hard-Coded Values

### Example: Hard-Coded Settings ❌

```csharp
// BAD PRACTICE - Don't do this!
app.MapGet("/api/jokes", (IJokeService jokeService) =>
{
    int pageSize = 10; // Hard-coded!
    return jokeService.GetAll().Take(pageSize);
});

app.MapPost("/api/jokes", (Joke joke, HttpContext ctx) =>
{
    // NEVER hard-code secrets!
    if (ctx.Request.Headers["X-API-Key"] != "secret-123")
        return Results.Unauthorized();
    return Results.Ok(jokeService.Create(joke));
});
```

### Problems

1. ❌ Must recompile to change settings
2. ❌ Same settings for dev/staging/production
3. ❌ Security risk - secrets in source control
4. ❌ Difficult to test with different configurations

## Configuration Sources and Precedence

ASP.NET Core loads configuration from multiple sources. **Later sources override earlier ones!**

```
1. appsettings.json                    (Base settings)
2. appsettings.{Environment}.json      (Environment override)
3. User Secrets                        (Development only)
4. Environment Variables               (Container/OS)
5. Command-line Arguments              (Runtime)
```

### Precedence Example

**appsettings.json**: `PageSize: 10`  
**appsettings.Production.json**: `PageSize: 50`  
**Environment Variable**: `Pagination__PageSize=20`  
**Result**: PageSize = **20** (environment variable wins)

### Accessing Configuration

```csharp
var builder = WebApplication.CreateBuilder(args);

// String access (colon for nesting)
string? name = builder.Configuration["JokeApi:Name"];

// Typed access with default
int pageSize = builder.Configuration.GetValue<int>("Pagination:PageSize", 10);

// Connection strings
string connStr = builder.Configuration.GetConnectionString("DefaultConnection");
```

## Working with appsettings.json

### Example Structure

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  
  "JokeApi": {
    "Name": "Chuck Norris Joke API",
    "Version": "1.0.0"
  },
  
  "Pagination": {
    "DefaultPageSize": 10,
    "MaxPageSize": 100
  },
  
  "Features": {
    "EnableCaching": true,
    "EnableDetailedErrors": false
  },
  
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=JokesDb;Trusted_Connection=true;"
  }
}
```

## Environment-Specific Configuration

ASP.NET Core automatically loads environment-specific files based on `ASPNETCORE_ENVIRONMENT`.

### appsettings.Development.json

```json
{
  "Logging": { "LogLevel": { "Default": "Debug" } },
  "Features": {
    "EnableDetailedErrors": true,
    "EnableCaching": false
  }
}
```

### appsettings.Production.json

```json
{
  "Logging": { "LogLevel": { "Default": "Warning" } },
  "Features": {
    "EnableDetailedErrors": false,
    "EnableCaching": true
  },
  "Pagination": { "DefaultPageSize": 50 }
}
```

### Using Environment in Code

```csharp
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwagger();
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddResponseCaching();
}
```

### Setting Environment

**launchSettings.json**:
```json
{
  "profiles": {
    "Development": {
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## Environment Variables and User Secrets

### Environment Variables

Use double underscores (`__`) for nesting:

```bash
# Windows PowerShell
$env:Pagination__DefaultPageSize = "20"

# Linux/macOS/Bash
export Pagination__DefaultPageSize=20

# Docker
docker run -e Pagination__DefaultPageSize=20 myapp
```

**Kubernetes ConfigMap**:
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: joke-api-config
data:
  Pagination__DefaultPageSize: "25"
  Features__EnableCaching: "true"
```

### User Secrets (Development Only)

**Never commit secrets!** User Secrets stores sensitive data outside your project.

```bash
# Initialize
dotnet user-secrets init

# Add secrets
dotnet user-secrets set "ExternalApi:ApiKey" "sk_test_1234"
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Password=Dev123"

# List secrets
dotnet user-secrets list

# Clear all
dotnet user-secrets clear
```

**Storage Location**:
- Windows: `%APPDATA%\Microsoft\UserSecrets\<id>\secrets.json`
- Linux/macOS: `~/.microsoft/usersecrets/<id>/secrets.json`

⚠️ User Secrets are NOT encrypted and only work in Development!

## The Options Pattern

The Options pattern provides strongly-typed configuration.

### Benefits

✅ Type safety  
✅ IntelliSense  
✅ Validation  
✅ Testability

### Three Interfaces

| Interface | Lifetime | Use Case |
|-----------|----------|----------|
| `IOptions<T>` | Singleton | Config doesn't change |
| `IOptionsSnapshot<T>` | Scoped | May change per request |
| `IOptionsMonitor<T>` | Singleton | Real-time + notifications |

### Implementation

**1. Create Settings Class**:
```csharp
public class PaginationSettings
{
    public int DefaultPageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 100;
}

public class FeatureSettings
{
    public bool EnableCaching { get; set; }
    public bool EnableDetailedErrors { get; set; }
}
```

**2. Add to appsettings.json**:
```json
{
  "Pagination": {
    "DefaultPageSize": 10,
    "MaxPageSize": 100
  },
  "Features": {
    "EnableCaching": true
  }
}
```

**3. Register in DI**:
```csharp
builder.Services.Configure<PaginationSettings>(
    builder.Configuration.GetSection("Pagination"));
builder.Services.Configure<FeatureSettings>(
    builder.Configuration.GetSection("Features"));
```

**4. Inject and Use**:
```csharp
public class JokeService : IJokeService
{
    private readonly IJokeRepository _repository;
    private readonly PaginationSettings _settings;
    
    public JokeService(
        IJokeRepository repository,
        IOptions<PaginationSettings> options)
    {
        _repository = repository;
        _settings = options.Value;
    }
    
    public IEnumerable<Joke> GetAll(int? pageSize = null)
    {
        int size = pageSize ?? _settings.DefaultPageSize;
        if (size > _settings.MaxPageSize)
            size = _settings.MaxPageSize;
        
        return _repository.GetAll().Take(size);
    }
}
```

**5. Use in Endpoints**:
```csharp
app.MapGet("/api/jokes", (
    IJokeService jokeService,
    IOptions<PaginationSettings> options,
    int? pageSize) =>
{
    var settings = options.Value;
    int size = pageSize ?? settings.DefaultPageSize;
    
    if (size > settings.MaxPageSize)
        return Results.BadRequest($"Max size: {settings.MaxPageSize}");
    
    return Results.Ok(jokeService.GetAll(size));
});
```

### IOptionsSnapshot (Reloads Per Request)

```csharp
public class CachedService
{
    private readonly IOptionsSnapshot<FeatureSettings> _features;
    
    public CachedService(IOptionsSnapshot<FeatureSettings> features)
    {
        _features = features;
    }
    
    public void Process()
    {
        if (_features.Value.EnableCaching)  // Latest value per request
        {
            // Use cache
        }
    }
}
```

### IOptionsMonitor (Real-Time Updates)

```csharp
public class MonitoredService
{
    private readonly IOptionsMonitor<FeatureSettings> _monitor;
    
    public MonitoredService(IOptionsMonitor<FeatureSettings> monitor)
    {
        _monitor = monitor;
        _monitor.OnChange(s => Console.WriteLine($"Changed: {s.EnableCaching}"));
    }
    
    public void Process()
    {
        var current = _monitor.CurrentValue;  // Always latest
    }
}
```

## Best Practices and Security

### 1. Use Strongly-Typed Settings

✅ **DO:**
```csharp
public class EmailSettings { /* properties */ }
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
```

❌ **DON'T:**
```csharp
string smtp = builder.Configuration["Email:SmtpServer"]; // Magic strings
```

### 2. Validate at Startup

```csharp
using System.ComponentModel.DataAnnotations;

public class EmailSettings
{
    [Required, EmailAddress]
    public string FromAddress { get; set; } = string.Empty;
    
    [Range(1, 65535)]
    public int Port { get; set; } = 587;
}

builder.Services.AddOptions<EmailSettings>()
    .Bind(builder.Configuration.GetSection("Email"))
    .ValidateDataAnnotations()
    .ValidateOnStart();  // Fail fast!
```

### 3. Provide Default Values

```csharp
public class PaginationSettings
{
    public int DefaultPageSize { get; set; } = 10;  // ✅
}
```

### 4. Group Related Settings

✅ **DO**: `{ "Email": { "SmtpServer": "...", "Port": 587 } }`  
❌ **DON'T**: `{ "EmailSmtpServer": "...", "EmailPort": 587 }`

### 5. Security Best Practices

❌ **NEVER commit secrets**:
```json
// appsettings.json - DON'T!
{ "ApiKey": "sk_live_secret" }
```

✅ **Use User Secrets (Development)**:
```bash
dotnet user-secrets set "ApiKey" "sk_test_dev_key"
```

✅ **Use Environment Variables (Production)**:
```bash
export ApiKey="sk_live_production_key"
```

### 6. Placeholder Pattern

**appsettings.json** (checked in):
```json
{
  "ExternalApi": {
    "ApiKey": "YOUR_API_KEY_HERE",
    "BaseUrl": "https://api.example.com"
  }
}
```

**README.md**:
```markdown
## Setup
1. `dotnet user-secrets init`
2. `dotnet user-secrets set "ExternalApi:ApiKey" "your-key"`
```

### 7. Update .gitignore

```gitignore
**/appsettings.*.Local.json
.env
.env.local
```

---

## Exercises

### Exercise 1: Add Pagination Configuration

Implement configurable pagination.

**Requirements:**
1. Create `PaginationSettings` (DefaultPageSize, MaxPageSize)
2. Add to `appsettings.json`
3. Register with Options pattern
4. Update `GET /api/jokes` with page/pageSize query parameters
5. Return: `{ "page": 1, "pageSize": 10, "totalCount": 50, "data": [] }`

### Exercise 2: Feature Flags

Create a feature toggle system.

**Requirements:**
1. Create `FeatureSettings` (EnableSwagger, EnableDetailedErrors, EnableRateLimiting)
2. Configure differently for Dev vs Production
3. Use flags to conditionally enable features
4. Create `GET /api/features` endpoint

### Exercise 3: External API Configuration

Add external API configuration.

**Requirements:**
1. Create `ExternalApiSettings` (ApiKey, BaseUrl, TimeoutSeconds)
2. Store API key in User Secrets
3. Use placeholder in `appsettings.json`
4. Different timeouts: Dev (30s), Production (10s)

### Exercise 4: Environment-Specific Logging

Configure log levels per environment.

**Requirements:**
1. Development: Default=Debug
2. Production: Default=Warning
3. Test logging behavior in each environment

### Exercise 5: Validate Configuration

Add startup validation.

**Requirements:**
1. Add data annotations ([Required], [Range], [Url])
2. Use ValidateDataAnnotations() and ValidateOnStart()
3. Test with invalid config
4. Add custom validation:
   ```csharp
   .Validate(s => s.MaxPageSize >= s.DefaultPageSize,
             "MaxPageSize must be >= DefaultPageSize")
   ```

### Exercise 6: Database Configuration

Prepare for Module 04.

**Requirements:**
1. Create `DatabaseSettings` (ConnectionString, CommandTimeout, EnableRetryOnFailure)
2. Add to `appsettings.json` with placeholder
3. Store password in User Secrets
4. Different DBs per environment (JokesDb_Dev, JokesDb_Production)

### Exercise 7: Explore IOptionsSnapshot

Experiment with runtime changes.

**Requirements:**
1. Create service using `IOptionsSnapshot<T>`
2. Modify `appsettings.json` while running
3. Observe new value on next request
4. Compare with `IOptions<T>`
5. Document when to use each

**Hint**: ASP.NET Core watches `appsettings.json` in Development.

---

## Summary

### What You Learned

✅ ASP.NET Core's configuration system  
✅ Configuration sources and precedence  
✅ appsettings.json structure  
✅ Environment-specific configuration  
✅ Environment variables and User Secrets  
✅ Options pattern (IOptions, IOptionsSnapshot, IOptionsMonitor)  
✅ Strongly-typed configuration  
✅ Validation and security best practices

### Key Takeaways

1. **Never commit secrets** to source control
2. **Use strongly-typed settings** (avoid magic strings)
3. **Validate configuration** at startup (fail fast)
4. **Environment variables** for production secrets
5. **User Secrets** for local development only
6. **Provide default values** in classes
7. **Choose appropriate Options interface** for your needs

### Next Steps

Next lesson covers:
- Middleware and the request pipeline
- Custom middleware components
- Error handling and logging
- CORS and security headers

---

**Additional Resources:**
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Options pattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)
- [Safe storage of app secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Multiple environments](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments)
