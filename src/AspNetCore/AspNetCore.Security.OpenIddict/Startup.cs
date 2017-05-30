using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AspNetCore.Security.OpenIddict.Models;
using Microsoft.EntityFrameworkCore;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AspNetCore.Security.OpenIddict.Infrastructure;

namespace AspNetCore.Security.OpenIddict
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            ConfigureAuth(services);
        }

        private void ConfigureAuth(IServiceCollection services)
        {
            // Register the Identity services.
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:IdentityContext"]);
                options.UseOpenIddict();
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AppPolicies.CanAccessCustomerSaleHistory, policy => policy.RequireClaim(AppClaimTypes.ManagerId));
                options.AddPolicy(AppPolicies.CanReadCustomerAge, policy => policy.RequireClaim(AppClaimTypes.Department, Departments.CustomerManagement));
                options.AddPolicy(AppPolicies.CanDeleteCustomer, policy => policy.RequireClaim(AppClaimTypes.Department, Departments.CustomerManagement));
                options.AddPolicy(AppPolicies.CanUpdateCustomer, policy => policy.RequireClaim(AppClaimTypes.Department, Departments.CustomerManagement, Departments.CustomerSupport));
                options.AddPolicy(AppPolicies.CanCreateCustomer, policy => policy.RequireClaim(AppClaimTypes.Department, Departments.CustomerManagement, Departments.CustomerSupport));

                options.AddPolicy("TopSecret", policy =>
                {
                    policy.RequireClaim(AppClaimTypes.AccessLevel, "A1")
                    .RequireRole("Executive Manager")
                    .RequireAssertion(authContext =>
                        authContext.User.HasClaim(claim =>
                            claim.Type == AppClaimTypes.Age && int.TryParse(claim.Value, out int age) && age > 18));
                });
            });

            // Die JWT-Token-Claims statt den WS-Federation verwenden (die nimmt Identity by default)
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;

                // Passwort-Regeln können sehr granular festgelegt werden
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });

            services.AddOpenIddict(options =>
            {
                // Register the Entity Framework stores.
                options.AddEntityFrameworkCoreStores<IdentityContext>();

                // Register the ASP.NET Core MVC binder used by OpenIddict.
                // Note: if you don't call this method, you won't be able to
                // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                options.AddMvcBinders();

                // Endpunkte aktivieren
                options.EnableTokenEndpoint("/connect/token")
                .EnableUserinfoEndpoint("/api/userinfo");

                // Note: the Mvc.Client sample only uses the code flow and the password flow, but you
                // can enable the other flows if you need to support implicit or client credentials.
                options.AllowPasswordFlow()
                       .AllowRefreshTokenFlow();

                // During development, you can disable the HTTPS requirement.
                options.DisableHttpsRequirement();

                // Note: to use JWT access tokens instead of the default
                // encrypted format, the following lines are required:
                options.UseJsonWebTokens();

                // Für die Signierung der JWT-Tokens wird ein Zertifikat benötigt.
                // Für DEV-Zwecke kann ein Key dynamisch erzeugt werden.
                options.AddEphemeralSigningKey();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Automatisches Mapping von JWT auf WS-Federation (Legacy) Tokens unterbinden
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            app.UseOAuthValidation();
            //app.UseIdentity();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                Authority = "http://localhost:28476/",
                Audience = "resource_server",
                RequireHttpsMetadata = false,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = OpenIdConnectConstants.Claims.Subject,
                    RoleClaimType = OpenIdConnectConstants.Claims.Role,
                }
            });



            app.UseOpenIddict();

            app.UseMvc();

            InitializeAuthDb(app.ApplicationServices).Wait();
        }

        private async Task InitializeAuthDb(IServiceProvider applicationServices)
        {
            using (var scope = applicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
                await context.Database.EnsureCreatedAsync();

                var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                await AddUser(manager, "chuck", "chuck@norris.com", "norris", GetManagerClaims());
                await AddUser(manager, "jason", "jason@bourne.com", "bourne", GetSupportClaims());
            }
        }

        private async Task AddUser(UserManager<ApplicationUser> manager, string username, string email, string password, IEnumerable<Claim> claims)
        {
            var user = await manager.FindByNameAsync(username);
            if (user != null)
                return;

            var result = await manager.CreateAsync(new ApplicationUser { UserName = username, Email = email }, password);

            user = await manager.FindByNameAsync(username);
            await manager.AddClaimsAsync(user, claims);
        }

        private IEnumerable<Claim> GetManagerClaims()
        {
            yield return CreateClaim(ClaimTypes.PostalCode, "12345");
            yield return CreateClaim(AppClaimTypes.Department, "Customer Management");
        }

        private IEnumerable<Claim> GetSupportClaims()
        {
            yield return CreateClaim(ClaimTypes.PostalCode, "54321");
            yield return CreateClaim(AppClaimTypes.Department, "Customer Support");
        }

        private Claim CreateClaim(string type, string value) => new Claim(type, value, ClaimValueTypes.String, ClaimsIdentity.DefaultIssuer);
    }
}
