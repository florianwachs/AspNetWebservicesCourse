using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using ReactAppWithAuth1.Data;
using ReactAppWithAuth1.Infrastructure;
using ReactAppWithAuth1.Models;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);

var app = builder.Build();
await SeedDb(app.Services);
ConfigurePipeline(app);

app.Run();


void ConfigureServices(WebApplicationBuilder builder)
{
    var services = builder.Services;
    services.AddCors(options => options.AddDefaultPolicy(o => o.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()));
    services.AddSpaStaticFiles(configuration =>
    {
        configuration.RootPath = "ClientApp/build";
    });

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection")));

    services.AddDatabaseDeveloperPageExceptionFilter();

    services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();


    services.AddIdentityServer()
        .AddApiAuthorization<ApplicationUser, ApplicationDbContext>()
        .AddProfileService<ProfileService>();

    services.Configure<IdentityOptions>(options =>
    {
        // Hier können die Standard Passwort Regeln überschrieben werden
        //options.Password.RequireDigit = true;
        //options.Password.RequireLowercase = true;
        //options.Password.RequireNonAlphanumeric = true;
        //options.Password.RequireUppercase = true;
        //options.Password.RequiredLength = 6;
        //options.Password.RequiredUniqueChars = 1;
    });

    services.AddSingleton<IAuthorizationHandler, CanReadTempRequirementHandler>();

    services.AddAuthorization(options =>
    {
        options.AddPolicy(AppPolicies.CanReadWeather, p => p.RequireClaim(CanReadWeatherClaim.Type));
        options.AddPolicy(AppPolicies.CanAddWeather, p => p.RequireClaim(CanAddWeatherClaim.Type));
        options.AddPolicy(AppPolicies.CanReadTemp, p => p.AddRequirements(new CanReadTempRequirement()));
    });

    services.AddAuthentication()
        .AddIdentityServerJwt();

    services.AddControllersWithViews();
    services.AddRazorPages();
}

void ConfigurePipeline(WebApplication app)
{
    var env = app.Environment;

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseCors();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseIdentityServer();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");
        endpoints.MapRazorPages();
    });

    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
            spa.UseReactDevelopmentServer(npmScript: "start");
        }
    });
}

async Task SeedDb(IServiceProvider provider)
{
    using var scope = provider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    //await dbContext.Database.EnsureCreatedAsync();
    await dbContext.Database.MigrateAsync();


    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    ApplicationUser[] defaultUsers =
    {
            new ApplicationUser() { Id = "user1@test.de", UserName = "user1@test.de", Email = "user1@test.de" },
            new ApplicationUser() { Id = "pay@test.de", UserName = "pay@test.de", Email = "pay@test.de" },
            new ApplicationUser() { Id = "admin1@test.de", UserName = "admin1@test.de", Email = "admin1@test.de", IsAdmin = true }
        };

    foreach (var user in defaultUsers)
    {
        var existingUser = await userManager.FindByIdAsync(user.Id);
        if (existingUser is not null)
        {
            await userManager.DeleteAsync(existingUser);
        }

        var result = await userManager.CreateAsync(user, "Test@Test123");
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to create user " + result.ToString());
        }

        // Claims können direkt am User hinterlegt werden oder dynamisch im ProfileService
        result = await userManager.AddClaimsAsync(user, AppClaims.GetUserClaims());

        if (user.IsAdmin)
        {
            result = await userManager.AddClaimsAsync(user, AppClaims.GetAdminClaims());
        }
    }
}
