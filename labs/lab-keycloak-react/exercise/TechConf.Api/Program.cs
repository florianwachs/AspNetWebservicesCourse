using Scalar.AspNetCore;
using TechConf.Api.Data;
using TechConf.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.AddNpgsqlDbContext<AppDbContext>("eventdb");

// TODO: Task 3 — Add JWT Bearer authentication
// Hint: add `using Microsoft.AspNetCore.Authentication.JwtBearer;` and then:
// var keycloakHttpEndpoint = builder.Configuration["KEYCLOAK_HTTP"]
//     ?? builder.Configuration["services:keycloak:http:0"]
//     ?? throw new InvalidOperationException("Missing Keycloak HTTP endpoint configuration.");
// var keycloakAuthority = $"{keycloakHttpEndpoint.TrimEnd('/')}/realms/techconf";
//
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = keycloakAuthority;
//         options.Audience = "techconf-api";
//         options.MapInboundClaims = false;
//
//         if (builder.Environment.IsDevelopment())
//         {
//             options.RequireHttpsMetadata = false;
//         }
//
//         options.TokenValidationParameters.RoleClaimType = "roles";
//     });

// TODO: Task 3 — Add authorization policies for "Organizer", "Speaker", and "Attendee" roles
// Hint: builder.Services.AddAuthorizationBuilder()
//     .AddPolicy("Organizer", p => p.RequireRole("organizer"))
//     ...

// TODO: Task 5 — Add CORS policy to allow the React frontend to call the API
// Hint: builder.Services.AddCors(options => { ... });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();

// TODO: Task 3 — Enable authentication and authorization middleware
// Hint: app.UseAuthentication();
//       app.UseAuthorization();

// TODO: Task 5 — Enable CORS middleware
// Hint: app.UseCors("AllowFrontend");

app.MapEventEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
