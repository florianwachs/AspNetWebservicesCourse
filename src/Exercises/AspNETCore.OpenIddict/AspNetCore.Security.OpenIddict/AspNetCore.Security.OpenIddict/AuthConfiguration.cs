using AspNet.Security.OpenIdConnect.Primitives;
using AspNetCore.Security.OpenIddict.Infrastructure;
using AspNetCore.Security.OpenIddict.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Security.OpenIddict
{
    public static class AuthConfiguration
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfigurationRoot configuration)
        {
            AddServicesForIdentity(services);
            AddServicesForIdentityContext(services, configuration);
            ConfigureIdentityOptionsForJwt(services);
            AddOpenIddictAndSetup(services);

            AddAndConfigurePolicies(services);

            return services;
        }

        private static void AddServicesForIdentity(IServiceCollection services)
        {
            // Die Services für Identity konfigurieren            
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();
        }

        private static void AddServicesForIdentityContext(IServiceCollection services, IConfigurationRoot configuration)
        {
            // Entity Framework bezogene Configuration für OpenIddict
            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionStrings:IdentityContext"]);

                // Fügt die von OpenIddict benötigten Entitäten und Modelklassen
                // dem Entity Framework Context hinzu
                options.UseOpenIddict();
            });
        }

        private static void ConfigureIdentityOptionsForJwt(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                // Die JWT-Token-Claims statt den WS-Federation verwenden (die nimmt Identity by default)
                // Identity verwendet standardmäßig nicht die JWT-Tokens, kann aber hiermit umkonfiguriert werden
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;

                // Passwort-Regeln können sehr granular festgelegt werden
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Auch Regeln für den Usernamen können festgelegt werden
                //options.User.AllowedUserNameCharacters = "";
            });
        }

        private static void AddOpenIddictAndSetup(IServiceCollection services)
        {
            services.AddOpenIddict(options =>
            {
                // Register the Entity Framework stores.
                options.AddEntityFrameworkCoreStores<IdentityContext>();

                // ModelBinder von OpenIddict registerieren.
                // Damit können Request-Parameter wieOpenIdConnectRequest oder
                // OpenIdConnectResponse verwendet werden.Diese werden meist in 
                // einem AuthorizationController verwendet um einen Autorisierungsrequest verarbeiten zu können.
                options.AddMvcBinders();

                // Endpunkte aktivieren
                // Hier wird registriert unter welchem Endpunkt ein Token angefragt werden kann.
                options.EnableTokenEndpoint("/connect/token")
                .EnableUserinfoEndpoint("/api/userinfo");

                // Der OAuth-Standard definiert verschiedene Möglichkeiten sich zu authentifizieren
                // und einen Token zu erhalten. Eine Möglichkeit ist der PasswordFlow.
                options.AllowPasswordFlow()
                       .AllowRefreshTokenFlow();

                // Dies aktiviert die Anwendung des JWT-Standards
                options.UseJsonWebTokens();

                // Das erleichert die Entwicklung in Produktion aber ein no go
                options.DisableHttpsRequirement();

                // Für die Signierung der JWT-Tokens wird ein Zertifikat benötigt.
                // Für DEV-Zwecke kann ein Key dynamisch erzeugt werden.
                options.AddEphemeralSigningKey();
            });
        }

        private static void AddAndConfigurePolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // TODO Policies
            });
        }
        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            DisableAutoTokenMapping();

            // Die OAuth-Middleware muss möglichst früh in der Pipeline registriert werden.            
            app.UseOAuthValidation();

            UseJwtBearerAuth(app);

            app.UseOpenIddict();

            // Sicherstellen das ein paar User in der DB vorhanden sind.
            InitializeAuthDb(app.ApplicationServices).Wait();

            return app;
        }

        private static void DisableAutoTokenMapping()
        {
            // Automatisches Mapping von JWT auf WS-Federation (Legacy) Tokens unterbinden
            // Identity probiert Claims automatisch zu mappen. Durch das zurücksetzen
            // der Mappingtabellen werden die Tokens nicht modifiziert
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
        }

        private static void UseJwtBearerAuth(IApplicationBuilder app)
        {
            // Die JWT-Middleware kann den Token aus dem Header extrahieren und das Request-Objekt damit erweitern
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                Authority = "http://localhost:24677/",
                Audience = "resource_server",
                RequireHttpsMetadata = false,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = OpenIdConnectConstants.Claims.Subject,
                    RoleClaimType = OpenIdConnectConstants.Claims.Role,
                }
            });
        }

        #region Seed

        private static async Task InitializeAuthDb(IServiceProvider applicationServices)
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

        private static async Task AddUser(UserManager<ApplicationUser> manager, string username, string email, string password, IEnumerable<Claim> claims)
        {
            // Prüfen ob User schon vorhanden ist
            var user = await manager.FindByNameAsync(username);
            if (user != null)
                return;

            // User mit Passwort erzeugen und in DB ablegen
            var result = await manager.CreateAsync(new ApplicationUser { UserName = username, Email = email }, password);

            // Den angelegten User laden und Claims hinzufügen
            user = await manager.FindByNameAsync(username);

            await manager.AddClaimsAsync(user, claims);
        }

        private static IEnumerable<Claim> GetManagerClaims()
        {
            yield return CreateClaim(ClaimTypes.PostalCode, "12345");
            yield return CreateClaim(AppClaimTypes.Department, "Customer Management");
        }

        private static IEnumerable<Claim> GetSupportClaims()
        {
            yield return CreateClaim(ClaimTypes.PostalCode, "54321");
            yield return CreateClaim(AppClaimTypes.Department, "Customer Support");
        }

        private static Claim CreateClaim(string type, string value) => new Claim(type, value, ClaimValueTypes.String, ClaimsIdentity.DefaultIssuer);

        #endregion
    }
}
