using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StsServerIdentity.Data;
using StsServerIdentity.Models;

namespace StsServerIdentity
{
    public class SeedData
    {
        public static void EnsureSeedUsers(IServiceProvider serviceProvider)
        {
            // Scope erzeugen damit nach der Anlage alle Services aufgeräumt werden
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                CreateUser(userMgr, "bob1@bob.de", "bob1@bob.de", "Pass123$", new Claim[]  {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                                new Claim(JwtClaimTypes.GivenName, "Studen1"),
                                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                new Claim(JwtClaimTypes.Email, "StudentSmith@email.com"),
                                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                                new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                                new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                                new Claim("location", "somewhere")
                            });
            }
        }

        private static void CreateUser(UserManager<ApplicationUser> userMgr, string userName, string userEmail, string password, IEnumerable<Claim> claims)
        {
            var user = userMgr.FindByNameAsync(userName).Result;
            if (user != null)
                return;

            user = new ApplicationUser
            {
                UserName = userName,
                Email = userEmail,
                EmailConfirmed = true
            };
            var result = userMgr.CreateAsync(user, password).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(user, claims).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
    }
}
