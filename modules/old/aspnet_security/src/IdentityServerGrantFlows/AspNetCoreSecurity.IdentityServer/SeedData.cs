// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using StsServerIdentity.Data;
using StsServerIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections.Generic;

namespace StsServerIdentity
{
    public record UserCreateData(string Id, string UserName, string FamilyName, string GivenName, string Email);
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

            return claims;
        }

        public static IEnumerable<UserCreateData> KnownUsers =>
            new UserCreateData[]
            {
                new UserCreateData( "1","katie","Katie", "Katie", "katie@th.de")
            };

        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    foreach (var userData in KnownUsers)
                    {
                        CreateUser(userMgr, userData);
                    }
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
                UserName = data.UserName,
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
