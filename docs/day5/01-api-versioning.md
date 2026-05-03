# API Versioning — Evolving Without Breaking

## Why Versioning Matters

Once your API is public, you no longer control who consumes it.
Mobile apps, third-party integrations, partner systems — they all depend on the **exact shape** of your responses and the **exact behavior** of your endpoints.

A seemingly harmless change like renaming a JSON field can **break production systems** you've never heard of.

**The cost of breaking changes:**

- 📱 Mobile apps in the App Store can't be force-updated overnight
- 🤝 Partner integrations require coordination and development cycles
- 💰 Broken consumers mean lost revenue, broken trust, and support tickets
- ⏰ Rollbacks are expensive — consumers may have already adapted

API versioning gives you a **contract**: consumers choose when to migrate, and you can evolve your API **without breaking existing clients**.

> **TechConf scenario:** Your Events API is consumed by the conference website, a mobile app, and three sponsor integrations. You need to add venue details and registration counts — but you can't change the response shape without warning everyone first.

---

## Breaking vs Non-Breaking Changes

Before versioning, you need to understand **what actually requires a new version**.
The golden rule: **additive changes are safe, subtractive or mutating changes are not.**

| Change | Breaking? | Example (TechConf) |
|--------|-----------|---------------------|
| Adding optional field | ❌ No | Add `venue` to Event response |
| Removing field | ✅ Yes | Remove `location` from response |
| Renaming field | ✅ Yes | `location` → `venue` |
| Changing field type | ✅ Yes | `maxAttendees` string → int |
| Adding required field | ✅ Yes | New required `category` on POST |
| Adding new endpoint | ❌ No | `GET /api/events/featured` |
| Removing endpoint | ✅ Yes | Remove `DELETE /api/events` |
| Changing status code | ✅ Yes | 200 → 201 for POST |
| Adding optional query param | ❌ No | `?includeArchived=true` |
| Changing error format | ✅ Yes | Custom errors → Problem Details |
| Tightening validation | ✅ Yes | Title max length 200 → 100 |
| Loosening validation | ❌ No | Title max length 100 → 200 |

**Key insight:** If a well-behaved existing client could break or behave differently after your change, it's breaking.

```
✅ Safe: GET /api/events returns { "id": "...", "title": "..." }
         → now returns  { "id": "...", "title": "...", "venue": "..." }
         Clients just ignore the new field.

❌ Breaking: GET /api/events returns { "id": "...", "location": "..." }
             → now returns  { "id": "...", "venue": "..." }
             Clients reading "location" get null/undefined.
```

> 💡 **Rule of thumb:** You can always **add** to your API. You can never **remove** or **rename** without a version bump.

---

## Versioning Strategies Compared

There are four main strategies for communicating the API version. Each has trade-offs:

| Strategy | Request Example | Pros | Cons |
|----------|----------------|------|------|
| **URL Path** | `GET /api/v1/events` | Visible, cacheable, simple to test | URLs change between versions |
| **Query String** | `GET /api/events?api-version=1.0` | URL path stays stable | Easy to forget, clutters URLs |
| **Header** | `X-Api-Version: 1.0` | Clean URLs | Not visible in browser/logs |
| **Media Type** | `Accept: application/json;v=1.0` | REST-purist, content negotiation | Complex, tooling support varies |

### URL Path Versioning (Recommended)

```
GET /api/v1/events          → V1 response shape
GET /api/v2/events          → V2 response shape
```

**Why URL Path wins for most APIs:**

- ✅ Immediately visible in logs, browser, documentation
- ✅ Easy to test with `curl` or Postman — no special headers needed
- ✅ HTTP caches differentiate versions automatically
- ✅ Swagger/OpenAPI can generate separate docs per version
- ✅ Developers understand it instantly

**When to consider alternatives:**

- Header versioning: internal APIs where URL stability matters
- Media type versioning: APIs that must follow strict REST/HATEOAS principles
- Query string: legacy systems where URL routing can't be changed

> 💡 **Our recommendation:** Use **URL Path versioning** for the TechConf API. It's the most discoverable and the easiest to document, test, and debug.

---

## Asp.Versioning Setup

Install the NuGet package:

```bash
dotnet add package Asp.Versioning.Http
```

### Service Configuration

```csharp
builder.Services.AddApiVersioning(options =>
{
    // Default version when client doesn't specify one
    options.DefaultApiVersion = new ApiVersion(1, 0);

    // Allow requests without an explicit version (uses DefaultApiVersion)
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Include api-supported-versions and api-deprecated-versions headers
    options.ReportApiVersions = true;

    // Accept version from URL segment AND custom header
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddApiExplorer(options =>
{
    // Format: v1, v1.0, v2, etc.
    options.GroupNameFormat = "'v'VVV";

    // Replace {version:apiVersion} in route templates
    options.SubstituteApiVersionInUrl = true;
});
```

