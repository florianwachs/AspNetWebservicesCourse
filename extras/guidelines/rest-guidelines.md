# REST Guidelines

- [REST Guidelines](#rest-guidelines)
  - [Origins of REST](#origins-of-rest)
  - [Core Components of REST](#core-components-of-rest)
    - [Resources (URIs)](#resources-uris)
    - [HTTP Verbs](#http-verbs)
    - [HTTP Status Codes](#http-status-codes)
    - [Stateless](#stateless)
  - [RESTful URIs and Actions](#restful-uris-and-actions)
    - [Relations / Nesting](#relations--nesting)
    - [Beyond CRUD](#beyond-crud)
  - [Error Responses — RFC 9457 Problem Details](#error-responses--rfc-9457-problem-details)
  - [API Documentation with OpenAPI](#api-documentation-with-openapi)
  - [API Versioning](#api-versioning)
  - [Filtering, Searching, Sorting](#filtering-searching-sorting)
  - [Returning the Resource After an Action](#returning-the-resource-after-an-action)
  - [JSON Property Serialization](#json-property-serialization)
  - [Pagination](#pagination)
  - [HTTP-Verb Override](#http-verb-override)
  - [Rate Limiting](#rate-limiting)
  - [Additional Considerations](#additional-considerations)
  - [Resources](#resources)

> The following document presents guidelines and best practices for REST APIs.
> It is important to note that REST is **not** a certified standard — these guidelines are based on
> industry experience. Some may not apply to your use case or may overcomplicate your API.
> As with everything, critically evaluate whether a guideline fits your context.
> They exist to make API development easier, not as rigid rules to follow in every situation.

## Origins of REST

**Representational State Transfer (REST)** was introduced by Roy Fielding as part of his doctoral dissertation (2000):
<https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm>

## Core Components of REST

### Resources (URIs)

- The **nouns** of your API
- Examples: `events`, `speakers`, `sessions`, `attendees`
- Always use **plural nouns** for consistency

### HTTP Verbs

- Define **what should happen** to a resource
- Standard verbs: `GET`, `POST`, `PUT`, `DELETE`, `PATCH`
- Custom verbs are technically possible but **not recommended**
- Reference: [RFC 9110 — HTTP Semantics](https://www.rfc-editor.org/rfc/rfc9110#section-9)

### HTTP Status Codes

The server's response indicating the result of the client's request.

| Range | Meaning          | Common Codes                                                                                                                                  |
| ----- | ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| `2xx` | **Success**      | `200 OK`, `201 Created`, `204 No Content`                                                                                                     |
| `3xx` | **Redirection**  | `301 Moved Permanently`, `304 Not Modified`                                                                                                   |
| `4xx` | **Client Error** | `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, `409 Conflict`, `422 Unprocessable Content`, `429 Too Many Requests` |
| `5xx` | **Server Error** | `500 Internal Server Error`, `502 Bad Gateway`, `503 Service Unavailable`                                                                     |

Reference: [RFC 9110 — Status Codes](https://www.rfc-editor.org/rfc/rfc9110#section-15)

### Stateless

REST APIs are **stateless** — they do not store any session state between requests. Every request must contain all the information needed (including authentication) to execute the action on the server.

## RESTful URIs and Actions

- Resources are **nouns**: events, speakers, sessions, attendees
- HTTP verbs are the **actions**:
  - `GET /events` — Returns a list of events
  - `GET /events/42` — Returns a specific event
  - `POST /events` — Creates a new event resource
  - `PUT /events/42` — Replaces the entire event resource
  - `PATCH /events/42` — Partially updates the event resource
  - `DELETE /events/42` — Deletes\* the event resource

> **GET** must never modify data or change the state of a resource (safe & idempotent).
> Using **plural nouns** has become the convention, leading to a single API endpoint per resource on which all actions can be performed.

### Relations / Nesting

- `GET /events/42/sessions` — Returns all sessions for event 42
- `GET /events/42/sessions/7` — Returns session 7 of event 42
- `POST /events/42/sessions` — Creates a new session for event 42
- `PUT /events/42/sessions/7` — Replaces session 7
- `PATCH /events/42/sessions/7` — Partially updates session 7
- `DELETE /events/42/sessions/7` — Deletes\* session 7

> \*"Deleting" in most applications means **soft-deleting** (deactivating) the record.

> **Limit nesting to 2 levels.** Instead of `events/1/sessions/123/speakers`, create a separate endpoint: `sessions/123/speakers`. This is a guideline, not a hard rule.

### Beyond CRUD

Some actions do not map cleanly to RESTful CRUD operations — for example, marking a session as a favorite.

> **Important:** Do not fall back to RPC-style endpoints.
> ❌ `PATCH /sessions/1/setasfavorite=true`

Alternatives:

1. Include `isFavorite` as a field in the resource and update it via `PATCH`
2. Model the action as a sub-resource:
   - `POST /sessions/1/favorites` — Add to favorites
   - `DELETE /sessions/1/favorites` — Remove from favorites

## Error Responses — RFC 9457 Problem Details

Use [RFC 9457 (Problem Details for HTTP APIs)](https://www.rfc-editor.org/rfc/rfc9457) to return structured, machine-readable error responses. ASP.NET Core has built-in support via `Results.Problem()` and `Results.ValidationProblem()`.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "detail": "Event with ID '42' was not found.",
  "instance": "/api/events/42"
}
```

In Minimal APIs:

```csharp
app.MapGet("/api/events/{id:guid}", async (Guid id, TechConfDbContext db) =>
{
    var ev = await db.Events.FindAsync(id);
    return ev is not null
        ? Results.Ok(ev)
        : Results.Problem(
            title: "Not Found",
            detail: $"Event with ID '{id}' was not found.",
            statusCode: 404);
});
```

For validation errors, return `422 Unprocessable Content` with validation details:

```csharp
app.MapPost("/api/events", async (CreateEventRequest request, IValidator<CreateEventRequest> validator, TechConfDbContext db) =>
{
    var result = validator.Validate(request);
    if (!result.IsValid)
    {
        return Results.ValidationProblem(result.ToDictionary());
    }
    // ... create event
});
```

## API Documentation with OpenAPI

- Every production API should be documented
- Use **OpenAPI (Swagger)** as the standard specification format
- .NET 10 has first-class OpenAPI support via `Microsoft.AspNetCore.OpenApi`

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();

app.MapGet("/api/events", async (TechConfDbContext db) =>
    await db.Events.ToListAsync())
    .WithName("GetEvents")
    .WithTags("Events")
    .WithDescription("Returns all events")
    .Produces<List<Event>>(200);
```

**What to document:**

- Available resources and their operations
- Expected request/response bodies
- Status codes and error formats
- Authentication requirements
- Changelog between versions
- Deprecation schedule for old API versions
- Example: [GitHub API Documentation](https://docs.github.com/en/rest)

## API Versioning

**Why version your API:**

- URIs should remain valid over a long period
- Consumers may not be able to react immediately to breaking changes
- Old versions can be phased out gradually

**Versioning strategies:**

| Strategy        | Example                                                        | Notes                           |
| --------------- | -------------------------------------------------------------- | ------------------------------- |
| URL path        | `/api/v2/events`                                               | Most common, easy to understand |
| Query parameter | `/api/events?api-version=2`                                    | Flexible, but less discoverable |
| HTTP header     | `api-version: 2` or `Accept: application/vnd.techconf.v2+json` | Clean URLs, but harder to test  |
| Media type      | `Content-Type: application/vnd.techconf.v2+json`               | RESTful purist approach         |

> All approaches are debated — the important thing is that you **version at all**.

**ASP.NET Core support:**

- [Asp.Versioning](https://github.com/dotnet/aspnet-api-versioning) — the official library for API versioning
- Supports URL segment, query string, header, and media type versioning
- Integrates with OpenAPI/Swagger

## Filtering, Searching, Sorting

**Filtering:**

```http
GET /events?status=published
```

**Sorting:**

```http
GET /events?sort=startDate
GET /events?sort=-startDate    # descending
```

**Searching:**

```http
GET /speakers?q=Martin
```

**Combining:**

```http
GET /events?q=DevConf&status=published&sort=startDate,title
```

> Do not over-engineer filtering capabilities. For complex queries, consider dedicated search technologies like Elasticsearch or OData.
> For frequently used queries, create **aliases**: `GET /events/upcoming`

## Returning the Resource After an Action

- HTTP verbs that create or modify resources should return the resource in the response.
- `POST /events` → Returns the newly created event with status `201 Created` and a `Location` header pointing to the new resource URI.
- `PUT` or `PATCH /events/42` → Returns the updated event with status `200 OK`.
- `DELETE /events/42` → Returns `204 No Content`.

## JSON Property Serialization

| Style          | Example      | Notes                               |
| -------------- | ------------ | ----------------------------------- |
| **camelCase**  | `firstName`  | Most widely used — **recommended**  |
| **snake_case** | `first_name` | Common in newer APIs (e.g., GitHub) |
| **PascalCase** | `FirstName`  | Uncommon for APIs — **avoid**       |

> Use a JSON library that supports configurable property naming. `System.Text.Json` defaults to camelCase in ASP.NET Core.

## Pagination

When a collection endpoint may return many results, implement pagination.

**Common approaches:**

- **Offset-based:** `GET /events?offset=20&limit=10`
- **Cursor-based:** `GET /events?cursor=abc123&limit=10` (preferred for large datasets)

**Response metadata:**

```json
{
  "data": [ ... ],
  "pagination": {
    "totalCount": 142,
    "pageSize": 10,
    "currentPage": 3,
    "totalPages": 15,
    "nextCursor": "eyJpZCI6MTB9"
  }
}
```

**Link headers** (following [RFC 8288](https://www.rfc-editor.org/rfc/rfc8288)):

```http
Link: </api/events?cursor=abc123&limit=10>; rel="next",
      </api/events?cursor=xyz789&limit=10>; rel="prev"
```

Example: [GitHub API Pagination](https://docs.github.com/en/rest/using-the-rest-api/using-pagination-in-the-rest-api)

## HTTP-Verb Override

Some corporate firewalls only allow `GET` and `POST` requests. The `X-HTTP-Method-Override` header tells the server which method was actually intended.

```http
POST /api/events/42 HTTP/1.1
Host: api.techconf.example.com
Content-Type: application/json
X-HTTP-Method-Override: PUT
```

> **Never** use this with `GET` requests — `GET` must never change the state of a resource.

## Rate Limiting

- Especially public APIs should limit client access
  - Total number of requests
  - Requests within a specific time window

Inform clients about their rate limit status using standard headers (following the [IETF Rate Limiting draft](https://datatracker.ietf.org/doc/draft-ietf-httpapi-ratelimit-headers/)):

| Header                | Description                        |
| --------------------- | ---------------------------------- |
| `RateLimit-Limit`     | Maximum number of allowed requests |
| `RateLimit-Remaining` | Number of requests remaining       |
| `RateLimit-Reset`     | Seconds until the limit resets     |

When a client exceeds the limit, return `429 Too Many Requests` with a `Retry-After` header.

ASP.NET Core has built-in rate limiting middleware:

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

app.UseRateLimiter();
```

## Additional Considerations

- **Caching** — HTTP has built-in caching mechanisms. Use `ETag` and `Last-Modified` headers for efficient API caching. See [RFC 9110 — Conditional Requests](https://www.rfc-editor.org/rfc/rfc9110#section-13).
- **TLS/HTTPS** — Always encrypt API traffic to protect data in transit and ensure integrity. In .NET 10, HTTPS is enforced by default.
- **Authentication & Authorization** — Most APIs are protected with standards like OAuth 2.0 / OpenID Connect. ASP.NET Core supports JWT Bearer tokens, cookie auth, and identity providers.
- **Compression** — Enable response compression (`gzip`, `brotli`) for significant bandwidth savings. ASP.NET Core includes `ResponseCompression` middleware.
- **Error Handling** — Use proper status codes (`4xx` = client error, `5xx` = server error) and return structured error bodies using [RFC 9457 Problem Details](#error-responses--rfc-9457-problem-details).
- **CORS** — Configure Cross-Origin Resource Sharing appropriately when your API is consumed by browser-based clients.
- **Health Checks** — Expose `/health` endpoints for monitoring and orchestration platforms.

## Resources

- [Roy Fielding's Dissertation](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm)
- [RFC 9110 — HTTP Semantics](https://www.rfc-editor.org/rfc/rfc9110)
- [RFC 9457 — Problem Details for HTTP APIs](https://www.rfc-editor.org/rfc/rfc9457)
- [Microsoft — RESTful Web API Design](https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design)
- [GitHub REST API Documentation](https://docs.github.com/en/rest)
- [Best Practices for a Pragmatic RESTful API](http://www.vinaysahni.com/best-practices-for-a-pragmatic-restful-api)
