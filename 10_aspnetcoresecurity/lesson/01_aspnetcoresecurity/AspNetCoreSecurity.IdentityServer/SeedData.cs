using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AspNetCoreSecurity.Domain.Data;
using AspNetCoreSecurity.Domain.Domain;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StsServerIdentity.Data;
using StsServerIdentity.Models;
using static AspNetCoreSecurity.Domain.Data.KnownUsers;

namespace StsServerIdentity
{
    public class SeedData
    {
        private static IEnumerable<Claim> GetClaimsFor(UserCreateData data, ApplicationUser user)
        {
            var claims = new List<Claim>();

            claims.AddRange(new Claim[]  {
                new Claim(JwtClaimTypes.Name, $"{data.GivenName} {data.FamilyName}"),
                                new Claim(JwtClaimTypes.GivenName, data.GivenName),
                                new Claim(JwtClaimTypes.FamilyName, data.FamilyName),
                                new Claim(JwtClaimTypes.Email, data.Email),
                                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                                new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                                new Claim("location", "Rosenheim")
                            });

            switch (data.Type)
            {
                case UserTypes.Student:
                    claims.Add(new Claim(AuthConstants.StudentType, user.Id, ClaimValueTypes.String, AuthConstants.Issuer));
                    break;
                case UserTypes.Professor:
                    claims.Add(new Claim(AuthConstants.ProfessorType, user.Id, ClaimValueTypes.String, AuthConstants.Issuer));
                    break;
                case UserTypes.Principal:
                    claims.Add(new Claim(AuthConstants.PrincipalType, user.Id, ClaimValueTypes.String, AuthConstants.Issuer));
                    break;
            }

            return claims;

        }

        public static void EnsureSeedUsers(IServiceProvider serviceProvider)
        {
            // Scope erzeugen damit nach der Anlage alle Services aufgeräumt werden
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                foreach (var userData in KnownUsers.Get())
                {
                    CreateUser(userMgr, userData);
                }
            }
        }

        private static void CreateUser(UserManager<ApplicationUser> userMgr, UserCreateData data)
        {
            var user = userMgr.FindByIdAsync(data.Id).Result;
            if (user != null)
            {
                // Für die Vorlesung erzeugen wir den User jedes mal neu
                var deleteResult = userMgr.DeleteAsync(user).Result;
            }

            user = new ApplicationUser
            {
                Id = data.Id,
                UserName = data.Email,
                Email = data.Email,
                EmailConfirmed = true
            };
            var result = userMgr.CreateAsync(user, "Pass123$").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(user, GetClaimsFor(data, user)).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
    }
}