### What Each Option Does

| Option | Purpose |
|--------|---------|
| `DefaultApiVersion` | The version used when the client doesn't specify one. Set to `1.0` so existing consumers aren't broken when you add versioning. |
| `AssumeDefaultVersionWhenUnspecified` | If `true`, requests without a version get the default. If `false`, they receive `400 Bad Request`. Start with `true` for backward compatibility. |
| `ReportApiVersions` | Adds response headers listing supported and deprecated versions. Consumers can use these to detect upcoming deprecations programmatically. |
| `ApiVersionReader` | Defines **where** the version is read from. `Combine` allows multiple readers — here, both URL segments and a custom header work. |
| `GroupNameFormat` | Controls how versions appear in OpenAPI group names. `'v'VVV` produces `v1`, `v1.1`, `v2`. |
| `SubstituteApiVersionInUrl` | Replaces `{version:apiVersion}` with actual values in generated docs, so Swagger shows `/api/v1/events` instead of `/api/v{version}/events`. |

---

## URL Path Versioning with Minimal APIs

### Defining Versioned Endpoint Groups

```csharp
// ── V1 Endpoints ──────────────────────────────────────────
var v1 = app.NewVersionedApi()
    .MapGroup("/api/v{version:apiVersion}/events")
    .HasApiVersion(1.0)
    .WithTags("Events");

v1.MapGet("/", GetEventsV1);
v1.MapGet("/{id:guid}", GetEventByIdV1);
v1.MapPost("/", CreateEventV1);

// ── V2 Endpoints ──────────────────────────────────────────
var v2 = app.NewVersionedApi()
    .MapGroup("/api/v{version:apiVersion}/events")
    .HasApiVersion(2.0)
    .WithTags("Events");

v2.MapGet("/", GetEventsV2);
v2.MapGet("/{id:guid}", GetEventByIdV2);
v2.MapPost("/", CreateEventV2);
```

### V1 vs V2 Response Shapes

The whole point of versioning: **different versions return different shapes**.

```csharp
// ── V1: Simple, flat response ─────────────────────────────
public record EventResponseV1(
    Guid Id,
    string Title,
    DateTime StartDate,
    string Location);

// ── V2: Richer, structured response ──────────────────────
public record EventResponseV2(
    Guid Id,
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    VenueInfo Venue,
    int RegisteredCount,
    EventStatus Status);

public record VenueInfo(string Name, string Address, int Capacity);
public enum EventStatus { Draft, Published, Cancelled, Completed }
```

### Handler Implementations

```csharp
static IResult GetEventsV1(TechConfDb db)
{
    var events = db.Events.Select(e => new EventResponseV1(
        e.Id, e.Title, e.StartDate, e.Location)).ToList();
    return Results.Ok(events);
}

static IResult GetEventsV2(TechConfDb db)
{
    var events = db.Events
        .Include(e => e.Venue)
        .Select(e => new EventResponseV2(
            e.Id, e.Title, e.Description,
            e.StartDate, e.EndDate,
            new VenueInfo(e.Venue.Name, e.Venue.Address, e.Venue.Capacity),
            e.Registrations.Count,
            e.Status))
        .ToList();
    return Results.Ok(events);
}
```

**V1 response:**
```json
{ "id": "...", "title": "Keynote", "startDate": "2026-03-15", "location": "Main Hall" }
```

**V2 response:**
```json
{
  "id": "...", "title": "Keynote", "description": "Opening keynote...",
  "startDate": "2026-03-15T09:00:00", "endDate": "2026-03-15T10:30:00",
  "venue": { "name": "Main Hall", "address": "123 Tech Blvd", "capacity": 500 },
  "registeredCount": 342, "status": "Published"
}
```

---

## Deprecation Workflow

Deprecation is a **process**, not a switch. You announce, warn, and then eventually remove.

### Marking a Version as Deprecated

```csharp
var versionSet = app.NewVersionedApi();

var v1 = versionSet
    .MapGroup("/api/v{version:apiVersion}/events")
    .HasDeprecatedApiVersion(1.0)
    .HasApiVersion(2.0)
    .WithTags("Events");
```

When `ReportApiVersions = true`, every response includes:

```
api-supported-versions: 2.0
api-deprecated-versions: 1.0
```

### Adding a Sunset Header

The `Sunset` header (RFC 8594) tells consumers **when** the version will stop working:

```csharp
v1.MapGet("/", GetEventsV1)
    .WithMetadata(new SunsetAttribute(2027, 1, 1));
```

Response:

```
Sunset: Sat, 01 Jan 2027 00:00:00 GMT
api-deprecated-versions: 1.0
```

### The Deprecation Timeline

