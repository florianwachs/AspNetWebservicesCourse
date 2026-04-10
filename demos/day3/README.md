# Day 3 Demos — Aspire, Auth & Architecture

## Runnable samples

This folder now contains two runnable reference solutions:

### Demos 1-4 sample: Aspire + ASP.NET Identity + JWT/Cookie switch

```bash
cd demos/day3/TechConf.Day3.AuthDemo/TechConf.AppHost
aspire secret set Parameters:postgres-password DevOnlyPassword123!
dotnet run
```

Useful files:

- Solution: `TechConf.Day3.AuthDemo.slnx`
- AppHost: `TechConf.Day3.AuthDemo/TechConf.AppHost`
- API requests: `TechConf.Day3.AuthDemo/TechConf.Api/requests.http`
- Auth mode switch: `TechConf.Day3.AuthDemo/TechConf.Api/appsettings.json` (`Authentication:Mode = Cookie` or `Jwt`)
- Seed users: `admin@techconf.dev` / `Admin123!`, `speaker@techconf.dev` / `Speaker123!`, `attendee@techconf.dev` / `Attendee123!`
- If PostgreSQL was previously started with an older random password, remove the persisted volume once: `docker volume rm day3-auth-demo-postgres`

### Demo 5 sample: Aspire + Keycloak

```bash
cd demos/day3/TechConf.Day3.KeycloakDemo/TechConf.AppHost
aspire secret set Parameters:postgres-password DevOnlyPassword123!
dotnet run
```

Useful files:

- Solution: `demos/day3/TechConf.Day3.KeycloakDemo.slnx`
- AppHost: `TechConf.Day3.KeycloakDemo/TechConf.AppHost`
- Realm import: `TechConf.Day3.KeycloakDemo/TechConf.AppHost/Realms/techconf-realm.json`
- API requests: `TechConf.Day3.KeycloakDemo/TechConf.Api/requests.http`
- Imported client: `techconf-api` / `techconf-api-secret`
- Imported users: `admin@techconf.dev` / `Admin123!` (Organizer), `speaker@techconf.dev` / `Speaker123!`, `attendee@techconf.dev` / `Attendee123!`
- If PostgreSQL was previously started with an older random password, remove the persisted volume once: `docker volume rm day3-keycloak-demo-postgres`
- If realm changes do not re-apply, remove the persisted Keycloak volume: `docker volume rm day3-keycloak-demo-keycloak`

Open the Aspire Dashboard to inspect resources, console logs, structured logs, and traces.

## Demo 1: Aspire from Scratch (20 min)

1. Create Aspire starter: `dotnet new aspire-starter -n TechConfDemo`
2. Walk through the generated solution structure
3. Show AppHost `Program.cs` — explain `AddProject`, `AddPostgres`, `AddRedis`
4. Run AppHost and explore the Dashboard:
   - Resources tab (health status)
   - Console logs (aggregated)
   - Structured logs (searchable)
   - Traces (distributed)
5. Show `WaitFor()` — stop the database container and watch API wait
6. Show automatic connection string injection

## Demo 2: ASP.NET Identity Setup (15 min)

1. Add ASP.NET Identity to the API project
2. Configure Identity with EF Core stores
3. Add roles: organizer, speaker, attendee
4. Seed test users and assign roles
5. Expose register / login endpoints
6. Show Identity tables in the database
7. Explain built-in user management vs external IdP

## Demo 3: JWT Auth in API (10 min)

1. Add `AddAuthentication().AddJwtBearer()`
2. Configure issuer, audience, and signing key
3. Issue a JWT from the login endpoint with role claims
4. Add `UseAuthentication()` / `UseAuthorization()`
5. Show `RequireAuthorization("Organizer")` on an endpoint
6. Call endpoint without token → 401 / with wrong role → 403
7. Call with correct role → 200

## Demo 4: Switch from JWT to Cookie Auth (15 min)

1. Start from the JWT-based version of the API
2. Replace `AddJwtBearer()` with `AddCookie()`
3. Update login to sign in with Identity cookies
4. Keep authorization policies and role checks unchanged
5. Show browser flow with automatic cookie handling
6. Call endpoint without login → 401 / after login with wrong role → 403
7. Explain when cookies fit better than bearer tokens

## Demo 5: Start the Same App with Keycloak (15 min)

1. Start the Keycloak-backed version of the app in Aspire
2. Show the Keycloak AppHost setup with `.WithRealmImport(...)`
3. Access the Keycloak admin console
4. Inspect the imported `techconf` realm and `techconf-api` client
5. Show the imported organizer, speaker, and attendee users
6. Get a token from Keycloak and call the secured endpoint
7. Compare external identity provider flow vs ASP.NET Identity
