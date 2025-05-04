using BlazorWasamAuth.Api.DataAccess;
using BlazorWasamAuth.Api.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasamAuth.Api;

public class SeedData
{
    public static async Task EnsureMigratedDb(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
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

        var result = await userManager.CreateAsync(admin, "Test123Test123!");
        result = await userManager.CreateAsync(alice, "Test123Test123!");
        result = await userManager.CreateAsync(chucky, "Test123Test123!");
    }
}