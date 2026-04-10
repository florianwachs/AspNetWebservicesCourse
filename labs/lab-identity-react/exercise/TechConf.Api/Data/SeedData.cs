using Microsoft.AspNetCore.Identity;

namespace TechConf.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // TODO: Task 2 — Create roles: "Organizer", "Speaker", "Attendee"
        // Hint:
        // string[] roles = ["Organizer", "Speaker", "Attendee"];
        // foreach (var role in roles)
        // {
        //     if (!await roleManager.RoleExistsAsync(role))
        //         await roleManager.CreateAsync(new IdentityRole(role));
        // }

        // TODO: Task 2 — Create seed users and assign roles
        // Hint: Use userManager.CreateAsync() and userManager.AddToRoleAsync()
        // Create admin@techconf.dev with password "Admin123!" → Organizer role
        // Create speaker@techconf.dev with password "Speaker123!" → Speaker role
        // Create attendee@techconf.dev with password "Attendee123!" → Attendee role
    }
}
