using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactAppWithAuth1.Data;
using ReactAppWithAuth1.Models;

namespace ReactAppWithAuth1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await SeedDb(host);
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static async Task SeedDb(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await dbContext.Database.EnsureCreatedAsync();
            await dbContext.Database.MigrateAsync();


            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser[] defaultUsers =
            {
                new ApplicationUser() { Id = "user1@test.de", UserName = "user1@test.de", Email = "user1@test.de" },
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

                result = await userManager.AddClaimsAsync(user, AppClaims.GetUserClaims());

                if (user.IsAdmin)
                {
                    result = await userManager.AddClaimsAsync(user, AppClaims.GetAdminClaims());
                }

            }
        }
    }
}
