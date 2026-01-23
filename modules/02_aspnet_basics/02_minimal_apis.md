# Lesson 02: Minimal APIs

## Table of Contents

1. [Introduction](#introduction)
2. [What are Minimal APIs?](#what-are-minimal-apis)
3. [Creating Your First CRUD API](#creating-your-first-crud-api)
4. [Routing in Detail](#routing-in-detail)
5. [HTTP Methods and Endpoints](#http-methods-and-endpoints)
6. [Parameter Binding](#parameter-binding)
7. [Response Types](#response-types)
8. [Organizing Large APIs](#organizing-large-apis)
9. [Best Practices](#best-practices)
10. [Exercises](#exercises)

## Introduction

In Lesson 01, you created a simple "Hello World" API. Now it's time to build real, production-ready APIs using **Minimal APIs**—ASP.NET Core's streamlined approach to building HTTP APIs with minimal ceremony and maximum productivity.

### Learning Objectives

After completing this lesson, you will be able to:

- Create full CRUD (Create, Read, Update, Delete) APIs
- Implement advanced routing patterns and constraints
- Bind parameters from routes, query strings, headers, and body
- Return appropriate HTTP responses with proper status codes
- Organize endpoints in larger applications
- Follow RESTful design principles in practice

### What You'll Build

By the end of this lesson, you'll have built a fully functional **Chuck Norris Joke API** with:

- ✅ Get all jokes (with optional filtering and sorting)
- ✅ Get a specific joke by ID
- ✅ Create a new joke
- ✅ Update an existing joke
- ✅ Delete a joke
- ✅ Get a random joke
- ✅ Search jokes by text

## What are Minimal APIs?

**Minimal APIs** are ASP.NET Core's approach to creating HTTP APIs with minimal code and dependencies, introduced in .NET 6 and enhanced in subsequent versions.

### Traditional vs. Minimal APIs

#### Traditional Approach (Controllers)

```csharp
// JokesController.cs
[ApiController]
[Route("api/[controller]")]
public class JokesController : ControllerBase
{
    private readonly IJokeService _jokeService;
    
    public JokesController(IJokeService jokeService)
    {
        _jokeService = jokeService;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<Joke>> GetAll()
    {
        return Ok(_jokeService.GetAll());
    }
    
    [HttpGet("{id}")]
    public ActionResult<Joke> GetById(int id)
    {
        var joke = _jokeService.GetById(id);
        if (joke == null)
            return NotFound();
        return Ok(joke);
    }
}
```

#### Minimal API Approach

```csharp
// Program.cs
app.MapGet("/api/jokes", (IJokeService service) => service.GetAll());

app.MapGet("/api/jokes/{id}", (int id, IJokeService service) =>
{
    var joke = service.GetById(id);
    return joke is null ? Results.NotFound() : Results.Ok(joke);
});
```

### Key Differences

| Aspect | Controllers | Minimal APIs |
|--------|-------------|--------------|
| **Lines of code** | More boilerplate | Less ceremony |
| **Learning curve** | Steeper | Gentler |
| **Performance** | Good | Slightly better |
| **Testing** | Well-established patterns | Newer patterns |
| **Filters** | Rich attribute-based | Endpoint filters (.NET 7+) |
| **Model binding** | Attribute-based | Parameter-based |
| **Organization** | Class-based | Method/lambda-based |

### When to Use Minimal APIs

✅ **Use Minimal APIs when:**
- Building microservices
- Creating simple to medium-complexity APIs
- Prioritizing performance and small footprint
- Wanting less boilerplate code
- Building APIs for modern SPAs or mobile apps

⚠️ **Consider Controllers when:**
- You have very complex APIs with many filters
- Your team is already expert with Controllers
- You need advanced features like API versioning (though this is improving)

**For this course**: We'll use Minimal APIs throughout because they're modern, performant, and easier to learn.

## Creating Your First CRUD API

Let's build the Chuck Norris Joke API step by step.

### Step 1: Create the Project

```bash
# Create a new minimal API project
dotnet new web -n ChuckNorrisApi

# Navigate to the project
cd ChuckNorrisApi

# Open in your IDE
code .
```

### Step 2: Define the Model

Create a `Models` folder and add `Joke.cs`:

```csharp
namespace ChuckNorrisApi.Models;

public record Joke(
    int Id,
    string Text,
    string Category,
    DateTime CreatedAt
)
{
    // Constructor for creating new jokes (without ID and timestamp)
    public Joke(string text, string category) 
        : this(0, text, category, DateTime.UtcNow)
    {
    }
}
```

**Why use a record?**
- Immutability by default (good for data transfer objects)
- Built-in value equality
- Concise syntax
- Perfect for API models

### Step 3: Create an In-Memory Data Store

For now, we'll use a simple in-memory store. (We'll use a real database in Module 04.)

```csharp
// Program.cs
using ChuckNorrisApi.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory data store (temporary - will be replaced with a database)
var jokes = new List<Joke>
{
    new(1, "Chuck Norris can divide by zero.", "math", DateTime.UtcNow.AddDays(-10)),
    new(2, "Chuck Norris can compile syntax errors.", "programming", DateTime.UtcNow.AddDays(-9)),
    new(3, "Chuck Norris can access private methods.", "programming", DateTime.UtcNow.AddDays(-8)),
    new(4, "Chuck Norris doesn't use debuggers, debuggers use Chuck Norris.", "programming", DateTime.UtcNow.AddDays(-7)),
    new(5, "Chuck Norris's keyboard doesn't have a Ctrl key because nothing controls Chuck Norris.", "general", DateTime.UtcNow.AddDays(-6))
};
var nextId = 6;

app.Run();
```

### Step 4: Implement GET All Jokes

```csharp
// GET /api/jokes - Get all jokes
app.MapGet("/api/jokes", () => jokes);
```

That's it! Let's test it:

```bash
dotnet run

# In another terminal:
curl http://localhost:5000/api/jokes
```

**Response:**
```json
[
  {
    "id": 1,
    "text": "Chuck Norris can divide by zero.",
    "category": "math",
    "createdAt": "2025-01-14T10:00:00Z"
  },
  ...
]
```

### Step 5: Implement GET Joke by ID

```csharp
// GET /api/jokes/{id} - Get a specific joke
app.MapGet("/api/jokes/{id:int}", (int id) =>
{
    var joke = jokes.FirstOrDefault(j => j.Id == id);
    return joke is null 
        ? Results.NotFound(new { message = $"Joke with ID {id} not found." })
        : Results.Ok(joke);
});
```

**Key points:**
- `{id:int}` is a route parameter with a type constraint
- `Results.NotFound()` returns HTTP 404
- `Results.Ok()` returns HTTP 200
- Ternary operator keeps code concise

**Test it:**
```bash
# Existing joke
curl http://localhost:5000/api/jokes/1
# Returns: 200 OK with joke

# Non-existent joke
curl http://localhost:5000/api/jokes/999
# Returns: 404 Not Found with error message
```

### Step 6: Implement POST (Create) Joke

```csharp
// POST /api/jokes - Create a new joke
app.MapPost("/api/jokes", (Joke newJoke) =>
{
    var joke = new Joke(nextId++, newJoke.Text, newJoke.Category, DateTime.UtcNow);
    jokes.Add(joke);
    return Results.Created($"/api/jokes/{joke.Id}", joke);
});
```

**Key points:**
- `Joke newJoke` parameter is bound from the request body (JSON)
- `Results.Created()` returns HTTP 201 with a `Location` header
- The `Location` header points to the new resource's URL

**Test it:**
```bash
curl -X POST http://localhost:5000/api/jokes \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Chuck Norris can uninstall Windows.",
    "category": "tech"
  }'
```

**Response:**
```http
HTTP/1.1 201 Created
Location: /api/jokes/6
Content-Type: application/json

{
  "id": 6,
  "text": "Chuck Norris can uninstall Windows.",
  "category": "tech",
  "createdAt": "2025-01-24T10:00:00Z"
}
```

### Step 7: Implement PUT (Update) Joke

```csharp
// PUT /api/jokes/{id} - Replace an existing joke
app.MapPut("/api/jokes/{id:int}", (int id, Joke updatedJoke) =>
{
    var index = jokes.FindIndex(j => j.Id == id);
    if (index == -1)
        return Results.NotFound(new { message = $"Joke with ID {id} not found." });
    
    var joke = new Joke(id, updatedJoke.Text, updatedJoke.Category, jokes[index].CreatedAt);
    jokes[index] = joke;
    return Results.Ok(joke);
});
```

**Key points:**
- PUT replaces the entire resource
- We preserve the `CreatedAt` timestamp from the original
- Returns 404 if the joke doesn't exist
- Returns 200 with the updated joke

**Test it:**
```bash
curl -X PUT http://localhost:5000/api/jokes/1 \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Chuck Norris can divide by zero and get infinity + 1.",
    "category": "math"
  }'
```

### Step 8: Implement DELETE Joke

```csharp
// DELETE /api/jokes/{id} - Delete a joke
app.MapDelete("/api/jokes/{id:int}", (int id) =>
{
    var index = jokes.FindIndex(j => j.Id == id);
    if (index == -1)
        return Results.NotFound(new { message = $"Joke with ID {id} not found." });
    
    jokes.RemoveAt(index);
    return Results.NoContent();
});
```

**Key points:**
- `Results.NoContent()` returns HTTP 204 (success with no body)
- Some APIs return 200 with the deleted resource; both are acceptable

**Test it:**
```bash
curl -X DELETE http://localhost:5000/api/jokes/1

# Returns: 204 No Content (empty response)
```

### Complete Program.cs So Far

```csharp
using ChuckNorrisApi.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory data store
var jokes = new List<Joke>
{
    new(1, "Chuck Norris can divide by zero.", "math", DateTime.UtcNow.AddDays(-10)),
    new(2, "Chuck Norris can compile syntax errors.", "programming", DateTime.UtcNow.AddDays(-9)),
    new(3, "Chuck Norris can access private methods.", "programming", DateTime.UtcNow.AddDays(-8)),
    new(4, "Chuck Norris doesn't use debuggers, debuggers use Chuck Norris.", "programming", DateTime.UtcNow.AddDays(-7)),
    new(5, "Chuck Norris's keyboard doesn't have a Ctrl key.", "general", DateTime.UtcNow.AddDays(-6))
};
var nextId = 6;

// GET all jokes
app.MapGet("/api/jokes", () => jokes);

// GET joke by ID
app.MapGet("/api/jokes/{id:int}", (int id) =>
{
    var joke = jokes.FirstOrDefault(j => j.Id == id);
    return joke is null 
        ? Results.NotFound(new { message = $"Joke with ID {id} not found." })
        : Results.Ok(joke);
});

// POST new joke
app.MapPost("/api/jokes", (Joke newJoke) =>
{
    var joke = new Joke(nextId++, newJoke.Text, newJoke.Category, DateTime.UtcNow);
    jokes.Add(joke);
    return Results.Created($"/api/jokes/{joke.Id}", joke);
});

// PUT update joke
app.MapPut("/api/jokes/{id:int}", (int id, Joke updatedJoke) =>
{
    var index = jokes.FindIndex(j => j.Id == id);
    if (index == -1)
        return Results.NotFound(new { message = $"Joke with ID {id} not found." });
    
    var joke = new Joke(id, updatedJoke.Text, updatedJoke.Category, jokes[index].CreatedAt);
    jokes[index] = joke;
    return Results.Ok(joke);
});

// DELETE joke
app.MapDelete("/api/jokes/{id:int}", (int id) =>
{
    var index = jokes.FindIndex(j => j.Id == id);
    if (index == -1)
        return Results.NotFound(new { message = $"Joke with ID {id} not found." });
    
    jokes.RemoveAt(index);
    return Results.NoContent();
});

app.Run();
```

You now have a fully functional CRUD API in about 50 lines of code! 🎉

## Routing in Detail

Routing is the process of matching incoming HTTP requests to endpoint handlers. Let's explore routing in depth.

### Route Templates

Route templates define URL patterns:

```csharp
// Static route
app.MapGet("/api/jokes", ...);

// Route with parameter
app.MapGet("/api/jokes/{id}", ...);

// Route with multiple parameters
app.MapGet("/api/users/{userId}/jokes/{jokeId}", ...);

// Optional parameter (using ?)
app.MapGet("/api/jokes/{id?}", ...);

// Catch-all parameter (using *)
app.MapGet("/files/{*path}", ...);
```

### Route Parameters

#### Basic Parameters

```csharp
// Simple parameter
app.MapGet("/api/jokes/{id}", (int id) => $"Joke ID: {id}");

// Multiple parameters
app.MapGet("/api/categories/{category}/jokes/{id}", 
    (string category, int id) => $"Category: {category}, ID: {id}");
```

#### Optional Parameters

```csharp
// Optional parameter (must be nullable)
app.MapGet("/api/jokes/{id?}", (int? id) =>
{
    return id.HasValue 
        ? $"Joke {id.Value}" 
        : "All jokes";
});
```

### Route Constraints

Constraints validate route parameters before the endpoint handler is invoked:

```csharp
// Integer constraint
app.MapGet("/api/jokes/{id:int}", (int id) => ...);

// Minimum value
app.MapGet("/api/jokes/{id:int:min(1)}", (int id) => ...);

// Maximum value
app.MapGet("/api/jokes/{id:int:max(1000)}", (int id) => ...);

// Range
app.MapGet("/api/jokes/{id:int:range(1,1000)}", (int id) => ...);

// String length
app.MapGet("/api/categories/{name:minlength(2):maxlength(50)}", (string name) => ...);

// Regular expression
app.MapGet("/api/jokes/{code:regex(^[A-Z]{{3}}[0-9]{{3}}$)}", (string code) => ...);

// GUID
app.MapGet("/api/jokes/{id:guid}", (Guid id) => ...);

// DateTime
app.MapGet("/api/jokes/created/{date:datetime}", (DateTime date) => ...);

// Boolean
app.MapGet("/api/settings/{enabled:bool}", (bool enabled) => ...);
```

#### Multiple Constraints

```csharp
// Combine multiple constraints with colons
app.MapGet("/api/jokes/{id:int:min(1):max(1000)}", (int id) => ...);
```

#### Common Built-in Constraints

| Constraint | Description | Example |
|------------|-------------|---------|
| `int` | Must be an integer | `{id:int}` |
| `bool` | Must be true or false | `{flag:bool}` |
| `datetime` | Must be a DateTime | `{date:datetime}` |
| `decimal` | Must be a decimal | `{price:decimal}` |
| `double` | Must be a double | `{rating:double}` |
| `float` | Must be a float | `{score:float}` |
| `guid` | Must be a GUID | `{id:guid}` |
| `long` | Must be a long | `{id:long}` |
| `minlength(n)` | Minimum string length | `{name:minlength(2)}` |
| `maxlength(n)` | Maximum string length | `{name:maxlength(50)}` |
| `length(n)` | Exact string length | `{code:length(6)}` |
| `min(n)` | Minimum numeric value | `{id:min(1)}` |
| `max(n)` | Maximum numeric value | `{id:max(1000)}` |
| `range(min,max)` | Numeric range | `{age:range(18,120)}` |
| `alpha` | Letters only | `{name:alpha}` |
| `regex(expr)` | Matches regex | `{code:regex(^[A-Z]{{3}}$)}` |

### Route Precedence

When multiple routes could match a request, ASP.NET Core uses specific rules:

```csharp
// More specific routes should be defined first
app.MapGet("/api/jokes/random", () => "Random joke");          // 1. Most specific
app.MapGet("/api/jokes/{id:int}", (int id) => $"Joke {id}");  // 2. Constrained
app.MapGet("/api/jokes/{slug}", (string slug) => $"Slug: {slug}"); // 3. Unconstrained

// Without proper ordering, /api/jokes/random might match {slug} instead!
```

**Best practice**: Define more specific routes before more general ones.

### Route Groups (ASP.NET Core 7+)

Group related endpoints to avoid repetition:

```csharp
var jokesGroup = app.MapGroup("/api/jokes");

jokesGroup.MapGet("/", () => jokes);
jokesGroup.MapGet("/{id:int}", (int id) => ...);
jokesGroup.MapPost("/", (Joke joke) => ...);
jokesGroup.MapPut("/{id:int}", (int id, Joke joke) => ...);
jokesGroup.MapDelete("/{id:int}", (int id) => ...);

// All endpoints are now under /api/jokes
```

You can also apply common configuration to a group:

```csharp
var jokesGroup = app.MapGroup("/api/jokes")
    .RequireAuthorization()  // All endpoints require authentication
    .WithTags("Jokes");      // For OpenAPI documentation
```

## HTTP Methods and Endpoints

Let's dive deeper into each HTTP method and when to use them.

### GET - Retrieve Resources

**Purpose**: Retrieve one or more resources without modifying server state.

**Characteristics**:
- ✅ Safe (doesn't modify state)
- ✅ Idempotent (multiple calls have the same effect)
- ✅ Cacheable
- ❌ Should not have a request body

**Examples**:

```csharp
// Get all resources
app.MapGet("/api/jokes", () => jokes);

// Get a specific resource
app.MapGet("/api/jokes/{id:int}", (int id) => ...);

// Get with filtering
app.MapGet("/api/jokes", (string? category) =>
{
    if (category is null)
        return jokes;
    return jokes.Where(j => j.Category == category);
});

// Get with pagination
app.MapGet("/api/jokes", (int page = 1, int pageSize = 10) =>
{
    var paginatedJokes = jokes
        .Skip((page - 1) * pageSize)
        .Take(pageSize);
    return new { 
        data = paginatedJokes, 
        page, 
        pageSize, 
        totalCount = jokes.Count 
    };
});
```

### POST - Create Resources

**Purpose**: Create a new resource.

**Characteristics**:
- ❌ Not safe (modifies state)
- ❌ Not idempotent (multiple calls create multiple resources)
- ❌ Not cacheable
- ✅ Should have a request body

**Best practices**:
- Return `201 Created` with `Location` header
- Return the created resource in the response body
- Generate ID on the server, not client

```csharp
app.MapPost("/api/jokes", (Joke newJoke) =>
{
    // Validate (we'll add proper validation in Lesson 06)
    if (string.IsNullOrWhiteSpace(newJoke.Text))
        return Results.BadRequest(new { message = "Text is required." });
    
    // Create with server-generated ID
    var joke = new Joke(nextId++, newJoke.Text, newJoke.Category, DateTime.UtcNow);
    jokes.Add(joke);
    
    // Return 201 Created with Location header
    return Results.Created($"/api/jokes/{joke.Id}", joke);
});
```

### PUT - Replace Resources

**Purpose**: Replace an existing resource entirely.

**Characteristics**:
- ❌ Not safe (modifies state)
- ✅ Idempotent (multiple calls have the same effect)
- ❌ Not cacheable
- ✅ Should have a request body

**Key concept**: PUT should replace the **entire** resource.

```csharp
app.MapPut("/api/jokes/{id:int}", (int id, Joke updatedJoke) =>
{
    var index = jokes.FindIndex(j => j.Id == id);
    if (index == -1)
        return Results.NotFound();
    
    // Replace entire resource (except server-managed fields like CreatedAt)
    var joke = new Joke(id, updatedJoke.Text, updatedJoke.Category, jokes[index].CreatedAt);
    jokes[index] = joke;
    
    return Results.Ok(joke);
});
```

### PATCH - Partial Update

**Purpose**: Partially update a resource (modify specific fields).

**Characteristics**:
- ❌ Not safe (modifies state)
- ⚠️ Not idempotent (depends on implementation)
- ❌ Not cacheable
- ✅ Should have a request body

**Implementation options**:

#### Option 1: Simple Approach (JSON with nullable fields)

```csharp
// DTO for partial updates
record JokePatchDto(string? Text, string? Category);

app.MapPatch("/api/jokes/{id:int}", (int id, JokePatchDto patch) =>
{
    var index = jokes.FindIndex(j => j.Id == id);
    if (index == -1)
        return Results.NotFound();
    
    var existing = jokes[index];
    var updated = new Joke(
        id,
        patch.Text ?? existing.Text,        // Update only if provided
        patch.Category ?? existing.Category, // Update only if provided
        existing.CreatedAt
    );
    
    jokes[index] = updated;
    return Results.Ok(updated);
});
```

**Test it**:
```bash
# Update only the text
curl -X PATCH http://localhost:5000/api/jokes/1 \
  -H "Content-Type: application/json" \
  -d '{"text": "Updated text"}'

# Update only the category
curl -X PATCH http://localhost:5000/api/jokes/1 \
  -H "Content-Type: application/json" \
  -d '{"category": "updated-category"}'
```

#### Option 2: JSON Patch (RFC 6902)

For more complex scenarios, use JSON Patch standard:

```json
[
  { "op": "replace", "path": "/text", "value": "New text" },
  { "op": "replace", "path": "/category", "value": "new-category" }
]
```

(We'll cover this in advanced lessons.)

### DELETE - Remove Resources

**Purpose**: Delete a resource.

**Characteristics**:
- ❌ Not safe (modifies state)
- ✅ Idempotent (deleting twice has same effect as once)
- ❌ Not cacheable
- ❌ Usually no request body

**Return options**:
- `204 No Content` - Common choice (resource deleted, nothing to return)
- `200 OK` - With deleted resource in body
- `202 Accepted` - For async deletion

```csharp
app.MapDelete("/api/jokes/{id:int}", (int id) =>
{
    var index = jokes.FindIndex(j => j.Id == id);
    if (index == -1)
        return Results.NotFound();
    
    jokes.RemoveAt(index);
    return Results.NoContent();  // 204 No Content
});

// Alternative: Return deleted resource
app.MapDelete("/api/jokes/{id:int}", (int id) =>
{
    var joke = jokes.FirstOrDefault(j => j.Id == id);
    if (joke is null)
        return Results.NotFound();
    
    jokes.Remove(joke);
    return Results.Ok(joke);  // 200 OK with deleted joke
});
```

### HEAD - Get Headers Only

**Purpose**: Same as GET, but only returns headers (no body).

**Use case**: Check if a resource exists or get metadata without downloading the body.

```csharp
app.MapHead("/api/jokes/{id:int}", (int id) =>
{
    var joke = jokes.FirstOrDefault(j => j.Id == id);
    return joke is null ? Results.NotFound() : Results.Ok();
});
```

### OPTIONS - Discover Allowed Methods

**Purpose**: Discover which HTTP methods are allowed for a resource.

**Use case**: CORS preflight requests, API discovery.

ASP.NET Core handles this automatically for CORS, but you can implement it manually:

```csharp
app.MapMethods("/api/jokes/{id:int}", new[] { "OPTIONS" }, (int id) =>
{
    return Results.Ok(new { 
        allowedMethods = new[] { "GET", "PUT", "DELETE", "OPTIONS" }
    });
});
```

## Parameter Binding

ASP.NET Core can bind parameters from multiple sources. Understanding parameter binding is crucial for building flexible APIs.

### Binding Sources

```csharp
app.MapGet("/api/jokes/{id}", 
    (
        int id,                    // From route
        string? category,          // From query string
        [FromHeader(Name = "X-API-Key")] string apiKey,  // From header
        HttpContext context        // Special service
    ) => 
    {
        // Use parameters
        return Results.Ok(new { id, category, apiKey });
    });
```

### 1. Route Parameters

Bound from the URL path:

```csharp
// URL: /api/jokes/42
app.MapGet("/api/jokes/{id:int}", (int id) => $"Joke {id}");

// URL: /api/users/alice/jokes/42
app.MapGet("/api/users/{username}/jokes/{jokeId:int}", 
    (string username, int jokeId) => $"User: {username}, Joke: {jokeId}");
```

### 2. Query String Parameters

Bound from the query string:

```csharp
// URL: /api/jokes?category=programming&minRating=4.5
app.MapGet("/api/jokes", (string? category, double? minRating) =>
{
    var filtered = jokes.AsEnumerable();
    
    if (category is not null)
        filtered = filtered.Where(j => j.Category == category);
    
    // minRating logic would go here if we had ratings
    
    return filtered;
});
```

**Complex query parameters**:

```csharp
// URL: /api/jokes?categories=math&categories=programming
app.MapGet("/api/jokes", (string[]? categories) =>
{
    if (categories is null || categories.Length == 0)
        return jokes;
    
    return jokes.Where(j => categories.Contains(j.Category));
});
```

### 3. Request Body

Bound from the request body (JSON by default):

```csharp
app.MapPost("/api/jokes", (Joke newJoke) =>
{
    // newJoke is deserialized from JSON body
    var joke = new Joke(nextId++, newJoke.Text, newJoke.Category, DateTime.UtcNow);
    jokes.Add(joke);
    return Results.Created($"/api/jokes/{joke.Id}", joke);
});
```

### 4. Headers

Bound from HTTP headers:

```csharp
app.MapGet("/api/jokes", 
    ([FromHeader(Name = "X-API-Key")] string? apiKey) =>
    {
        if (apiKey != "secret123")
            return Results.Unauthorized();
        
        return Results.Ok(jokes);
    });
```

### 5. Services (Dependency Injection)

Services registered in DI container:

```csharp
app.MapGet("/api/jokes", (IJokeService service) => service.GetAll());
```

### 6. Special Types

ASP.NET Core provides special types you can inject:

```csharp
app.MapGet("/api/jokes", 
    (
        HttpContext context,        // Current HTTP context
        HttpRequest request,        // Current request
        HttpResponse response,      // Current response
        ClaimsPrincipal user,       // Current user (authentication)
        CancellationToken ct        // Cancellation token
    ) => 
    {
        // Access to full request/response objects
        var userAgent = request.Headers.UserAgent;
        var method = request.Method;
        
        return Results.Ok(new { userAgent, method });
    });
```

### Binding Attributes

Control binding behavior with attributes:

| Attribute | Source |
|-----------|--------|
| `[FromRoute]` | Route parameters |
| `[FromQuery]` | Query string |
| `[FromHeader]` | HTTP headers |
| `[FromBody]` | Request body |
| `[FromServices]` | DI container |
| `[FromForm]` | Form data |

**Example**:

```csharp
app.MapPost("/api/jokes", 
    (
        [FromBody] Joke joke,
        [FromHeader(Name = "X-API-Key")] string apiKey,
        [FromServices] IJokeService service
    ) => 
    {
        // ...
    });
```

### Binding Priority

When ASP.NET Core can't determine the source, it uses this priority:

1. Special types (HttpContext, CancellationToken, etc.)
2. Services from DI container
3. Route parameters (if name matches)
4. Query string (if simple types)
5. Request body (if complex types)

## Response Types

How you return data from endpoints matters. Let's explore all the options.

### Results Helper Methods

The `Results` static class provides methods for common responses:

#### Success Responses (2xx)

```csharp
// 200 OK
Results.Ok(data);
Results.Ok();  // No body

// 201 Created
Results.Created($"/api/jokes/{id}", joke);
Results.Created(new Uri($"/api/jokes/{id}", UriKind.Relative), joke);

// 202 Accepted (async operations)
Results.Accepted($"/api/jobs/{jobId}", new { jobId, status = "processing" });

// 204 No Content
Results.NoContent();
```

#### Client Error Responses (4xx)

```csharp
// 400 Bad Request
Results.BadRequest(new { message = "Invalid request" });

// 401 Unauthorized
Results.Unauthorized();

// 403 Forbidden
Results.Forbid();

// 404 Not Found
Results.NotFound(new { message = "Joke not found" });

// 409 Conflict
Results.Conflict(new { message = "Joke already exists" });

// 422 Unprocessable Entity (validation errors)
Results.UnprocessableEntity(new { errors = validationErrors });
```

#### Server Error Responses (5xx)

```csharp
// 500 Internal Server Error
Results.StatusCode(500);

// 503 Service Unavailable
Results.StatusCode(503);
```

#### Other Responses

```csharp
// Redirect (301, 302, 307, 308)
Results.Redirect("https://example.com");
Results.RedirectPermanent("https://example.com");

// File download
Results.File(bytes, "application/pdf", "document.pdf");

// Empty response
Results.Empty;

// Custom status code
Results.StatusCode(418);  // I'm a teapot
```

### Direct Return Values

You can also return values directly:

```csharp
// ASP.NET Core infers 200 OK with JSON body
app.MapGet("/api/jokes", () => jokes);

// Returns string as text/plain
app.MapGet("/api/jokes/{id}/text", (int id) =>
{
    var joke = jokes.FirstOrDefault(j => j.Id == id);
    return joke?.Text ?? "Not found";
});

// Returns int
app.MapGet("/api/jokes/count", () => jokes.Count);
```

**When to use which**:

- **Direct return**: Simple cases, always 200 OK
- **Results methods**: Need specific status codes or headers

### TypedResults (Strongly-Typed)

ASP.NET Core 7+ introduced `TypedResults` for better type safety:

```csharp
app.MapGet("/api/jokes/{id:int}", 
    (int id) =>
    {
        var joke = jokes.FirstOrDefault(j => j.Id == id);
        return joke is null 
            ? TypedResults.NotFound()
            : TypedResults.Ok(joke);
    });
```

**Benefits**:
- Better IntelliSense
- Compile-time type checking
- OpenAPI documentation generation

## Organizing Large APIs

As your API grows, having everything in `Program.cs` becomes unwieldy. Let's organize it better.

### Option 1: Extension Methods

Create extension methods to group related endpoints:

```csharp
// Endpoints/JokeEndpoints.cs
public static class JokeEndpoints
{
    public static void MapJokeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/jokes");
        
        group.MapGet("/", GetAll);
        group.MapGet("/{id:int}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:int}", Update);
        group.MapDelete("/{id:int}", Delete);
        group.MapGet("/random", GetRandom);
    }
    
    private static IResult GetAll()
    {
        // Implementation
    }
    
    private static IResult GetById(int id)
    {
        // Implementation
    }
    
    // ... other methods
}
```

**Program.cs**:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapJokeEndpoints();  // Clean!

app.Run();
```

### Option 2: Endpoint Filters (Advanced)

For cross-cutting concerns (validation, logging, etc.):

```csharp
app.MapPost("/api/jokes", (Joke joke) => 
{
    // Create joke
})
.AddEndpointFilter<ValidationFilter<Joke>>();
```

(We'll cover filters in detail in later lessons.)

### Option 3: IEndpointRouteBuilder Extensions

For reusable endpoint groups:

```csharp
public static class EndpointExtensions
{
    public static RouteGroupBuilder MapApiGroup(this WebApplication app, string prefix)
    {
        return app.MapGroup(prefix)
            .WithOpenApi()
            .WithTags(prefix.TrimStart('/'));
    }
}

// Usage
var jokes = app.MapApiGroup("/api/jokes");
jokes.MapGet("/", () => ...);
```

## Best Practices

Let's summarize the best practices for building Minimal APIs.

### 1. Use Appropriate HTTP Methods

✅ **Do**:
```csharp
app.MapGet("/api/jokes", () => jokes);           // Retrieve
app.MapPost("/api/jokes", (Joke j) => ...);      // Create
app.MapPut("/api/jokes/{id}", ...);              // Replace
app.MapPatch("/api/jokes/{id}", ...);            // Partial update
app.MapDelete("/api/jokes/{id}", ...);           // Delete
```

❌ **Don't**:
```csharp
app.MapPost("/api/jokes/get", () => jokes);      // Wrong! Use GET
app.MapGet("/api/jokes/create", () => ...);      // Wrong! Use POST
```

### 2. Return Correct Status Codes

✅ **Do**:
```csharp
app.MapGet("/api/jokes/{id}", (int id) =>
{
    var joke = jokes.FirstOrDefault(j => j.Id == id);
    return joke is null ? Results.NotFound() : Results.Ok(joke);
});

app.MapPost("/api/jokes", (Joke joke) =>
{
    // ... create joke
    return Results.Created($"/api/jokes/{joke.Id}", joke);
});
```

❌ **Don't**:
```csharp
app.MapGet("/api/jokes/{id}", (int id) =>
{
    var joke = jokes.FirstOrDefault(j => j.Id == id);
    // WRONG: Always returns 200, even when not found
    return joke is null 
        ? new { error = "Not found" } 
        : joke;
});
```

### 3. Use Route Constraints

✅ **Do**:
```csharp
app.MapGet("/api/jokes/{id:int:min(1)}", (int id) => ...);
```

❌ **Don't**:
```csharp
app.MapGet("/api/jokes/{id}", (int id) =>
{
    if (id < 1)
        return Results.BadRequest();
    // ...
});
```

### 4. Keep Endpoints Focused

✅ **Do**:
```csharp
// Each endpoint has a single responsibility
app.MapGet("/api/jokes", (IJokeService service) => service.GetAll());
app.MapGet("/api/jokes/{id}", (int id, IJokeService service) => service.GetById(id));
```

❌ **Don't**:
```csharp
// Trying to do too much in one endpoint
app.MapGet("/api/jokes", (string? action, int? id, IJokeService service) =>
{
    if (action == "random")
        return service.GetRandom();
    if (id.HasValue)
        return service.GetById(id.Value);
    return service.GetAll();
});
```

### 5. Use Dependency Injection

✅ **Do**:
```csharp
// Services injected automatically
app.MapGet("/api/jokes", (IJokeService service) => service.GetAll());
```

❌ **Don't**:
```csharp
// Creating services manually
app.MapGet("/api/jokes", () =>
{
    var service = new JokeService(); // Avoid this!
    return service.GetAll();
});
```

### 6. Organize Related Endpoints

✅ **Do**:
```csharp
var jokes = app.MapGroup("/api/jokes");
jokes.MapGet("/", ...);
jokes.MapGet("/{id}", ...);
jokes.MapPost("/", ...);
```

### 7. Use Record Types for DTOs

✅ **Do**:
```csharp
record Joke(int Id, string Text, string Category);
record CreateJokeRequest(string Text, string Category);
```

### 8. Handle Not Found Consistently

✅ **Do**:
```csharp
return joke is null 
    ? Results.NotFound(new { message = $"Joke {id} not found" })
    : Results.Ok(joke);
```

## Exercises

### Exercise 1: Complete CRUD API (60 minutes)

Build the complete Chuck Norris Joke API with all endpoints:

1. GET /api/jokes - Get all jokes
2. GET /api/jokes/{id} - Get specific joke
3. POST /api/jokes - Create joke
4. PUT /api/jokes/{id} - Update joke
5. DELETE /api/jokes/{id} - Delete joke

**Requirements**:
- Use proper HTTP methods
- Return correct status codes
- Use route constraints
- Handle errors gracefully

### Exercise 2: Add Filtering and Sorting (30 minutes)

Enhance the GET /api/jokes endpoint:

```csharp
// GET /api/jokes?category=programming&sortBy=createdAt&order=desc
```

**Filters**:
- `category` - Filter by category
- `search` - Search in text (case-insensitive)

**Sorting**:
- `sortBy` - Sort by field (text, category, createdAt)
- `order` - asc or desc

### Exercise 3: Add Pagination (30 minutes)

Add pagination to GET /api/jokes:

```csharp
// GET /api/jokes?page=2&pageSize=10
```

Return:
```json
{
  "data": [...],
  "page": 2,
  "pageSize": 10,
  "totalCount": 45,
  "totalPages": 5
}
```

### Exercise 4: Add Special Endpoints (30 minutes)

Add these endpoints:

1. GET /api/jokes/random - Returns a random joke
2. GET /api/jokes/categories - Returns list of categories
3. GET /api/jokes/stats - Returns statistics (count per category)

### Exercise 5: Organize Your Code (30 minutes)

Refactor your code:

1. Move jokes to a separate file (e.g., Data/JokesData.cs)
2. Create JokeEndpoints extension class
3. Move models to Models folder
4. Keep Program.cs clean

### Exercise 6: Add PATCH Support (30 minutes)

Implement PATCH /api/jokes/{id} to partially update jokes:

```bash
curl -X PATCH http://localhost:5000/api/jokes/1 \
  -H "Content-Type: application/json" \
  -d '{"text": "Updated text only"}'
```

## Summary

In this lesson, you learned:

- ✅ **Minimal APIs**: ASP.NET Core's streamlined approach to building APIs
- ✅ **CRUD operations**: Create, Read, Update, Delete with proper HTTP methods
- ✅ **Routing**: Route templates, parameters, and constraints
- ✅ **Parameter binding**: From routes, query, body, headers, and services
- ✅ **Response types**: Using Results methods for proper HTTP responses
- ✅ **Organization**: Keeping large APIs maintainable

### Key Takeaways

1. **Minimal APIs are productive**: Less boilerplate, more focus on logic
2. **HTTP methods matter**: Use the right method for the right operation
3. **Status codes communicate**: Return appropriate codes for every scenario
4. **Route constraints help**: Validate parameters before your handler runs
5. **Organization scales**: Extract endpoints as your API grows

### What's Next?

In the next lesson, [03. Dependency Injection](03_dependency_injection.md), you'll learn:

- What dependency injection is and why it matters
- The built-in DI container
- Service lifetimes (Singleton, Scoped, Transient)
- Refactoring your Chuck Norris API to use proper services

Ready to make your code more maintainable and testable? Let's go! 🚀

---

## Quick Reference

### Common Minimal API Patterns

```csharp
// GET all
app.MapGet("/api/jokes", () => jokes);

// GET by ID
app.MapGet("/api/jokes/{id:int}", (int id) =>
    jokes.FirstOrDefault(j => j.Id == id) is { } joke
        ? Results.Ok(joke)
        : Results.NotFound());

// POST create
app.MapPost("/api/jokes", (Joke joke) => {
    jokes.Add(joke);
    return Results.Created($"/api/jokes/{joke.Id}", joke);
});

// PUT update
app.MapPut("/api/jokes/{id:int}", (int id, Joke joke) => {
    var index = jokes.FindIndex(j => j.Id == id);
    if (index == -1) return Results.NotFound();
    jokes[index] = joke;
    return Results.Ok(joke);
});

// DELETE
app.MapDelete("/api/jokes/{id:int}", (int id) => {
    var removed = jokes.RemoveAll(j => j.Id == id);
    return removed > 0 ? Results.NoContent() : Results.NotFound();
});
```

Happy coding! 🎉
