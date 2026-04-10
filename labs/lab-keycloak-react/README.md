# Lab: Keycloak Authentication with Aspire and React

## Overview

In this lab you will build a **full-stack authenticated application** using [Keycloak](https://www.keycloak.org/) as the identity provider, **.NET Aspire** for orchestration, a **Minimal API** backend protected with JWT Bearer tokens, and a **React** single-page application that lets users sign in via OpenID Connect and manage TechConf events. The lab now includes a checked-in Keycloak realm import so the starter and solution both use the same reproducible realm, clients, roles, and sample users.

**Scaffold Level**: Level 2 (50–60 % starter code provided)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) running (required for Keycloak and PostgreSQL)
- [Node.js LTS](https://nodejs.org/) installed (required for the React frontend)
- Basic knowledge of C#, React, and authentication concepts

## Learning Objectives

1. Orchestrate Keycloak, PostgreSQL, and a React frontend using .NET Aspire.
2. Inspect and adapt an imported Keycloak realm, clients, and roles for an SPA.
3. Protect ASP.NET Core Minimal API endpoints with JWT Bearer authentication and role-based authorization.
4. Integrate a React application with Keycloak using OpenID Connect (`react-oidc-context`).
5. Pass JWT access tokens from the React frontend to the API for authenticated requests.
6. Manage the full authentication lifecycle: login, logout, and token refresh.

## Getting Started

### 1. Install frontend dependencies

```bash
cd exercise/techconf-frontend
npm install
```

### 2. Set a stable PostgreSQL password

From the AppHost directory, set the password once for the `postgres` resource:

```bash
cd exercise/TechConf.AppHost
aspire secret set Parameters:postgres-password DevOnlyPassword123!
```

This stores the value in the **AppHost user secrets**. You do not need to run `dotnet user-secrets` separately unless you prefer managing the same secret that way.

### 3. Start the Aspire AppHost

```bash
dotnet run
```

Open the **Aspire Dashboard** (usually at `https://localhost:15888`) to monitor all services.

### 4. Imported Keycloak setup

Once Keycloak is running (check the Aspire Dashboard), the `techconf` realm is already imported for you.

- Open the **Keycloak** resource URL from the Aspire Dashboard if you want to inspect the setup.
- The admin console still uses the bootstrap admin user in the **`master`** realm with the credentials shown by Aspire.
- The imported lab realm already includes:
  - realm: `techconf`
  - API client: `techconf-api`
  - SPA client: `techconf-frontend`
  - roles: `organizer`, `speaker`, `attendee`
  - sample users:
    - `admin@techconf.dev` / `Admin123!`
    - `speaker@techconf.dev` / `Speaker123!`
    - `attendee@techconf.dev` / `Attendee123!`
- The imported `techconf-frontend` SPA client already allows the dynamic localhost ports that Aspire/Vite use. Its redirect URIs stay scoped to localhost, and its web origins use `*` so Keycloak accepts the SPA's random dev port for the token exchange.

> If Keycloak state looks stale, imported users cannot log in, you see `client not found`, `invalid redirect_uri`, or a token-endpoint CORS error in the browser, or the admin password in the dashboard no longer works, delete the persisted Keycloak volume and restart the AppHost:
>
> ```bash
> docker volume rm lab-keycloak-react-keycloak
> ```
>
> If you ran an earlier version of the lab or the Day 3 Keycloak demo before this fix, you may also want to remove the old shared volume once:
>
> ```bash
> docker volume rm techconf-keycloak
> ```

## Tasks

### Task 1 — Orchestrate PostgreSQL with Aspire

Open `TechConf.AppHost/Program.cs` and add PostgreSQL with a stable password, a named data volume, and a database:

```csharp
var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume("lab-keycloak-react-postgres")
    .AddDatabase("eventdb");
```

Set the secret once from the AppHost directory:

```bash
aspire secret set Parameters:postgres-password DevOnlyPassword123!
```

If you already created the `lab-keycloak-react-postgres` volume with an older random password and PostgreSQL now shows `password authentication failed`, remove that volume once and start again:

```bash
docker volume rm lab-keycloak-react-postgres
```

### Task 2 — Add Keycloak to the Aspire AppHost

In the same file, add Keycloak with a named persistent data volume, import the checked-in realm file, and wire up the API project:

```csharp
const string keycloakVolumeName = "lab-keycloak-react-keycloak";
var keycloakRealmImportPath = Path.Combine(AppContext.BaseDirectory, "Realms");

var keycloak = builder.AddKeycloak("keycloak")
    .WithDataVolume(keycloakVolumeName)
    .WithRealmImport(keycloakRealmImportPath);

var api = builder.AddProject<Projects.TechConf_Api>("api")
    .WithReference(postgres)
    .WithReference(keycloak)
    .WaitFor(postgres)
    .WaitFor(keycloak);
```

The realm JSON already exists in both `exercise/TechConf.AppHost/Realms/` and `solution/TechConf.AppHost/Realms/`; in the exercise you still need to wire it into the AppHost code.

### Task 3 — Protect the API with JWT Bearer authentication

Open `TechConf.Api/Program.cs` and configure authentication and authorization:

```csharp
var keycloakHttpEndpoint = builder.Configuration["KEYCLOAK_HTTP"]
    ?? builder.Configuration["services:keycloak:http:0"]
    ?? throw new InvalidOperationException("Missing Keycloak HTTP endpoint configuration.");
var keycloakAuthority = $"{keycloakHttpEndpoint.TrimEnd('/')}/realms/techconf";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthority;
        options.Audience = "techconf-api";
        options.MapInboundClaims = false;
        options.RequireHttpsMetadata = false; // Development only
        options.TokenValidationParameters.RoleClaimType = "roles";
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Organizer", p => p.RequireRole("organizer"))
    .AddPolicy("Speaker", p => p.RequireRole("speaker"))
    .AddPolicy("Attendee", p => p.RequireRole("attendee"));
```

Enable the middleware:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

### Task 4 — Apply authorization policies to endpoints

Open `TechConf.Api/Endpoints/EventEndpoints.cs` and add `.RequireAuthorization("Organizer")` to the `POST`, `PUT`, and `DELETE` endpoints. The `GET` endpoints should remain publicly accessible.

### Task 5 — Connect the React frontend to Keycloak

1. **Configure the OIDC provider** in `techconf-frontend/src/auth/AuthProvider.tsx`:

```tsx
const oidcConfig = {
  authority: keycloakAuthority,
  client_id: clientId,
  redirect_uri: window.location.origin,
  post_logout_redirect_uri: window.location.origin,
  scope: "openid profile email",
  onSigninCallback: () => {
    window.history.replaceState({}, document.title, window.location.pathname);
  },
};
```

The frontend authority is injected from Aspire via the AppHost/Keycloak reference and the Vite config, so `keycloakAuthority` should resolve to the actual Keycloak URL instead of relying on `http://localhost:8080`. Clear the callback payload from the URL after sign-in so the SPA does not keep re-processing the `code` and `state` query parameters.

2. **Wrap the app** with `AuthProvider` in `src/main.tsx`.

3. **Update the Navbar** in `src/components/Navbar.tsx` to show login/logout buttons using the `useAuth()` hook.

### Task 6 — Wire up authenticated API calls

1. **Implement `createEvent` and `deleteEvent`** in `src/api/events.ts` — include the JWT token in the `Authorization: Bearer <token>` header.

2. **Update `EventList.tsx`** to pass the access token when creating or deleting events, and only show the create form and delete buttons when the user is authenticated.

3. **Add the React frontend to the Aspire AppHost**:

```csharp
var webfrontend = builder.AddViteApp("webfrontend", "../techconf-frontend")
    .WithReference(api)
    .WithReference(keycloak)
    .WaitFor(api)
    .WaitFor(keycloak);

api.PublishWithContainerFiles(webfrontend, "wwwroot");
```

4. **Add CORS** to the API in `Program.cs` so the frontend can call it:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ... after building the app:
app.UseCors("AllowFrontend");
```

## Stretch Goals

1 **Role-based UI** — Parse the access token claims in the React app and only show admin actions (create/delete) to users with the `organizer` role.
2 **User profile page** — Add a `/profile` route that displays the authenticated user's Keycloak profile information (name, email, roles).
3 **Realm customization** — Extend the imported realm JSON with extra roles, users, or a second frontend client for another app.
4 ** compare with Cookies — Implement a version of the frontend that uses cookie-based authentication with Keycloak's `keycloak-js` adapter and compare the differences in setup, token handling, and API protection.

## Solution

A fully working solution is available in the `solution/` directory.
