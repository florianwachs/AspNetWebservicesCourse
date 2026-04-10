using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TechConf.Api.Data;
using TechConf.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var useJwtAuth = string.Equals(builder.Configuration["Authentication:Mode"], "Jwt", StringComparison.OrdinalIgnoreCase);

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

if (useJwtAuth)
{
    var issuer = builder.Configuration["Jwt:Issuer"]
        ?? throw new InvalidOperationException("Missing Jwt:Issuer configuration.");
    var audience = builder.Configuration["Jwt:Audience"]
        ?? throw new InvalidOperationException("Missing Jwt:Audience configuration.");
    var signingKey = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("Missing Jwt:Key configuration.");

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });
}
else
{
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
}

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Organizer", p => p.RequireRole("organizer"))
    .AddPolicy("Speaker", p => p.RequireRole("speaker", "organizer"))
    .AddPolicy("Attendee", p => p.RequireRole("attendee", "speaker", "organizer"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints(useJwtAuth);
app.MapEventEndpoints();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.EnsureCreatedAsync();
await SeedData.InitializeAsync(scope.ServiceProvider);

app.Run();
