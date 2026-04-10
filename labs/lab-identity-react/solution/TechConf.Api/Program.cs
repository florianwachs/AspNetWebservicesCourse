using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TechConf.Api.Data;
using TechConf.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("eventdb")));

// ASP.NET Identity with cookie authentication
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

// ──────────────────────────────────────────────────────────────────────
// AUTH MODE: Cookie (default) vs JWT
// The lab uses cookie auth by default. To switch to JWT bearer tokens:
//   1. Comment out the ConfigureApplicationCookie block below.
//   2. Uncomment the JWT bearer block that follows.
//   3. In the React frontend, switch to the JWT helpers in auth.ts / events.ts / AuthProvider.tsx
//      (see the "JWT MODE" comments in those files).
// ──────────────────────────────────────────────────────────────────────

// ── Cookie mode (active) ────────────────────────────────────────────
// Configure cookie to return 401/403 JSON responses instead of redirecting
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

// ── JWT mode (uncomment to switch) ──────────────────────────────────
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme =
//     options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "https://techconf.dev",
//         ValidAudience = builder.Configuration["Jwt:Audience"] ?? "techconf-api",
//         IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
//             System.Text.Encoding.UTF8.GetBytes(
//                 builder.Configuration["Jwt:Key"] ?? "super-secret-key-for-dev-only-min-32-chars!!"))
//     };
// });

// Authorization policies based on roles
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("OrganizerPolicy", p => p.RequireRole("Organizer"))
    .AddPolicy("SpeakerPolicy", p => p.RequireRole("Speaker", "Organizer"))
    .AddPolicy("AttendeePolicy", p => p.RequireRole("Attendee", "Speaker", "Organizer"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapEventEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

app.Run();
