using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;
using TechConf.Api.Data;
using TechConf.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var keycloakHttpEndpoint = builder.Configuration["KEYCLOAK_HTTP"]
    ?? builder.Configuration["services:keycloak:http:0"]
    ?? throw new InvalidOperationException("Missing Keycloak HTTP endpoint configuration.");
var keycloakAuthority = $"{keycloakHttpEndpoint.TrimEnd('/')}/realms/techconf";

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.AddNpgsqlDbContext<AppDbContext>("eventdb");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthority;
        options.Audience = "techconf-api";
        options.MapInboundClaims = false;

        if (builder.Environment.IsDevelopment())
        {
            options.RequireHttpsMetadata = false;
        }

        options.TokenValidationParameters.RoleClaimType = "roles";
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Organizer", p => p.RequireRole("organizer"))
    .AddPolicy("Speaker", p => p.RequireRole("speaker"))
    .AddPolicy("Attendee", p => p.RequireRole("attendee"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();

app.MapEventEndpoints();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.EnsureCreatedAsync();

app.Run();
