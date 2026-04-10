using Microsoft.AspNetCore.Identity;

namespace TechConf.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = ["Organizer", "Speaker", "Attendee"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create a default admin/organizer user
        var adminEmail = "admin@techconf.dev";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Organizer");
        }

        // Create a speaker user
        var speakerEmail = "speaker@techconf.dev";
        if (await userManager.FindByEmailAsync(speakerEmail) is null)
        {
            var speaker = new IdentityUser
            {
                UserName = speakerEmail,
                Email = speakerEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(speaker, "Speaker123!");
            await userManager.AddToRoleAsync(speaker, "Speaker");
        }

        // Create an attendee user
        var attendeeEmail = "attendee@techconf.dev";
        if (await userManager.FindByEmailAsync(attendeeEmail) is null)
        {
            var attendee = new IdentityUser
            {
                UserName = attendeeEmail,
                Email = attendeeEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(attendee, "Attendee123!");
            await userManager.AddToRoleAsync(attendee, "Attendee");
        }
    }
}
