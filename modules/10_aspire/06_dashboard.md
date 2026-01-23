# Lesson 6: Aspire Dashboard - Observability and Debugging

## Introduction

The Aspire Dashboard is a powerful web-based tool for monitoring, debugging, and understanding your distributed applications. It provides real-time insights into logs, traces, metrics, and resource health—all in one unified interface. The dashboard is automatically launched when you run your AppHost and is essential for developing and troubleshooting cloud-native applications.

In this lesson, we'll explore every feature of the Aspire Dashboard, learn how to leverage it for debugging, and discover tips and tricks for maximizing your productivity.

## Table of Contents

1. [Dashboard Overview](#dashboard-overview)
2. [Getting Started](#getting-started)
3. [Resources View](#resources-view)
4. [Console Logs](#console-logs)
5. [Structured Logs](#structured-logs)
6. [Distributed Tracing](#distributed-tracing)
7. [Metrics and Performance](#metrics-and-performance)
8. [Environment Variables](#environment-variables)
9. [Debugging Distributed Applications](#debugging-distributed-applications)
10. [Dashboard Configuration](#dashboard-configuration)
11. [Tips and Tricks](#tips-and-tricks)

## Dashboard Overview

### What is the Aspire Dashboard?

The Aspire Dashboard is a web application that:
- Monitors all services and resources in your application
- Aggregates logs from multiple sources
- Visualizes distributed traces
- Displays real-time metrics
- Shows resource health and status
- Provides environment configuration inspection

### Dashboard Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                    Aspire Dashboard                          │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │  Resources  │  │    Logs     │  │   Traces    │         │
│  │             │  │             │  │             │         │
│  │  • Status   │  │  • Console  │  │  • Spans    │         │
│  │  • Health   │  │  • Struct.  │  │  • Timeline │         │
│  │  • Endpoint │  │  • Filter   │  │  • Details  │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
│                                                              │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │   Metrics   │  │   Environ.  │  │   Config    │         │
│  │             │  │             │  │             │         │
│  │  • Counters │  │  • Vars     │  │  • Settings │         │
│  │  • Gauges   │  │  • Secrets  │  │  • Tokens   │         │
│  │  • Histog.  │  │  • Connect. │  │  • Theme    │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

### Key Features

1. **Real-time monitoring**: Live updates of all application activity
2. **Cross-service visibility**: See the entire application topology
3. **Intelligent filtering**: Quickly find what you're looking for
4. **Correlation**: Link logs, traces, and metrics across services
5. **Zero configuration**: Works out of the box with Aspire applications

## Getting Started

### Launching the Dashboard

The dashboard launches automatically when you start your AppHost:

```bash
cd MyApp.AppHost
dotnet run
```

Output:
```
info: Aspire.Hosting.DistributedApplication[0]
      Aspire version: 10.0.0
info: Aspire.Hosting.DistributedApplication[0]
      Dashboard running at: https://localhost:15001
```

### Accessing the Dashboard

Open your browser to the URL shown (typically `https://localhost:15001`).

```
Default URLs:
┌──────────────────────────────────────────────┐
│ Dashboard:  https://localhost:15001          │
│ OTLP:       http://localhost:18889           │
│ Token:      Automatically passed to services │
└──────────────────────────────────────────────┘
```

### Dashboard Navigation

```
┌────────────────────────────────────────────────────────────┐
│  🏠 Aspire Dashboard                          [Theme] [?]  │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  📋 Resources  📝 Console  🔍 Structured  🔎 Traces        │
│  📊 Metrics    🌍 Environment                              │
│                                                            │
│  [Active View Content]                                     │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

## Resources View

The Resources view is the dashboard's home page, showing all services, containers, and resources.

### Resource Table

```
┌──────────────┬─────────┬───────────┬──────────────┬───────────┐
│ Name         │ State   │ Health    │ Start Time   │ Endpoints │
├──────────────┼─────────┼───────────┼──────────────┼───────────┤
│ frontend     │ Running │ Healthy   │ 12:30:45     │ http, ... │
│ catalogapi   │ Running │ Healthy   │ 12:30:47     │ http, ... │
│ orderapi     │ Running │ Healthy   │ 12:30:48     │ http, ... │
│ postgres     │ Running │ Healthy   │ 12:30:46     │ 5432      │
│ redis        │ Running │ Healthy   │ 12:30:46     │ 6379      │
│ rabbitmq     │ Running │ Healthy   │ 12:30:46     │ 5672, ... │
└──────────────┴─────────┴───────────┴──────────────┴───────────┘
```

### Resource States

- **Starting**: Resource is initializing
- **Running**: Resource is active
- **Healthy**: Health checks passing
- **Unhealthy**: Health checks failing
- **Exited**: Resource has stopped

### Resource Details

Click on a resource to see:
- **Endpoints**: All exposed ports and URLs (clickable)
- **Environment Variables**: Configuration injected by AppHost
- **Console Output**: Stdout/stderr from the resource
- **Logs**: Structured logs with filtering
- **Metrics**: Resource-specific metrics

### Resource Actions

```
┌──────────────────────────────────────┐
│ catalogapi                           │
├──────────────────────────────────────┤
│ Actions:                             │
│  🔗 View Endpoint (http)             │
│  📝 View Console Logs                │
│  🔍 View Structured Logs             │
│  📊 View Metrics                     │
│  🌍 View Environment                 │
│  🔄 Restart                          │
│  🛑 Stop                             │
└──────────────────────────────────────┘
```

## Console Logs

Console logs show raw stdout/stderr output from resources.

### Console Log View

```
┌────────────────────────────────────────────────────────────┐
│ Console Logs                                               │
├────────────────────────────────────────────────────────────┤
│ Resource: [All Resources ▼]  🔍 Search  ⚙️ Settings       │
├────────────────────────────────────────────────────────────┤
│ 12:30:45.123  frontend    │ info: Microsoft.Hosting...   │
│ 12:30:45.456  catalogapi  │ info: Starting API...        │
│ 12:30:45.789  postgres    │ LOG:  database system ready  │
│ 12:30:46.012  redis       │ Ready to accept connections  │
│ 12:30:46.345  catalogapi  │ info: Application started    │
│ 12:30:47.678  frontend    │ info: Now listening on...    │
└────────────────────────────────────────────────────────────┘
```

### Features

- **Multi-resource view**: See logs from all resources in one view
- **Resource filtering**: Filter to specific resources
- **Search**: Full-text search across all logs
- **Auto-scroll**: Automatically scroll to new logs
- **Timestamps**: Precise timing information
- **Color coding**: Different colors for log levels

### Use Cases

**Startup debugging**:
```
Monitor console logs to see:
- When services start
- Initialization messages
- Configuration loading
- Dependency connections
- Error messages during startup
```

**Container output**:
```
See output from containers:
- PostgreSQL initialization
- Redis startup
- RabbitMQ broker status
- Container health checks
```

## Structured Logs

Structured logs provide queryable, filterable log entries with rich metadata.

### Structured Log View

```
┌─────────────────────────────────────────────────────────────────┐
│ Structured Logs                                                 │
├─────────────────────────────────────────────────────────────────┤
│ Filters: [Level ▼] [Resource ▼] [Category ▼]  🔍 Search        │
├──────────┬────────────┬──────────────┬─────────────────────────┤
│ Time     │ Level      │ Resource     │ Message                 │
├──────────┼────────────┼──────────────┼─────────────────────────┤
│ 12:30:45 │ Information│ catalogapi   │ Executing endpoint...   │
│ 12:30:45 │ Information│ catalogapi   │ Entity Framework...     │
│ 12:30:46 │ Warning    │ orderapi     │ Retry attempt 1 of 3    │
│ 12:30:46 │ Error      │ frontend     │ HTTP request failed     │
│ 12:30:47 │ Information│ catalogapi   │ Executed endpoint       │
└──────────┴────────────┴──────────────┴─────────────────────────┘
```

### Log Levels

```
Trace       ⚪ - Very detailed information
Debug       🔵 - Debugging information
Information ✅ - General information
Warning     ⚠️  - Warning messages
Error       ❌ - Error messages
Critical    💥 - Critical failures
```

### Log Details

Click on a log entry to see full details:

```json
{
  "Timestamp": "2024-01-23T12:30:45.123Z",
  "Level": "Information",
  "Category": "Microsoft.AspNetCore.Hosting.Diagnostics",
  "Message": "Request starting HTTP/1.1 GET http://localhost:5000/api/products",
  "TraceId": "00-abc123-def456-01",
  "SpanId": "abc123def456",
  "Properties": {
    "Protocol": "HTTP/1.1",
    "Method": "GET",
    "Path": "/api/products",
    "Scheme": "http",
    "Host": "localhost:5000"
  }
}
```

### Filtering and Search

**Filter by level**:
```
Select log level from dropdown
- Shows only logs at that level and above
- Example: "Warning" shows Warning, Error, Critical
```

**Filter by resource**:
```
Select specific service or container
- View logs from single resource
- Compare multiple resources
```

**Search**:
```
Search across:
- Message text
- Properties
- Categories
- Trace IDs
```

### Correlation with Traces

Logs include trace and span IDs:

```
Log Entry:
├─ TraceId: 00-abc123-def456-01
└─ SpanId: abc123def456

Click "View Trace" button → Jump to trace view
```

## Distributed Tracing

Distributed tracing shows the flow of requests across services.

### Traces View

```
┌─────────────────────────────────────────────────────────────────┐
│ Traces                                                          │
├─────────────────────────────────────────────────────────────────┤
│ Filters: [Service ▼] [Duration ▼] [Status ▼]  🔍 Search        │
├────────────┬─────────────┬──────────┬──────────────────────────┤
│ Start Time │ Service     │ Duration │ Spans                    │
├────────────┼─────────────┼──────────┼──────────────────────────┤
│ 12:30:45   │ frontend    │ 145ms    │ ████████░░░ 5 spans      │
│ 12:30:46   │ frontend    │ 89ms     │ █████░░░░░░ 4 spans      │
│ 12:30:47   │ catalogapi  │ 234ms    │ ███████████ 8 spans      │
└────────────┴─────────────┴──────────┴──────────────────────────┘
```

### Trace Details

Click on a trace to see the waterfall view:

```
Trace: GET /products - 145ms total
├─ [frontend] HTTP GET /products (145ms)
│  ├─ [frontend] HTTP GET http://catalogapi/api/products (120ms)
│  │  ├─ [catalogapi] Executing endpoint (118ms)
│  │  │  ├─ [catalogapi] EF Query: SELECT * FROM products (80ms)
│  │  │  │  └─ [postgres] Query execution (78ms)
│  │  │  └─ [catalogapi] Serialize response (8ms)
│  │  └─ [catalogapi] Executed endpoint
│  └─ [frontend] Render view (20ms)
└─ [frontend] Response completed

Timeline:
0ms        50ms       100ms      150ms
├──────────┼──────────┼──────────┤
│ frontend                       │
│   ├─ HTTP call                 │
│   │  ├─ catalogapi             │
│   │  │  ├─ EF Query            │
│   │  │  │  └─ postgres         │
│   │  │  └─ Serialize           │
│   │  └─                        │
│   └─ Render                    │
└─                               │
```

### Span Details

Click on a span to see metadata:

```json
{
  "Name": "HTTP GET",
  "Kind": "Client",
  "StartTime": "2024-01-23T12:30:45.123Z",
  "Duration": "120ms",
  "Status": "Ok",
  "Attributes": {
    "http.method": "GET",
    "http.url": "http://catalogapi/api/products",
    "http.status_code": 200,
    "http.response.size": 1024,
    "net.peer.name": "catalogapi",
    "net.peer.port": 5000
  },
  "Events": [],
  "Links": []
}
```

### Trace Filtering

**By service**:
```
Filter to traces involving specific services
- Frontend only
- Backend API only
- Multiple services
```

**By duration**:
```
Find slow requests:
- > 500ms
- > 1s
- > 5s
```

**By status**:
```
- Successful (200-299)
- Client errors (400-499)
- Server errors (500-599)
- Errors (any error status)
```

### Analyzing Performance

**Identify bottlenecks**:
```
Look for:
- Long-running spans
- Database queries taking too long
- Network latency between services
- Serialization overhead
```

**Example analysis**:
```
Request took 234ms total:
├─ Database query: 180ms (77%) ← BOTTLENECK
├─ HTTP call: 30ms (13%)
├─ Serialization: 15ms (6%)
└─ Other: 9ms (4%)

Action: Optimize database query or add caching
```

## Metrics and Performance

Metrics provide quantitative measurements of application behavior.

### Metrics View

```
┌─────────────────────────────────────────────────────────────────┐
│ Metrics                                                         │
├─────────────────────────────────────────────────────────────────┤
│ Resource: [catalogapi ▼]  Time Range: [Last 5 min ▼]          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  HTTP Request Rate                                              │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │     ▄▄                                                   │   │
│  │    ▀  ▀▄  ▄▀▀▄                                          │   │
│  │          ▀     ▀▄▄                                       │   │
│  │                   ▀▀▄▄                                   │   │
│  └─────────────────────────────────────────────────────────┘   │
│  15 req/sec                                                     │
│                                                                 │
│  Request Duration (p50, p90, p99)                               │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │  p99  ═══════════════════════════════════╗              │   │
│  │  p90  ════════════════════════╗          ║              │   │
│  │  p50  ══════════╗             ║          ║              │   │
│  └─────────────────────────────────────────────────────────┘   │
│  p50: 45ms, p90: 120ms, p99: 450ms                             │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Available Metrics

**HTTP metrics**:
```
- Request rate (requests/sec)
- Request duration (ms)
- Status code distribution
- Active requests
- Request size
- Response size
```

**Database metrics** (via EF Core):
```
- Query count
- Query duration
- Connection pool usage
- Connections opened/closed
- Command timeout errors
```

**Resource metrics**:
```
- CPU usage
- Memory usage
- Thread count
- GC collections
- Exception count
```

### Metric Details

Click on a metric to see:

```
┌──────────────────────────────────────────┐
│ HTTP Request Duration                    │
├──────────────────────────────────────────┤
│ Description:                             │
│   Measures the duration of HTTP requests │
│                                          │
│ Unit: milliseconds                       │
│                                          │
│ Tags:                                    │
│   • http.method: GET, POST, PUT, DELETE  │
│   • http.status_code: 200, 404, 500      │
│   • http.route: /api/products, /api/...  │
│                                          │
│ Percentiles:                             │
│   p50: 45ms                              │
│   p90: 120ms                             │
│   p95: 210ms                             │
│   p99: 450ms                             │
└──────────────────────────────────────────┘
```

### Custom Metrics

You can emit custom metrics from your code:

```csharp
// Creating a custom metric
public class OrderService
{
    private readonly Meter _meter;
    private readonly Counter<long> _ordersCreated;
    private readonly Histogram<double> _orderValue;
    
    public OrderService(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create("MyApp.Orders");
        
        _ordersCreated = _meter.CreateCounter<long>(
            "orders.created",
            description: "Number of orders created");
        
        _orderValue = _meter.CreateHistogram<double>(
            "order.value",
            unit: "USD",
            description: "Order value in USD");
    }
    
    public async Task<Order> CreateOrderAsync(Order order)
    {
        // Business logic
        var created = await SaveOrderAsync(order);
        
        // Record metrics
        _ordersCreated.Add(1, 
            new KeyValuePair<string, object?>("status", "success"));
        _orderValue.Record(order.TotalValue, 
            new KeyValuePair<string, object?>("currency", "USD"));
        
        return created;
    }
}
```

These custom metrics appear in the dashboard automatically.

## Environment Variables

The Environment view shows configuration injected into each service.

### Environment View

```
┌─────────────────────────────────────────────────────────────────┐
│ Environment Variables - catalogapi                              │
├─────────────────────────────────────────────────────────────────┤
│ Name                              │ Value                       │
├───────────────────────────────────┼─────────────────────────────┤
│ ASPNETCORE_ENVIRONMENT            │ Development                 │
│ ASPNETCORE_URLS                   │ http://+:5000               │
│ ConnectionStrings__catalogdb      │ Host=localhost;Port=5432... │
│ OTEL_EXPORTER_OTLP_ENDPOINT       │ http://localhost:18889      │
│ OTEL_SERVICE_NAME                 │ catalogapi                  │
│ services__redis__http__0          │ localhost:6379              │
└───────────────────────────────────┴─────────────────────────────┘
```

### Configuration Sources

Variables come from:
1. AppHost injection (service discovery, connection strings)
2. Service's appsettings.json
3. Environment-specific appsettings
4. User secrets
5. System environment variables

### Sensitive Values

```
Secrets are masked:
├─ ConnectionStrings__db   │ Host=localhost;Password=*** ✅
├─ ApiKeys__OpenAI         │ sk-proj-***                 ✅
└─ JWT__Secret             │ ***                         ✅
```

### Debugging Configuration

Use this view to:
- Verify connection strings are correct
- Check service discovery endpoints
- Confirm environment is Development/Production
- Validate OTEL configuration
- Troubleshoot configuration issues

## Debugging Distributed Applications

### Debugging Workflow

1. **Start AppHost with debugging**:
   ```bash
   # In Visual Studio: F5
   # In VS Code: Run and Debug → "AppHost"
   # CLI: dotnet run (in AppHost project)
   ```

2. **Attach to specific service**:
   ```
   Visual Studio:
   - Debug → Attach to Process
   - Find service process (e.g., CatalogApi.exe)
   - Click "Attach"
   
   VS Code:
   - Run and Debug → "Attach to Process"
   - Select service from list
   ```

3. **Set breakpoints** in service code

4. **Trigger request** (via Dashboard, Postman, or browser)

5. **Step through code** while monitoring Dashboard

### Multi-Service Debugging

```
Scenario: Debug frontend → API → database call

1. Set breakpoint in frontend (HTTP call)
2. Set breakpoint in API (endpoint handler)
3. Set breakpoint in API (database query)
4. Make request from browser
5. Step through each breakpoint
6. Monitor trace in Dashboard
```

### Using Traces for Debugging

```
Problem: Request is slow (500ms)

1. Find trace in Dashboard
2. Identify slow span (database query: 450ms)
3. Click span → see SQL query
4. Set breakpoint before query
5. Reproduce request
6. Examine query parameters
7. Optimize query
```

### Log-Based Debugging

```csharp
// Add detailed logging
_logger.LogInformation("Fetching products with filter: {Filter}", filter);

var products = await _context.Products
    .Where(p => p.Category == filter)
    .ToListAsync();

_logger.LogInformation("Found {Count} products", products.Count);

// View logs in Dashboard → Structured Logs
// Filter by category: "MyApp.Services.CatalogService"
```

### Exception Tracking

```
Exceptions appear in:
1. Console Logs (stack trace)
2. Structured Logs (with metadata)
3. Traces (span marked as error)
4. Metrics (exception counter increments)

Dashboard links all four views for full context
```

## Dashboard Configuration

### Customizing Dashboard Settings

Configure in AppHost's appsettings.json:

```json
{
  "Dashboard": {
    "Otlp": {
      "Endpoint": "http://localhost:18889",
      "AuthenticationMode": "BearerToken"
    },
    "Frontend": {
      "Endpoint": "https://localhost:15001",
      "AuthenticationMode": "BearerToken"
    },
    "ResourceServiceClient": {
      "AuthenticationMode": "BearerToken"
    }
  }
}
```

### Authentication Modes

```csharp
// BearerToken (default) - generates random token
"AuthenticationMode": "BearerToken"

// Unsecured - no authentication (development only)
"AuthenticationMode": "Unsecured"

// OpenIdConnect - use OIDC provider
"AuthenticationMode": "OpenIdConnect"
```

### Custom Ports

```json
{
  "Dashboard": {
    "Frontend": {
      "Endpoint": "https://localhost:18888"
    },
    "Otlp": {
      "Endpoint": "http://localhost:19999"
    }
  }
}
```

### Standalone Dashboard

You can run the dashboard without an AppHost:

```bash
# Install dashboard as global tool
dotnet tool install -g Aspire.Dashboard

# Run standalone
aspire-dashboard

# With OTLP endpoint
aspire-dashboard --urls "https://localhost:15001" --otlp-endpoint "http://localhost:18889"
```

Useful for:
- Monitoring production (with OTLP endpoint exposed)
- Viewing exported OTLP data
- Standalone telemetry viewer

## Tips and Tricks

### 1. Keyboard Shortcuts

```
Ctrl+K → Focus search
Ctrl+R → Refresh view
Ctrl+/ → Show help
/ → Quick filter
Esc → Clear filters
```

### 2. Trace ID in Logs

Copy trace ID from logs and search in traces:

```
Log entry: TraceId: 00-abc123def456-01
→ Copy trace ID
→ Go to Traces
→ Paste in search box
→ View full request flow
```

### 3. Filtering by Time Range

```
Use relative time ranges:
- Last 1 minute
- Last 5 minutes
- Last 15 minutes
- Last hour
- Custom range
```

### 4. Compare Metrics Across Services

```
Open multiple browser tabs:
Tab 1: frontend metrics
Tab 2: catalogapi metrics
Tab 3: orderapi metrics

Compare request rates, durations, error rates
```

### 5. Export Traces

```
Click trace → Export → Download JSON
Useful for:
- Sharing with team
- Filing bug reports
- Performance analysis
```

### 6. Live Metrics During Load Testing

```
Run load test in separate terminal:
$ k6 run loadtest.js

Watch live metrics in Dashboard:
- Request rate increasing
- Duration distribution
- Error rate
- Resource usage
```

### 7. Console Log Timestamps

```
Enable detailed timestamps:
Dashboard → Settings → Console Logs → Show Milliseconds
```

### 8. Create Saved Filters

```
Bookmark URLs with filters:
https://localhost:15001/structuredlogs?level=Warning&resource=catalogapi
```

### 9. Resource Health at a Glance

```
Dashboard home shows:
✅ All services healthy → Green
⚠️ Some warnings → Yellow
❌ Errors present → Red
```

### 10. Dark Mode

```
Dashboard → Settings → Theme → Dark
Easier on the eyes during long debugging sessions
```

## Summary

The Aspire Dashboard provides:

✅ **Unified observability** across all services and resources  
✅ **Real-time monitoring** with live updates  
✅ **Distributed tracing** with waterfall visualization  
✅ **Structured logging** with powerful filtering  
✅ **Metrics visualization** for performance monitoring  
✅ **Zero configuration** - works out of the box  

Key takeaways:
- Dashboard is your primary tool for understanding distributed apps
- Logs, traces, and metrics are correlated for easy debugging
- Resources view shows health and status of all components
- Environment view helps troubleshoot configuration issues
- Works in development and can be used with production telemetry

In the next lesson, we'll explore deploying Aspire applications to production environments.
