# Alternative Lab: ASP.NET Identity with Aspire and React

## Overview

This is the **simpler Day 3 authentication path**. In this lab you will build a **full-stack authenticated application** using [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity) as the identity provider, **.NET Aspire** for orchestration, a **Minimal API** backend protected with cookie authentication and role-based authorization, and a **React** single-page application with built-in login/register forms.

Unlike the advanced Keycloak/JWT lab, this lab uses **ASP.NET Identity** — a built-in membership system that stores users, passwords, and roles directly in your PostgreSQL database via Entity Framework Core. No external identity provider is needed, and the login flow stays inside your own application.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) running (required for PostgreSQL)
- [Node.js LTS](https://nodejs.org/) installed (required for the React frontend)
- Basic knowledge of C#, React, and authentication concepts

## Learning Objectives

1. Orchestrate PostgreSQL and a React frontend using .NET Aspire.
2. Configure ASP.NET Core Identity with `IdentityUser`, roles, and EF Core on PostgreSQL.
3. Protect Minimal API endpoints with cookie authentication and role-based authorization policies.
4. Build authentication API endpoints (register, login, logout, user info).
5. Implement a React login/register form that authenticates via cookies (no OIDC library needed).
6. Use role-based permissions to control UI and API access (Organizer, Speaker, Attendee).

## How This Fits into Day 3

- Choose **this lab** if you want the simpler app-local authentication path: **ASP.NET Identity + cookies + first-party login UI**.
- Choose **[Lab 3 — Aspire, Auth & Architecture](../lab3-aspire-auth-architecture/)** if you want the advanced path: **Keycloak + JWT + centralized auth + VSA/MediatR**.

## Key Differences from the Keycloak Lab

| Aspect | Keycloak Lab | This Lab (ASP.NET Identity) |
|---|---|---|
| Identity Provider | External Keycloak server | Built-in ASP.NET Identity |
| Auth Mechanism | JWT Bearer tokens (OIDC) | Cookie authentication |
| Frontend Auth | `react-oidc-context` library | Custom login form + fetch with cookies |
| User Storage | Keycloak database | PostgreSQL via EF Core (`IdentityDbContext`) |
| Infrastructure | Keycloak + PostgreSQL containers | PostgreSQL container only |
| Token Handling | `Authorization: Bearer <token>` header | Cookies sent automatically |

## Getting Started

### 1. Install frontend dependencies

```bash
cd exercise/techconf-frontend
npm install
```

### 2. Start the Aspire AppHost

```bash
cd exercise/TechConf.AppHost
dotnet run
```

Open the **Aspire Dashboard** (link is in the output) to monitor all services.

If you are using an IDE it should pop right up. There are also extensions for most IDEs.

### 3. Seed Users

The application automatically seeds three test users on startup:

| Email | Password | Role |
|---|---|---|
| `admin@techconf.dev` | `Admin123!` | Organizer |
| `speaker@techconf.dev` | `Speaker123!` | Speaker |
| `attendee@techconf.dev` | `Attendee123!` | Attendee |

## Tasks

### Task 1 — Orchestrate PostgreSQL with Aspire

Open `TechConf.AppHost/Program.cs` and add PostgreSQL with a database, then wire up the API project:

```csharp
var postgres = builder.AddPostgres("postgres").AddDatabase("eventdb");

var api = builder.AddProject<Projects.TechConf_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres);
```

Run the AppHost and verify PostgreSQL starts in the Aspire Dashboard.

### Task 2 — Configure ASP.NET Identity with seed data

1. Open `TechConf.Api/Program.cs` and configure Identity:

```csharp
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
```

Note that `AppDbContext` already extends `IdentityDbContext`, which provides the tables for users, roles, and claims.

2. Open `TechConf.Api/Data/SeedData.cs` and implement role and user seeding. Create three roles (Organizer, Speaker, Attendee) and three test users.

3. Call `SeedData.InitializeAsync()` in `Program.cs` after `EnsureCreated()`.

### Task 3 — Configure cookie auth and authorization policies

1. Configure the Identity cookie to return JSON status codes instead of redirecting:

```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});
```

2. Add authorization policies that map to roles:

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("OrganizerPolicy", p => p.RequireRole("Organizer"))
    .AddPolicy("SpeakerPolicy", p => p.RequireRole("Speaker", "Organizer"))
    .AddPolicy("AttendeePolicy", p => p.RequireRole("Attendee", "Speaker", "Organizer"));
```

3. Enable the middleware and add `.RequireAuthorization("OrganizerPolicy")` to the `POST`, `PUT`, and `DELETE` event endpoints.

### Task 4 — Build authentication API endpoints

Open `TechConf.Api/Endpoints/AuthEndpoints.cs` and implement:

- **POST `/api/auth/register`** — Create a new user with the `Attendee` role.
- **POST `/api/auth/login`** — Sign in using `SignInManager.PasswordSignInAsync()` (sets the auth cookie).
- **POST `/api/auth/logout`** — Sign out (clears the cookie). Requires authorization.
- **GET `/api/auth/me`** — Return the current user's ID, email, and roles from `ClaimsPrincipal`. Requires authorization.

Don't forget to call `app.MapAuthEndpoints()` in `Program.cs`.

### Task 5 — Build the React auth provider and login form

1. **Implement the API functions** in `src/api/auth.ts` — `login()`, `register()`, and `getMe()` using `fetch` with `credentials: "include"`.

2. **Implement `AuthProvider`** in `src/auth/AuthProvider.tsx` — use React Context to provide user state, with `refreshUser()` calling `getMe()` and `logout()` calling the logout endpoint.

3. **Wrap the app** with `AuthProvider` in `src/main.tsx`.

4. **Update the Navbar** to show the user email, roles, and a logout button when authenticated.

5. **Complete the LoginForm** to call `login()`/`register()` and then `refreshUser()`.

### Task 6 — Wire up authenticated event management

1. **Implement `createEvent` and `deleteEvent`** in `src/api/events.ts` — use `credentials: "include"` (no `Authorization` header needed with cookies).

2. **Complete `CreateEventForm.tsx`** so it calls `createEvent()`, clears the form, and invokes `onCreated()` after a successful submission.

3. **Update `EventList.tsx`** to:
   - Show the `LoginForm` when not authenticated
   - Show `CreateEventForm` only for users with the `Organizer` role
   - Show delete buttons only for Organizers

4. **Add the React frontend to the Aspire AppHost**:

```csharp
builder.AddNpmApp("frontend", "../techconf-frontend")
    .WithReference(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();
```

5. **Add CORS** to the API in `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ... after building the app:
app.UseCors("AllowFrontend");
```

> **Important**: Use `.AllowCredentials()` (not `.AllowAnyOrigin()`) so that cookies are sent cross-origin.

## Switching to JWT Bearer Tokens

The lab uses **cookie authentication** by default, but all files contain commented-out code to switch to **JWT bearer tokens**. This lets you compare both approaches side-by-side.

### How to switch

| File | What to change |
|---|---|
| `TechConf.Api/Program.cs` | Comment out `ConfigureApplicationCookie` block → uncomment `AddAuthentication` / `AddJwtBearer` block |
| `TechConf.Api/Endpoints/AuthEndpoints.cs` | Uncomment the `POST /token` endpoint (issues a JWT) |
| `techconf-frontend/src/api/auth.ts` | Comment out cookie functions → uncomment JWT functions (`getToken`, `clearToken`, token-based `login`) |
| `techconf-frontend/src/api/events.ts` | Comment out cookie functions → uncomment JWT functions (uses `Authorization: Bearer` header) |
| `techconf-frontend/src/components/CreateEventForm.tsx` | Keep using `createEvent()`; no JWT-specific changes needed here |
| `techconf-frontend/src/auth/AuthProvider.tsx` | Comment out cookie `AuthProvider` → uncomment JWT `AuthProvider` (uses `clearToken` for logout) |

> **Key difference**: Cookie auth sends credentials automatically via `credentials: "include"`. JWT auth stores the token in memory and sends it explicitly via the `Authorization: Bearer <token>` header.

## Stretch Goals

1. **Admin dashboard** — Create an admin page that lists all users and their roles. Only Organizers can access it. Use `UserManager<IdentityUser>` to query users.
2. **Role management** — Add an API endpoint that lets Organizers promote/demote users between roles.
3. **Custom claims** — Add a custom claim (e.g., `Department`) to users during registration and display it in the UI.
4. **Account lockout** — Enable account lockout after 5 failed login attempts using `lockoutOnFailure: true` and configure the lockout duration.
5. **Persistent JWT** — Store the JWT in `localStorage` so the user stays logged in across page refreshes (note the security trade-offs vs. in-memory storage).

## Solution

A fully working solution is available in the `solution/` directory.
