using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PoliciesWithSimpleToken.Auth;
using PoliciesWithSimpleToken.DataAccess;
using PoliciesWithSimpleToken.Domain;
using PoliciesWithSimpleToken.Endpoints;
using PoliciesWithSimpleToken.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Bearer Authentication",
        Description = "Enter your Bearer token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "Microsoft .NET"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddSingleton<DataProvider>();

// Configure Auth
builder.Services.AddSingleton<IAuthorizationHandler, AllowedToReadAboutChuckNorrisRequirementHandler>();
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy(AuthConstants.Policies.Admin, p => p.RequireClaim(AuthConstants.ClaimTypes.IsAdmin));
    
    o.AddPolicy(AuthConstants.Policies.AllowedToReadChuckNorrisBooks, p=> p.AddRequirements(new AllowedToReadAboutChuckNorrisRequirement()));

    o.AddPolicy(AuthConstants.Policies.CanDeleteAuthor, p => p.RequireClaim(AuthConstants.ClaimTypes.IsContentManager));
});

// Identity Services konfigurieren
builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<AppClaimsFactory>();

// DB konfigurieren
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite("Data Source=app.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapIdentityApi<AppUser>();
app.MapAuthors();

await EnsureMigratedDb(app);
await EnsureUsers(app);

app.Run();

async Task EnsureMigratedDb(WebApplication wapp)
{
    using var scope = wapp.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

async Task EnsureUsers(WebApplication wapp)
{
    using var scope = wapp.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    if (userManager.Users.Any())
    {
        return;
    }

    var admin = new AppUser()
    {
        IsAdmin = true,
        Age = 18,
        UserName = "admin@test.de",
        Email = "admin@test.de",
        EmailConfirmed = true,
    };
    
    var alice = new AppUser()
    {
        IsAdmin = false,
        Age = 40,
        UserName = "alice@test.de",
        Email = "alice@test.de",
        EmailConfirmed = true,
    };
    
    var chucky = new AppUser()
    {
        IsAdmin = false,
        Age = 3,
        UserName = "chucky@test.de",
        Email = "chucky@test.de",
        EmailConfirmed = true,
    };
    
    var contentManager = new AppUser()
    {
        IsAdmin = false,
        Age = 3,
        UserName = "content@test.de",
        Email = "content@test.de",
        EmailConfirmed = true,
        IsContentManager = true,
    };
    
    var result = await userManager.CreateAsync(admin, "Test123Test123!");
    result = await userManager.CreateAsync(alice, "Test123Test123!");
    result = await userManager.CreateAsync(chucky, "Test123Test123!");
    result = await userManager.CreateAsync(contentManager, "Test123Test123!");
}