| Phase | Action | Duration |
|-------|--------|----------|
| **1. Announce** | Blog post, changelog, email to known consumers | 3+ months before |
| **2. Deprecate** | Mark version deprecated, add `Sunset` header | At announcement |
| **3. Monitor** | Track V1 usage, reach out to remaining consumers | Ongoing |
| **4. Sunset** | Return `410 Gone` for V1 requests | On sunset date |
| **5. Remove** | Delete V1 code | After sunset |

> ⚠️ **Never skip the monitoring phase.** You may discover consumers you didn't know about. Give them time to migrate.

---

## Multiple OpenAPI Documents per Version

Each API version should have its own OpenAPI document so consumers see only the endpoints relevant to their version.

```csharp
// ── Register separate OpenAPI documents ───────────────────
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((doc, ctx, ct) =>
    {
        doc.Info.Title = "TechConf API";
        doc.Info.Version = "1.0 (deprecated)";
        doc.Info.Description = "⚠️ This version is deprecated. " +
            "Please migrate to v2 before January 2027.";
        return Task.CompletedTask;
    });
});

builder.Services.AddOpenApi("v2", options =>
{
    options.AddDocumentTransformer((doc, ctx, ct) =>
    {
        doc.Info.Title = "TechConf API";
        doc.Info.Version = "2.0";
        doc.Info.Description = "Current stable version of the TechConf API.";
        return Task.CompletedTask;
    });
});

// ── Map OpenAPI endpoint ──────────────────────────────────
app.MapOpenApi("/openapi/{documentName}.json");
```

This generates:

- `/openapi/v1.json` — V1 endpoints only (marked deprecated)
- `/openapi/v2.json` — V2 endpoints only

### Version Switcher in Scalar UI

```csharp
app.MapScalarApiReference(options =>
{
    options.WithTitle("TechConf API")
        .AddDocument("v1", "TechConf API v1 (deprecated)")
        .AddDocument("v2", "TechConf API v2");
});
```

Consumers can switch between versions in the UI dropdown — making migration easier since they can compare both versions side by side.

---

## Common Pitfalls

### ⚠️ Mistakes to Avoid

| Pitfall | Why It Hurts |
|---------|-------------|
| Not versioning from day one | Adding `v1` later forces all existing consumers to update their URLs or you must keep unversioned routes forever. |
| Too many active versions | Each version multiplies maintenance, testing, and documentation work. Three active versions means three times the bug fixes. |
| Forgetting `ReportApiVersions = true` | Consumers have no programmatic way to discover supported or deprecated versions. |
| Not documenting deprecation timeline | Consumers don't know when to migrate, leading to panic when you finally remove a version. |
| Versioning too aggressively | Not every change needs a new version. Additive changes are safe — only create a new version for breaking changes. |

### 💡 Best Practices

- **Start with `v1` from day one** — even if you think you'll never need `v2`
- **Keep maximum 2–3 active versions** — deprecate aggressively
- **Share internal models, version the DTOs** — your domain layer stays clean, only the API response shapes are versioned
- **Automate deprecation monitoring** — track which consumers still use deprecated versions
- **Test all active versions in CI** — a fix in shared code must not break older versions

---

## 🧪 Mini-Exercise

**Goal:** Add API versioning to the TechConf Events API.

> For a fuller guided exercise with `exercise/` and `solution/` scaffolds, see **[labs/lab-api-versioning](../../labs/lab-api-versioning/)**.

### Tasks

1. **Install** `Asp.Versioning.Http` and configure versioning with URL path strategy

2. **Create V1 endpoints** returning the simple `EventResponseV1` shape:
   ```
   GET /api/v1/events → [{ id, title, startDate, location }]
   ```

3. **Create V2 endpoints** returning the richer `EventResponseV2` shape:
   ```
   GET /api/v2/events → [{ id, title, description, startDate, endDate, venue, registeredCount, status }]
   ```

4. **Deprecate V1** using `HasDeprecatedApiVersion(1.0)` — verify the `api-deprecated-versions` header appears in responses

5. **Generate separate OpenAPI documents** for each version — verify `/openapi/v1.json` and `/openapi/v2.json` contain only their respective endpoints

### Verify with curl

```bash
curl -s http://localhost:5000/api/v1/events -D - | head -20
# Look for: api-deprecated-versions: 1.0

curl -s http://localhost:5000/api/v2/events | jq '.[0].venue'
# Should return venue object, not a string
```

---

## Further Reading

- 📖 [Asp.Versioning Documentation](https://github.com/dotnet/aspnet-api-versioning) — Official docs and wiki
- 📖 [API Versioning Best Practices](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design#versioning-a-restful-web-api) — Microsoft Architecture Center
- 📖 [RFC 8594 — The Sunset HTTP Header](https://www.rfc-editor.org/rfc/rfc8594) — Formal spec for sunset headers
- 📖 [Stripe's API Versioning](https://stripe.com/docs/api/versioning) — Real-world versioning done right
- 📖 [API Changelog Patterns](https://keepachangelog.com/) — How to communicate changes to consumers
