# Built-in OpenAPI & Scalar

## What is OpenAPI?

**OpenAPI** (formerly known as Swagger) is a specification for describing HTTP APIs in a machine-readable format — JSON or YAML. Think of it as a **contract** that tells both humans and tools exactly what your API can do: which endpoints exist, what parameters they accept, and what responses they return.

Why does this matter?

- **Frontend teams** can build against your API before the backend is finished
- **Code generators** can produce client SDKs in any language
- **Documentation UIs** like Scalar render beautiful, interactive docs automatically

In the .NET ecosystem, API documentation has undergone a major shift. For years, the community relied on **Swashbuckle** — a third-party NuGet package. Starting with **.NET 9**, Microsoft built OpenAPI support directly into ASP.NET Core. In **.NET 10**, this built-in support has matured further with YAML output, improved transformers, and OpenAPI 3.1 compliance.

---

## History: Swashbuckle → Built-in OpenAPI

For nearly a decade, **Swashbuckle.AspNetCore** was the de-facto standard for generating OpenAPI documents in ASP.NET Core. It was community-maintained, bundled **Swagger UI**, and required `AddSwaggerGen()` / `UseSwagger()` configuration. It served the community well, but lagged behind new ASP.NET Core features and was effectively unmaintained by late 2023.

### Why Microsoft Built It In

Starting with .NET 9, the ASP.NET Core team integrated OpenAPI document generation directly into the framework:

- **Maintenance risk** — Swashbuckle's maintenance was inconsistent
- **First-class integration** — Built-in support leverages internal framework APIs
- **OpenAPI 3.1** — Latest spec version from day one
- **Fewer dependencies** — No third-party packages needed

### Comparison

| Feature                  | Swashbuckle            | Built-in OpenAPI (.NET 9+)                 |
| ------------------------ | ---------------------- | ------------------------------------------ |
| Installation             | NuGet package required | Included in framework                      |
| OpenAPI version          | 3.0                    | 3.1                                        |
| Maintenance              | Community (inactive)   | Microsoft                                  |
| UI included              | Swagger UI bundled     | No UI (use Scalar, etc.)                   |
| Customization            | Filters, attributes    | Transformers (document, operation, schema) |
| YAML support             | Via third-party        | Native (.NET 10)                           |
| Minimal API support      | Partial                | Full                                       |
| TypedResults integration | Limited                | Automatic                                  |

💡 **Key takeaway**: For new .NET 10 projects, always use the built-in OpenAPI support. Swashbuckle is only relevant for legacy projects.

---

## Setup — Just Two Lines

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();       // 1. Register OpenAPI services

var app = builder.Build();
app.MapOpenApi();                    // 2. Serve the OpenAPI document

app.MapGet("/api/events", () => Results.Ok(new[] { "TechConf 2026" }));
app.Run();
```

### What Each Line Does

**`builder.Services.AddOpenApi()`** registers the OpenAPI document generation services into the DI container — schema generators, endpoint metadata readers, and document assemblers.

**`app.MapOpenApi()`** adds an endpoint that serves the generated OpenAPI document at:

```
GET /openapi/v1.json
```

Navigate to `https://localhost:5001/openapi/v1.json` to see the complete API description.

### Customizing the Document Path

You can change the document name or serve multiple versions:

```csharp
builder.Services.AddOpenApi("techconf");  // → /openapi/techconf.json
```

⚠️ **Important**: The document is generated at runtime. If you add or change endpoints, restart the app (or rely on hot reload) to see updates.

---

## OpenAPI 3.1 Features

The built-in implementation generates **OpenAPI 3.1** documents — a significant upgrade over Swashbuckle's 3.0 output.

**Full JSON Schema alignment** — OpenAPI 3.1 uses standard JSON Schema (draft 2020-12):

```json
// OpenAPI 3.0 (Swashbuckle) — custom nullable handling
{ "type": "string", "nullable": true }

// OpenAPI 3.1 (Built-in) — standard JSON Schema
{ "type": ["string", "null"] }
```

**Improved nullable handling** — C# nullable reference types (`string?`, `Event?`) are correctly represented using JSON Schema's type arrays.

**Webhooks support** — OpenAPI 3.1 adds a `webhooks` section for documenting callback URLs. Not commonly used in course projects, but available for advanced scenarios.

**YAML output** — .NET 10 adds native YAML support for OpenAPI documents. The JSON endpoint remains at `/openapi/v1.json`.

