# Lesson 01: Introduction to ASP.NET Core

## Table of Contents

1. [Introduction](#introduction)
2. [HTTP Fundamentals](#http-fundamentals)
3. [REST Principles](#rest-principles)
4. [ASP.NET Core Overview](#aspnet-core-overview)
5. [The .NET 10 Web Stack](#the-net-10-web-stack)
6. [Your First ASP.NET Core Application](#your-first-aspnet-core-application)
7. [Key Concepts Review](#key-concepts-review)
8. [Exercises](#exercises)

## Introduction

Welcome to ASP.NET Core! Before we start writing code, it's crucial to understand the fundamentals of web communication and the architectural principles that guide modern web API design. This lesson provides the theoretical foundation you need to become an effective ASP.NET Core developer.

### Learning Objectives

After completing this lesson, you will be able to:

- Explain how HTTP works and identify the key components of HTTP requests and responses
- Describe the REST architectural style and its constraints
- Understand the ASP.NET Core architecture and its benefits
- Identify when to use ASP.NET Core vs. other web frameworks
- Create a basic ASP.NET Core application
- Navigate the .NET 10 web stack

### Why This Matters

Many developers jump straight into coding without understanding the underlying protocols and principles. This leads to:

- ❌ APIs that don't follow HTTP semantics
- ❌ Misuse of HTTP status codes
- ❌ Poor error handling
- ❌ Security vulnerabilities
- ❌ Non-RESTful designs that are hard to use

By understanding these fundamentals, you'll write better, more intuitive APIs that are a joy to work with.

## HTTP Fundamentals

**HTTP (Hypertext Transfer Protocol)** is the foundation of data communication on the web. Everything you do with ASP.NET Core web APIs happens over HTTP.

### What is HTTP?

HTTP is an **application-layer protocol** for transmitting hypermedia documents. It follows a **client-server model**:

```
┌──────────┐                                    ┌──────────┐
│          │       HTTP Request                 │          │
│  Client  │  ─────────────────────────────────>│  Server  │
│          │                                     │          │
│          │       HTTP Response                │          │
│          │  <─────────────────────────────────│          │
└──────────┘                                    └──────────┘
```

### Key Characteristics

1. **Stateless**: Each request is independent; the server doesn't maintain client state
2. **Text-based**: Human-readable protocol (though HTTP/2 and HTTP/3 use binary)
3. **Request-Response**: Client initiates, server responds
4. **Port 80 (HTTP) or 443 (HTTPS)**: Default ports for web traffic

### Anatomy of an HTTP Request

Every HTTP request consists of:

```http
POST /api/jokes HTTP/1.1                          ← Request Line
Host: localhost:5000                              ← Headers
Content-Type: application/json
Authorization: Bearer eyJhbGc...
Content-Length: 85

{                                                 ← Body
  "text": "Chuck Norris can divide by zero.",
  "category": "math"
}
```

#### 1. Request Line

```
METHOD /path HTTP/version
```

- **METHOD**: What action to perform (GET, POST, PUT, DELETE, etc.)
- **Path**: The resource being accessed
- **HTTP Version**: Usually HTTP/1.1, HTTP/2, or HTTP/3

#### 2. Headers

Key-value pairs providing metadata about the request:

| Header | Purpose | Example |
|--------|---------|---------|
| `Host` | Target server | `localhost:5000` |
| `Content-Type` | Format of request body | `application/json` |
| `Accept` | Preferred response format | `application/json` |
| `Authorization` | Authentication credentials | `Bearer <token>` |
| `User-Agent` | Client information | `Mozilla/5.0...` |
| `Content-Length` | Size of request body | `85` |

#### 3. Body (Optional)

The actual data being sent, typically for POST, PUT, and PATCH requests.

### Anatomy of an HTTP Response

```http
HTTP/1.1 201 Created                              ← Status Line
Content-Type: application/json                    ← Headers
Location: /api/jokes/42
Date: Fri, 24 Jan 2025 10:00:00 GMT
Content-Length: 95

{                                                 ← Body
  "id": 42,
  "text": "Chuck Norris can divide by zero.",
  "category": "math"
}
```

#### 1. Status Line

```
HTTP/version STATUS_CODE Reason_Phrase
```

- **Status Code**: Three-digit code indicating the result
- **Reason Phrase**: Human-readable description

#### 2. Response Headers

Similar to request headers, but from the server:

| Header | Purpose | Example |
|--------|---------|---------|
| `Content-Type` | Format of response body | `application/json` |
| `Location` | URL of created resource | `/api/jokes/42` |
| `Cache-Control` | Caching directives | `no-cache` |
| `Set-Cookie` | Set cookies on client | `session=abc123` |

#### 3. Body

The actual data being returned.

### HTTP Methods (Verbs)

HTTP defines several methods that indicate the desired action:

| Method | Purpose | Safe? | Idempotent? | Request Body? | Response Body? |
|--------|---------|-------|-------------|---------------|----------------|
| **GET** | Retrieve a resource | ✅ Yes | ✅ Yes | ❌ No | ✅ Yes |
| **POST** | Create a new resource | ❌ No | ❌ No | ✅ Yes | ✅ Yes |
| **PUT** | Replace a resource | ❌ No | ✅ Yes | ✅ Yes | ✅ Yes |
| **PATCH** | Partially update | ❌ No | ❌ No | ✅ Yes | ✅ Yes |
| **DELETE** | Remove a resource | ❌ No | ✅ Yes | ❌ No | ⚠️ Optional |
| **HEAD** | Get headers only | ✅ Yes | ✅ Yes | ❌ No | ❌ No |
| **OPTIONS** | Get allowed methods | ✅ Yes | ✅ Yes | ❌ No | ✅ Yes |

**Safe**: Doesn't modify server state (read-only)  
**Idempotent**: Multiple identical requests have the same effect as a single request

### HTTP Status Codes

Status codes indicate the result of the request. They're grouped into five classes:

#### 1xx: Informational

Rarely used in APIs. Examples:
- `100 Continue` - Server received request headers, client should send body

#### 2xx: Success

The request was successful:
- `200 OK` - General success (GET, PUT, PATCH)
- `201 Created` - Resource created (POST)
- `202 Accepted` - Request accepted for processing (async operations)
- `204 No Content` - Success with no response body (DELETE)

#### 3xx: Redirection

Further action needed:
- `301 Moved Permanently` - Resource permanently moved
- `302 Found` - Resource temporarily moved
- `304 Not Modified` - Cached version is still valid

#### 4xx: Client Error

The client made a mistake:
- `400 Bad Request` - Invalid request syntax or validation error
- `401 Unauthorized` - Authentication required or failed
- `403 Forbidden` - Authenticated but not authorized
- `404 Not Found` - Resource doesn't exist
- `405 Method Not Allowed` - HTTP method not supported for this resource
- `409 Conflict` - Request conflicts with current state (e.g., duplicate)
- `422 Unprocessable Entity` - Validation error (semantic error)
- `429 Too Many Requests` - Rate limit exceeded

#### 5xx: Server Error

The server encountered an error:
- `500 Internal Server Error` - Generic server error
- `501 Not Implemented` - Server doesn't support the functionality
- `502 Bad Gateway` - Invalid response from upstream server
- `503 Service Unavailable` - Server temporarily unavailable

### Common Mistakes with Status Codes

❌ **Don't do this:**
```csharp
// WRONG: Returning 200 for errors
app.MapGet("/api/jokes/{id}", (int id) => 
{
    return Results.Ok(new { error = "Joke not found" }); // 200 OK with error!
});
```

✅ **Do this:**
```csharp
// CORRECT: Use appropriate status codes
app.MapGet("/api/jokes/{id}", (int id, IJokeService service) => 
{
    var joke = service.GetById(id);
    return joke is null 
        ? Results.NotFound() 
        : Results.Ok(joke);
});
```

### HTTP Headers in Detail

Headers provide critical metadata. Here are the most important ones for APIs:

#### Request Headers

```http
GET /api/jokes/42 HTTP/1.1
Accept: application/json                    # Preferred response format
Accept-Language: en-US,en;q=0.9            # Preferred language
Accept-Encoding: gzip, deflate, br         # Supported compression
Authorization: Bearer eyJhbGc...           # Authentication
If-None-Match: "abc123"                    # Conditional request (caching)
User-Agent: MyClient/1.0                   # Client identification
```

#### Response Headers

```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8   # Response format
Content-Length: 234                              # Body size in bytes
Cache-Control: max-age=3600                      # Caching instructions
ETag: "abc123"                                   # Resource version
Last-Modified: Fri, 24 Jan 2025 10:00:00 GMT   # Last modification time
Access-Control-Allow-Origin: *                   # CORS policy
```

## REST Principles

**REST (Representational State Transfer)** is an architectural style for designing networked applications, introduced by Roy Fielding in his 2000 PhD dissertation.

### What REST is NOT

Before we define REST, let's clear up misconceptions:

- ❌ REST is **not** a protocol (like HTTP or SOAP)
- ❌ REST is **not** a standard with specifications
- ❌ REST is **not** just "using HTTP for APIs"
- ❌ REST is **not** the same as JSON APIs

### What REST IS

REST is a set of **architectural constraints** that, when applied together, create scalable, maintainable, and performant web services.

### The Six REST Constraints

#### 1. Client-Server Architecture

**Separation of concerns** between client and server:

```
┌─────────────────┐              ┌─────────────────┐
│     Client      │              │     Server      │
├─────────────────┤              ├─────────────────┤
│ • UI/UX         │◄────────────►│ • Data storage  │
│ • User state    │   HTTP/REST  │ • Business logic│
│ • Presentation  │              │ • Resources     │
└─────────────────┘              └─────────────────┘
```

**Benefits**:
- Independent evolution of client and server
- Multiple clients can use the same API
- Improved scalability

#### 2. Statelessness

Each request must contain **all information** needed to process it. The server doesn't store client context between requests.

```csharp
// ✅ CORRECT: Stateless - client sends authentication with each request
app.MapGet("/api/jokes", [Authorize] (IJokeService service) =>
{
    return service.GetAll(); // Auth token verified each time
});

// ❌ WRONG: Stateful - server remembers authentication
// Session state maintained on server
app.MapGet("/api/jokes", (IJokeService service, HttpContext context) =>
{
    if (context.Session.GetString("UserId") is null)
        return Results.Unauthorized();
    return Results.Ok(service.GetAll());
});
```

**Benefits**:
- Improved scalability (no server-side session storage)
- Reliability (no session state to lose)
- Easier load balancing

**Drawbacks**:
- Larger requests (authentication with each request)
- Client must manage state

#### 3. Cacheability

Responses must be labeled as **cacheable or non-cacheable**.

```csharp
app.MapGet("/api/jokes/{id}", (int id, IJokeService service) =>
{
    var joke = service.GetById(id);
    if (joke is null)
        return Results.NotFound();
    
    // Mark response as cacheable for 1 hour
    return Results.Ok(joke)
        .WithHeader("Cache-Control", "public, max-age=3600")
        .WithHeader("ETag", $"\"{joke.Version}\"");
});
```

**Benefits**:
- Reduced server load
- Improved client performance
- Reduced network traffic

#### 4. Uniform Interface

REST APIs should have a **consistent, standardized interface**. This is the most distinctive REST constraint and includes four sub-constraints:

##### a) Resource Identification

Resources are identified by **URIs** (Uniform Resource Identifiers):

```
✅ Good URIs:
/api/jokes              # Collection
/api/jokes/42           # Specific resource
/api/jokes/42/comments  # Sub-resource

❌ Bad URIs:
/api/getJokes           # Verb in URI
/api/joke?id=42         # ID as query parameter for specific resource
/api/jokesById/42       # Redundant naming
```

##### b) Resource Manipulation Through Representations

Clients manipulate resources through **representations** (typically JSON):

```json
{
  "id": 42,
  "text": "Chuck Norris can divide by zero.",
  "category": "math",
  "rating": 4.8
}
```

The representation is not the resource itself; it's a snapshot of the resource's state.

##### c) Self-Descriptive Messages

Each message includes enough information to describe how to process it:

```http
POST /api/jokes HTTP/1.1
Content-Type: application/json    # How to parse the body
Accept: application/json           # How to format the response

{
  "text": "Chuck Norris can divide by zero.",
  "category": "math"
}
```

##### d) Hypermedia as the Engine of Application State (HATEOAS)

Responses include **links** to related resources (advanced topic):

```json
{
  "id": 42,
  "text": "Chuck Norris can divide by zero.",
  "category": "math",
  "_links": {
    "self": { "href": "/api/jokes/42" },
    "update": { "href": "/api/jokes/42", "method": "PUT" },
    "delete": { "href": "/api/jokes/42", "method": "DELETE" },
    "comments": { "href": "/api/jokes/42/comments" }
  }
}
```

*Note: HATEOAS is often omitted in pragmatic REST APIs.*

#### 5. Layered System

The client doesn't know if it's connected directly to the server or through intermediaries:

```
Client ←→ Load Balancer ←→ Cache ←→ API Gateway ←→ Server
```

**Benefits**:
- Security (hide internal structure)
- Load balancing and caching layers
- Legacy system encapsulation

#### 6. Code on Demand (Optional)

Servers can extend client functionality by sending executable code (e.g., JavaScript). This constraint is **optional** and rarely used in modern REST APIs.

### RESTful Resource Design

Good REST API design centers around **resources** and **representations**.

#### Resource Naming Conventions

```
✅ Use nouns, not verbs:
GET    /api/jokes           # Get all jokes
POST   /api/jokes           # Create a joke
GET    /api/jokes/42        # Get joke 42
PUT    /api/jokes/42        # Replace joke 42
DELETE /api/jokes/42        # Delete joke 42

❌ Don't use verbs:
GET    /api/getJokes
POST   /api/createJoke
GET    /api/getJoke/42
POST   /api/updateJoke/42
POST   /api/deleteJoke/42
```

#### Collection vs. Instance

```
/api/jokes          # Collection (plural)
/api/jokes/42       # Instance (singular ID)
```

#### Nested Resources

```
✅ Logical nesting:
/api/jokes/42/comments       # Comments on joke 42
/api/users/alice/favorites   # Alice's favorites

❌ Excessive nesting:
/api/categories/funny/subcategories/tech/jokes/42/comments/5/replies
# Too deep! Keep it to 2-3 levels max
```

#### Query Parameters for Filtering/Sorting

```
GET /api/jokes?category=math              # Filter by category
GET /api/jokes?rating>=4.5                # Filter by rating
GET /api/jokes?sort=rating&order=desc     # Sort by rating descending
GET /api/jokes?page=2&pageSize=20         # Pagination
```

### REST Maturity Model (Richardson Maturity Model)

Not all APIs are equally RESTful. The Richardson Maturity Model defines four levels:

#### Level 0: The Swamp of POX (Plain Old XML)

Single URI, single HTTP method (usually POST):

```http
POST /api/endpoint
{
  "method": "getJoke",
  "id": 42
}
```

This is essentially RPC over HTTP, not REST.

#### Level 1: Resources

Multiple URIs, but still one HTTP method:

```http
POST /api/jokes/42
{ "method": "get" }
```

Better, but still not RESTful.

#### Level 2: HTTP Verbs

Multiple URIs + proper HTTP methods:

```http
GET    /api/jokes/42      # Retrieve
POST   /api/jokes         # Create
PUT    /api/jokes/42      # Update
DELETE /api/jokes/42      # Delete
```

**This is what most people mean by "REST API"**. It's pragmatic and widely used.

#### Level 3: Hypermedia Controls (HATEOAS)

Include links to related resources:

```json
{
  "id": 42,
  "text": "...",
  "_links": {
    "self": "/api/jokes/42",
    "comments": "/api/jokes/42/comments"
  }
}
```

True REST according to Fielding, but rarely implemented in practice.

### Pragmatic REST

In the real world, most successful APIs are **Level 2 REST**:

- ✅ Resource-based URIs
- ✅ HTTP methods used correctly
- ✅ Proper status codes
- ✅ Stateless
- ✅ Cacheable where appropriate
- ⚠️ HATEOAS often omitted

This is a practical balance between purity and usability.

## ASP.NET Core Overview

Now that you understand HTTP and REST, let's explore ASP.NET Core, Microsoft's modern framework for building web applications and APIs.

### What is ASP.NET Core?

**ASP.NET Core** is a cross-platform, high-performance, open-source framework for building modern, cloud-based, Internet-connected applications.

Key characteristics:

- **Cross-platform**: Runs on Windows, macOS, and Linux
- **Open source**: Developed on GitHub with community contributions
- **High performance**: One of the fastest web frameworks (see TechEmpower benchmarks)
- **Cloud-ready**: Designed for containerization and cloud deployment
- **Modular**: Pay-for-what-you-use architecture
- **Modern**: Uses latest C# language features

### A Brief History

```
2002  ASP.NET Web Forms         Windows-only, System.Web
2009  ASP.NET MVC               Better separation of concerns
2012  ASP.NET Web API           APIs and SPAs
2016  ASP.NET Core 1.0          Complete rewrite, cross-platform
2018  ASP.NET Core 2.1          LTS release
2019  ASP.NET Core 3.0          .NET Core only (no .NET Framework)
2020  ASP.NET Core 5.0          .NET unification (version jump)
2021  ASP.NET Core 6.0          LTS, Minimal APIs introduced
2022  ASP.NET Core 7.0          Performance improvements
2023  ASP.NET Core 8.0          LTS, Native AOT support
2024  ASP.NET Core 9.0          Enhanced Minimal APIs
2025  ASP.NET Core 10.0         Latest version (this course)
```

### Why ASP.NET Core?

#### 1. Performance

ASP.NET Core consistently ranks among the fastest web frameworks:

```
TechEmpower Benchmark (Plain Text - Round 22):
1. Rust (actix)           ~7M requests/sec
2. C++ (drogon)           ~7M requests/sec
3. ASP.NET Core           ~7M requests/sec  ← Here!
...
50. Node.js (fastify)     ~650K requests/sec
80. Python (flask)        ~180K requests/sec
```

#### 2. Developer Productivity

- IntelliSense and strong typing
- Hot reload (edit code while running)
- Minimal API syntax (less boilerplate)
- Rich ecosystem of libraries

#### 3. Cloud and Container Ready

- Small, self-contained deployments
- Docker support out of the box
- Kubernetes-friendly
- Aspire for cloud orchestration

#### 4. Modern Architecture

- Built-in dependency injection
- Configuration system
- Middleware pipeline
- Logging and diagnostics
- Health checks

### ASP.NET Core Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    ASP.NET Core Application                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐  │
│  │                   Middleware Pipeline                    │  │
│  │                                                           │  │
│  │  ┌───────────┐  ┌───────────┐  ┌───────────┐           │  │
│  │  │  Logging  │→ │   CORS    │→ │   Auth    │→ ...      │  │
│  │  └───────────┘  └───────────┘  └───────────┘           │  │
│  │                                              ↓           │  │
│  │                                        ┌──────────────┐  │  │
│  │                                        │   Endpoints  │  │  │
│  │                                        └──────────────┘  │  │
│  └─────────────────────────────────────────────────────────┘  │
│                                                                 │
│  ┌───────────────────┐      ┌──────────────────────────────┐  │
│  │  DI Container     │      │    Configuration             │  │
│  │                   │      │                              │  │
│  │  • Services       │      │  • appsettings.json          │  │
│  │  • Lifetimes      │      │  • Environment variables     │  │
│  │  • Scopes         │      │  • User secrets              │  │
│  └───────────────────┘      └──────────────────────────────┘  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
                              ↓
                    ┌─────────────────────┐
                    │    Kestrel Server   │
                    │  (Web Server)       │
                    └─────────────────────┘
```

### Key Components

#### 1. Kestrel Web Server

- Cross-platform web server
- High performance (written in C#)
- Can be used standalone or behind a reverse proxy (IIS, Nginx, Apache)

#### 2. Middleware Pipeline

- Request processing pipeline
- Each middleware can:
  - Handle the request
  - Pass it to the next middleware
  - Modify the response

#### 3. Dependency Injection

- Built-in IoC (Inversion of Control) container
- Services registered and injected automatically
- Manages object lifetimes

#### 4. Configuration

- Unified configuration system
- Multiple sources (JSON, environment variables, command line, etc.)
- Strongly-typed options pattern

#### 5. Routing

- Maps URLs to endpoints
- Pattern matching and constraints
- Parameter extraction

## The .NET 10 Web Stack

Let's understand how .NET 10 and ASP.NET Core fit together.

### The .NET Stack

```
┌─────────────────────────────────────────────────────────────┐
│                    Your Application                         │
│                  (ASP.NET Core, etc.)                       │
├─────────────────────────────────────────────────────────────┤
│                    .NET Libraries                           │
│         (ASP.NET Core, EF Core, HttpClient, etc.)          │
├─────────────────────────────────────────────────────────────┤
│                    .NET Runtime                             │
│              (CoreCLR, Native AOT, etc.)                   │
├─────────────────────────────────────────────────────────────┤
│              Operating System (Windows/Linux/macOS)         │
└─────────────────────────────────────────────────────────────┘
```

### .NET 10 Highlights

**.NET 10** (released November 2024) includes:

- **C# 13**: Latest language features
- **Performance improvements**: Faster startup, lower memory usage
- **Native AOT**: Compile to native code for faster startup and smaller size
- **Aspire enhancements**: Better cloud-native development experience
- **JSON improvements**: Faster serialization
- **Minimal API enhancements**: More features for building APIs

### ASP.NET Core 10 New Features

Relevant to this course:

1. **Enhanced Minimal APIs**: More intuitive syntax, better parameter binding
2. **Improved Dependency Injection**: Better lifetime management
3. **Built-in rate limiting**: Protect APIs from abuse
4. **Better OpenAPI support**: Automatic API documentation
5. **Aspire integration**: First-class support for cloud-native patterns

## Your First ASP.NET Core Application

Time to write some code! Let's create a minimal "Hello World" API.

### Prerequisites

Ensure you have:

```bash
# Check .NET version
dotnet --version
# Should be 10.0.x or higher
```

### Creating the Project

```bash
# Create a new minimal API project
dotnet new web -n HelloWorldApi

# Navigate to the project directory
cd HelloWorldApi

# Open in your IDE
code .  # VS Code
# or just open HelloWorldApi.csproj in Visual Studio
```

### Project Structure

```
HelloWorldApi/
├── HelloWorldApi.csproj    # Project file
├── Program.cs              # Application entry point
├── appsettings.json        # Configuration
└── Properties/
    └── launchSettings.json # Development settings
```

### The Program.cs File

Open `Program.cs`. With .NET 6+, there's no `Main` method or `Startup` class—it's all in one file:

```csharp
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

Let's understand each line:

#### 1. Create the Builder

```csharp
var builder = WebApplication.CreateBuilder(args);
```

This creates a `WebApplicationBuilder` that:
- Sets up configuration (appsettings.json, environment variables, etc.)
- Configures the dependency injection container
- Sets up logging
- Configures Kestrel web server

#### 2. Build the Application

```csharp
var app = builder.Build();
```

This builds the `WebApplication` instance, which:
- Contains the dependency injection container
- Contains the middleware pipeline
- Is used to configure routing and endpoints

#### 3. Define an Endpoint

```csharp
app.MapGet("/", () => "Hello World!");
```

This maps an HTTP GET request to the root URL (`/`) to a lambda function that returns "Hello World!".

#### 4. Run the Application

```csharp
app.Run();
```

This starts the web server and begins listening for HTTP requests.

### Running the Application

```bash
# Run the application
dotnet run

# Output:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: http://localhost:5000
# info: Microsoft.Hosting.Lifetime[0]
#       Application started. Press Ctrl+C to shut down.
```

Open a browser and navigate to `http://localhost:5000`. You should see "Hello World!".

### Adding More Endpoints

Let's add a few more endpoints:

```csharp
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Root endpoint
app.MapGet("/", () => "Hello World!");

// Greeting endpoint with parameter
app.MapGet("/greet/{name}", (string name) => $"Hello, {name}!");

// JSON response
app.MapGet("/api/status", () => new { 
    status = "OK", 
    timestamp = DateTime.UtcNow 
});

// Different HTTP methods
app.MapGet("/api/jokes", () => "Get all jokes");
app.MapPost("/api/jokes", () => "Create a joke");
app.MapPut("/api/jokes/{id}", (int id) => $"Update joke {id}");
app.MapDelete("/api/jokes/{id}", (int id) => $"Delete joke {id}");

app.Run();
```

### Testing with curl

```bash
# GET root
curl http://localhost:5000/
# Output: Hello World!

# GET with parameter
curl http://localhost:5000/greet/Alice
# Output: Hello, Alice!

# GET JSON
curl http://localhost:5000/api/status
# Output: {"status":"OK","timestamp":"2025-01-24T10:00:00.000Z"}

# POST (create)
curl -X POST http://localhost:5000/api/jokes
# Output: Create a joke

# PUT (update)
curl -X PUT http://localhost:5000/api/jokes/42
# Output: Update joke 42

# DELETE
curl -X DELETE http://localhost:5000/api/jokes/42
# Output: Delete joke 42
```

### Behind the Scenes

When you run the application, ASP.NET Core:

1. **Reads configuration** from appsettings.json, environment variables, etc.
2. **Sets up the DI container** with default services
3. **Configures Kestrel** to listen on port 5000 (HTTP) and 5001 (HTTPS)
4. **Builds the middleware pipeline** (minimal by default)
5. **Sets up routing** with your endpoint mappings
6. **Starts the server** and begins accepting requests

## Key Concepts Review

Let's review the key concepts from this lesson:

### HTTP Fundamentals

- ✅ HTTP is a stateless, request-response protocol
- ✅ Requests contain method, path, headers, and optional body
- ✅ Responses contain status code, headers, and optional body
- ✅ HTTP methods: GET (retrieve), POST (create), PUT (replace), DELETE (remove), PATCH (partial update)
- ✅ Status codes: 2xx (success), 4xx (client error), 5xx (server error)

### REST Principles

- ✅ REST is an architectural style, not a protocol
- ✅ Six constraints: client-server, stateless, cacheable, uniform interface, layered system, code on demand
- ✅ Resources identified by URIs (nouns, not verbs)
- ✅ HTTP methods operate on resources
- ✅ Level 2 REST is pragmatic and widely used

### ASP.NET Core

- ✅ Cross-platform, high-performance web framework
- ✅ Built on Kestrel web server
- ✅ Middleware pipeline for request processing
- ✅ Built-in dependency injection
- ✅ Unified configuration system
- ✅ Minimal API syntax for simple, readable code

## Exercises

### Exercise 1: HTTP Request Analysis (15 minutes)

Analyze the following HTTP request and answer the questions:

```http
POST /api/jokes HTTP/1.1
Host: api.chucknorris.io
Content-Type: application/json
Accept: application/json
Authorization: Bearer abc123
Content-Length: 67

{
  "text": "Chuck Norris can compile syntax errors.",
  "category": "programming"
}
```

**Questions:**

1. What HTTP method is being used?
2. What is the resource being accessed?
3. What format is the request body in?
4. What format does the client prefer for the response?
5. Is authentication included? How?
6. How large is the request body?

### Exercise 2: Status Code Selection (15 minutes)

Choose the appropriate HTTP status code for each scenario:

1. User successfully created a new joke
2. User tries to delete a joke that doesn't exist
3. User provides invalid data (missing required fields)
4. Server database is down
5. User successfully retrieved a joke
6. User tries to access an admin endpoint without authentication
7. User is authenticated but doesn't have permission to delete jokes
8. User tries to create a joke with a duplicate ID

### Exercise 3: REST API Design (30 minutes)

Design a RESTful API for a blog system with Posts, Comments, and Users. Define:

1. The resource URIs
2. The HTTP methods for each operation
3. The expected status codes
4. Example request/response for creating a comment on a post

### Exercise 4: Hello World API Enhancement (45 minutes)

Enhance the Hello World API with the following endpoints:

1. `GET /api/jokes` - Returns a list of hardcoded jokes
2. `GET /api/jokes/{id}` - Returns a specific joke by ID
3. `GET /api/jokes/random` - Returns a random joke
4. `GET /api/jokes/search?q={query}` - Searches jokes by text

**Sample jokes:**

```csharp
var jokes = new List<Joke>
{
    new(1, "Chuck Norris can divide by zero."),
    new(2, "Chuck Norris can compile syntax errors."),
    new(3, "Chuck Norris can access private methods."),
    new(4, "Chuck Norris doesn't use debuggers, debuggers use Chuck Norris."),
    new(5, "Chuck Norris's keyboard doesn't have a Ctrl key because nothing controls Chuck Norris.")
};

record Joke(int Id, string Text);
```

**Hints:**

- Use `Results.Ok()`, `Results.NotFound()`, etc. for proper status codes
- Use LINQ for searching: `jokes.Where(j => j.Text.Contains(query, StringComparison.OrdinalIgnoreCase))`
- Use `Random.Shared.Next()` for random selection

### Exercise 5: HTTP Response Practice (30 minutes)

Write the complete HTTP response (status line, headers, and body) for each scenario:

1. Successfully creating a joke (ID 123)
2. Failing to find a joke with ID 999
3. Validation error (text field is required but missing)
4. Successfully retrieving all jokes (return 2 jokes as JSON)

## Summary

In this lesson, you learned:

- ✅ **HTTP fundamentals**: requests, responses, methods, status codes, headers
- ✅ **REST principles**: architectural constraints, resource design, maturity model
- ✅ **ASP.NET Core overview**: architecture, benefits, key components
- ✅ **.NET 10 web stack**: how the pieces fit together
- ✅ **Your first API**: creating and running a minimal API

### Key Takeaways

1. **HTTP is the foundation**: Understanding HTTP is essential for web API development
2. **REST guides design**: RESTful principles create intuitive, scalable APIs
3. **Status codes matter**: Use them correctly to communicate results clearly
4. **ASP.NET Core is powerful**: Modern, fast, and cross-platform
5. **Minimal APIs are simple**: Less boilerplate, more productivity

### What's Next?

In the next lesson, [02. Minimal APIs](02_minimal_apis.md), you'll dive deeper into:

- Creating full CRUD APIs
- Advanced routing patterns
- Parameter binding from different sources
- Response formatting
- Error handling

Ready to write more code? Let's continue! 🚀
