using AspNet.Security.OAuth.GitHub;
using GithubAuth.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllers();

services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/build";
});


var githubClientId = builder.Configuration["Auth:GitHub:ClientId"];
var githubClientSecret = builder.Configuration["Auth:GitHub:ClientSecret"];

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GitHubAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(setup =>
{
    setup.ExpireTimeSpan = TimeSpan.FromMinutes(60);
})
.AddGitHub(options =>
{
    options.ClientId = githubClientId;
    options.ClientSecret = githubClientSecret;
    options.SaveTokens = true;
    options.Scope.Add("openid read:user");
});

services.AddHttpClient<GithubApiClient>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});

app.Run();
