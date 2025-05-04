using System.Net.Http.Headers;
using System.Security.Claims;
using AspNet.Security.OAuth.GitHub;
using Auth3Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddTransient<GithubEvents>();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseSqlite("Data Source=app.db"); });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication().AddCookie().AddGitHub(options =>
{
    options.ClientId = builder.Configuration["Auth:ClientId"];
    options.ClientSecret = builder.Configuration["Auth:ClientSecret"];
    options.Scope.Add("user:email");
    options.CallbackPath = "/signin-github";
    options.SaveTokens = true;

    options.EventsType = typeof(GithubEvents);

    // options.Events.OnCreatingTicket = async context =>
    // {
    //     var userManager = context.HttpContext.RequestServices
    //         .GetRequiredService<UserManager<ApplicationUser>>();
    //     var signInManager = context.HttpContext.RequestServices
    //         .GetRequiredService<SignInManager<ApplicationUser>>();
    //
    //     // Get user information from GitHub
    //     var emailClaim = context.Identity?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
    //     var nameClaim = context.Identity?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
    //     
    //     if (string.IsNullOrEmpty(emailClaim))
    //     {
    //         // Handle case when email is not available
    //         return;
    //     }
    //
    //     // Find user by email
    //     var user = await userManager.FindByEmailAsync(emailClaim);
    //
    //     // Create new user if not exists
    //     if (user == null)
    //     {
    //         user = new ApplicationUser
    //         {
    //             UserName = emailClaim,
    //             Email = emailClaim,
    //             //Name = nameClaim ?? emailClaim.Split('@')[0]
    //         };
    //
    //         var result = await userManager.CreateAsync(user);
    //         if (!result.Succeeded)
    //         {
    //             // Handle error
    //             return;
    //         }
    //     }
    //
    //     // Add external login if not already added
    //     var existingLogins = await userManager.GetLoginsAsync(user);
    //     if (!existingLogins.Any(l => l.LoginProvider == "GitHub" && l.ProviderKey == context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value))
    //     {
    //         await userManager.AddLoginAsync(user, new UserLoginInfo(
    //             "GitHub",
    //             context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!,
    //             "GitHub"));
    //     }
    //     
    //     
    //
    //     // Sign in the user
    //     await signInManager.SignInAsync(user, isPersistent: true);
    // };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Map("/test", (ClaimsPrincipal principal) =>
{
    return Results.Text(string.Format("{0} has {1} public repositories.",
        principal!.FindFirst("name")?.Value ?? "Unknown",
        principal!.FindFirst("public_repos")?.Value ?? "Unknown"));
}).RequireAuthorization();

app.MapGet("/", () => Results.Content("""
                                      <html>
                                      <body>
                                      <form method="post" action="/triggerlogin">
                                      <input type="submit" value="Login with GitHub" />
                                      </form>
                                      </body>
                                      </html>
                                      """, "text/html"));

app.MapPost("triggerlogin",
    () => Results.Challenge(properties: new AuthenticationProperties { RedirectUri = "/" },
        authenticationSchemes: [GitHubAuthenticationDefaults.AuthenticationScheme]));

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class GithubEvents(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, HttpClient httpClient)
    : OAuthEvents
{
    public override async Task CreatingTicket(OAuthCreatingTicketContext context)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("FWA-AuthTest", "1.0"));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        var response = await httpClient.GetAsync("https://api.github.com/user/emails");
        
        var content = await response.Content.ReadAsStringAsync();
        
        /*
         * [{"email":"florian.wachs@live.de","primary":true,"verified":true,"visibility":"private"},{"email":"florian.wachs@googlemail.com","primary":false,"verified":true,"visibility":null},{"email":"2216547+florianwachs@users.noreply.github.com","primary":false,"verified":true,"visibility":null},{"email":"florian.wachs@th-rosenheim.de","primary":false,"verified":true,"visibility":null}]
         */
        
        // Get user information from GitHub
        var emailClaim = context.Identity
            ?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
        var nameClaim = context.Identity?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
            ?.Value;

        if (string.IsNullOrEmpty(emailClaim))
        {
            // Handle case when email is not available
            return;
        }

        // Find user by email
        var user = await userManager.FindByEmailAsync(emailClaim);

        // Create new user if not exists
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = emailClaim,
                Email = emailClaim,
                //Name = nameClaim ?? emailClaim.Split('@')[0]
            };

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                // Handle error
                return;
            }
        }

        // Add external login if not already added
        var existingLogins = await userManager.GetLoginsAsync(user);
        if (!existingLogins.Any(l =>
                l.LoginProvider == "GitHub" &&
                l.ProviderKey == context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value))
        {
            await userManager.AddLoginAsync(user, new UserLoginInfo(
                "GitHub",
                context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!,
                "GitHub"));
        }


        

        // Sign in the user
        await signInManager.SignInAsync(user, isPersistent: true);
    }
}