**Tooling compatibility** — OpenAPI 3.1 works seamlessly with Scalar, Kiota (Microsoft's API client generator), OpenAPI Generator, and Spectral (API linting).

---

## Document Customization with Transformers

The default OpenAPI document works, but it's generic. **Transformers** let you customize every aspect of the generated document.

### Document Transformers

A document transformer modifies the entire OpenAPI document — perfect for adding metadata:

```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info = new()
        {
            Title = "TechConf API",
            Version = "v1",
            Description = "Event management platform for tech conferences",
            Contact = new() { Name = "TechConf Team", Email = "api@techconf.dev" },
            License = new() { Name = "MIT" }
        };
        return Task.CompletedTask;
    });
});
```

### Operation Transformers

An operation transformer modifies individual endpoint descriptions:

```csharp
options.AddOperationTransformer((operation, context, ct) =>
{
    operation.Responses.TryAdd("500", new() { Description = "Internal server error" });
    return Task.CompletedTask;
});
```

### Schema Transformers

A schema transformer customizes how C# types appear in the schema:

```csharp
options.AddSchemaTransformer((schema, context, ct) =>
{
    if (context.JsonTypeInfo.Type == typeof(DateTime))
        schema.Example = new OpenApiString("2026-11-10T09:00:00Z");
    return Task.CompletedTask;
});
```

### Adding JWT Security Scheme

You'll use this on Day 3 when we add authentication:

```csharp
options.AddDocumentTransformer((document, context, ct) =>
{
    document.Components ??= new();
    document.Components.SecuritySchemes.Add("Bearer", new()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token"
    });
    return Task.CompletedTask;
});
```

💡 **Tip**: You can chain multiple transformers — they execute in registration order.

---

## Endpoint Metadata

While transformers customize the document globally, **endpoint metadata** describes individual endpoints. This is critical for useful documentation.

### Essential Metadata Methods

```csharp
app.MapGet("/api/events", GetAllEvents)
    .WithName("GetAllEvents")          // Unique operation ID
    .WithTags("Events")                // Groups endpoints in the UI
    .WithSummary("List all events")    // Short description
    .WithDescription("Returns all TechConf events, ordered by start date. " +
                      "Supports filtering by city and date range.");
```

### Documenting Responses

By default, only the 200 response is documented. Use `.Produces<T>()` to declare all possible responses:

```csharp
app.MapGet("/api/events/{id:int}", GetEventById)
    .Produces<Event>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .WithSummary("Get event by ID");

app.MapPost("/api/events", CreateEvent)
    .Produces<Event>(StatusCodes.Status201Created)
    .ProducesValidationProblem()
    .WithSummary("Create a new event");
```

### TypedResults — Automatic Metadata

With `TypedResults` and union return types, OpenAPI metadata is generated **automatically**:

```csharp
app.MapGet("/api/events/{id:int}", Results<Ok<Event>, NotFound> (int id) =>
{
    var evt = events.FirstOrDefault(e => e.Id == id);
    return evt is not null ? TypedResults.Ok(evt) : TypedResults.NotFound();
});
// Automatically documents 200 (Event) and 404 — no .Produces() needed!
```

💡 **Best practice**: Prefer `TypedResults` over `Results` — it gives you compile-time safety _and_ automatic OpenAPI generation.

### Route Group Metadata

Apply metadata to all endpoints in a group:

```csharp
var events = app.MapGroup("/api/events")
    .WithTags("Events")
    .WithOpenApi();

events.MapGet("/", GetAllEvents).WithSummary("List all events");
events.MapGet("/{id:int}", GetEventById).WithSummary("Get event by ID");
events.MapPost("/", CreateEvent).WithSummary("Create a new event");
events.MapPut("/{id:int}", UpdateEvent).WithSummary("Update an event");
events.MapDelete("/{id:int}", DeleteEvent).WithSummary("Delete an event");
```

### Hiding Endpoints

```csharp
app.MapGet("/internal/health", () => Results.Ok("Healthy"))
    .ExcludeFromDescription();
```

---

## Scalar — Modern API Documentation UI

The built-in OpenAPI support generates a JSON document, but reading raw JSON isn't practical. **Scalar** is an open-source documentation UI that renders your OpenAPI document as an interactive web page.

### Why Scalar over Swagger UI?

Scalar offers a significantly better developer experience — clean, responsive design, built-in API client, and code generation.

### Installation

```bash
dotnet add package Scalar.AspNetCore
```

### Setup

```csharp
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();  // ← Adds the Scalar UI

app.Run();
```

Navigate to `https://localhost:5001/scalar/v1` to see your API documentation.

### Feature Tour

| Feature             | Description                                         |
| ------------------- | --------------------------------------------------- |
| **Responsive UI**   | Clean, modern layout — desktop and mobile           |
| **Dark mode**       | Light and dark themes                               |
| **API client**      | Send requests from the browser — no Postman needed  |
| **Code generation** | Example code in curl, Python, JavaScript, C#, etc.  |
| **Authentication**  | API keys or JWT tokens that persist across requests |
| **Search**          | Full-text search across endpoints and descriptions  |

### Configuration

```csharp
app.MapScalarApiReference(options =>
{
    options.WithTitle("TechConf API Documentation");
    options.WithTheme(ScalarTheme.Mars);
    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    options.WithPreferredScheme("Bearer");
});
```

### The Scalar UI Layout

When you open Scalar, you'll see:

- **Left sidebar** — Navigation tree with all endpoints grouped by tags
- **Center panel** — Endpoint details: parameters, request body schema, response examples
- **Right panel** — Interactive API client with code snippets in multiple languages
- **Top bar** — Search, theme toggle, and authentication settings

💡 **Tip**: Documentation quality in Scalar is directly proportional to the metadata you add. Always use `.WithTags()`, `.WithSummary()`, and response types.

---

## .http Files — Testing Without Leaving Your IDE

**.http files** let you test APIs directly in your IDE — no external tools needed. They're supported in **Visual Studio 2022** (17.8+, built-in), **VS Code** (REST Client extension), and **JetBrains Rider** (built-in).

### Variables

Define reusable variables at the top of your file. Use `{{variableName}}` syntax to reference them in requests.

### Complete Example — TechConf API

Create a file named `TechConf.http` in your project root:

```http
@baseUrl = https://localhost:5001
@eventId = 1

### Get all events
GET {{baseUrl}}/api/events
Accept: application/json

### Get event by ID
GET {{baseUrl}}/api/events/{{eventId}}
Accept: application/json

### Create event
POST {{baseUrl}}/api/events
Content-Type: application/json

{
    "title": "dotnet conf 2026",
    "description": "Annual .NET conference",
    "startDate": "2026-11-10T09:00:00Z",
    "endDate": "2026-11-12T17:00:00Z",
    "location": "Online",
    "maxAttendees": 50000
}

### Update event
PUT {{baseUrl}}/api/events/{{eventId}}
Content-Type: application/json

{
    "title": "dotnet conf 2026 - Updated"
}

### Delete event
DELETE {{baseUrl}}/api/events/{{eventId}}
```

Each request is separated by `###`. The text after `###` serves as a label for IDE navigation.

### Response Variables (Chaining Requests)

You can capture response values and use them in subsequent requests:

```http
# @name createEvent
POST {{baseUrl}}/api/events
Content-Type: application/json

{ "title": "Workshop: Minimal APIs", "location": "Munich" }

### Use the created event's ID
GET {{baseUrl}}/api/events/{{createEvent.response.body.$.id}}
```

### Comparison with Other Tools

| Feature         | .http files   | Postman         | Insomnia        |
| --------------- | ------------- | --------------- | --------------- |
| Cost            | Free          | Freemium        | Freemium        |
| Version control | ✅ Plain text | ❌ Proprietary  | ❌ Proprietary  |
| IDE integration | ✅ Native     | ❌ Separate app | ❌ Separate app |
| Team sharing    | ✅ Git commit | 💰 Paid         | 💰 Paid         |

💡 **Recommendation**: Use `.http` files for day-to-day testing — they're free, version-controllable, and built into your IDE. Use Scalar for exploring and sharing documentation.

---

## Alternative: Swagger UI

While we recommend Scalar, other options exist.

**Swashbuckle.AspNetCore** still works with .NET 10 for legacy projects:

```bash
dotnet add package Swashbuckle.AspNetCore
```

**NSwag** is another option that can generate both OpenAPI documents and C#/TypeScript client code — useful when client generation is part of your build pipeline.

For new projects in 2026, built-in OpenAPI + Scalar is the recommended approach.

---

## Common Pitfalls

⚠️ **Forgetting `.WithTags()`** — Without tags, all endpoints appear ungrouped in Scalar.

⚠️ **Not adding `.Produces<T>()` for non-200 responses** — Scalar only shows 200 unless you declare other responses. Use `TypedResults` to avoid this.

⚠️ **OpenAPI document not updating** — Restart the app, clear browser cache, or try incognito mode.

⚠️ **Scalar not loading** — Check NuGet package version matches your .NET version. Verify both `app.MapOpenApi()` and `app.MapScalarApiReference()` are present.

💡 **Use TypedResults** — Automatic OpenAPI metadata with compile-time safety. The single most impactful improvement for documentation quality.

---

## 🧪 Mini-Exercise

1. **Add Scalar** — Install `Scalar.AspNetCore`, add `app.MapScalarApiReference()`, open the UI
2. **Customize the OpenAPI document** — Set title to "TechConf Events API", add description via document transformer
3. **Add metadata** — `.WithTags("Events")` on your route group, `.WithSummary()` on each endpoint, declare non-200 responses
4. **Create a `.http` file** — Add requests for all five CRUD operations, test against your running API
5. **Compare** — Open Scalar before and after adding metadata to see the improvement

---

## 📚 Further Reading

- [ASP.NET Core OpenAPI Documentation](https://learn.microsoft.com/aspnet/core/fundamentals/openapi/overview) — Microsoft's official guide
- [OpenAPI 3.1 Specification](https://spec.openapis.org/oas/v3.1.0) — The full specification
- [Scalar Documentation](https://github.com/scalar/scalar) — GitHub repository with examples
- [.http File Syntax Reference](https://learn.microsoft.com/aspnet/core/test/http-files) — Visual Studio docs
- [OpenAPI Transformers in .NET](https://learn.microsoft.com/aspnet/core/fundamentals/openapi/customize-openapi) — Transformer deep dive

---

_Next up: [HTTP Fundamentals Recap](04-http-fundamentals.md) — REST principles, status codes, and content negotiation_
