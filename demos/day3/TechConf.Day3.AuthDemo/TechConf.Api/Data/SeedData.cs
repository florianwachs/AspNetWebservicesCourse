using Microsoft.AspNetCore.Identity;

namespace TechConf.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var role in new[] { "organizer", "speaker", "attendee" })
        {
            await EnsureRoleAsync(roleManager, role);
        }

        await EnsureUserAsync(userManager, "admin@techconf.dev", "Admin123!", "organizer");
        await EnsureUserAsync(userManager, "speaker@techconf.dev", "Speaker123!", "speaker");
        await EnsureUserAsync(userManager, "attendee@techconf.dev", "Attendee123!", "attendee");
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string role)
    {
        if (await roleManager.RoleExistsAsync(role))
        {
            return;
        }

        var result = await roleManager.CreateAsync(new IdentityRole(role));
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(error => error.Description))}");
        }
    }

    private static async Task EnsureUserAsync(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create user '{email}': {string.Join(", ", createResult.Errors.Select(error => error.Description))}");
            }
        }

        if (await userManager.IsInRoleAsync(user, role))
        {
            return;
        }

        var addToRoleResult = await userManager.AddToRoleAsync(user, role);
        if (!addToRoleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to add user '{email}' to role '{role}': {string.Join(", ", addToRoleResult.Errors.Select(error => error.Description))}");
        }
    }
}
