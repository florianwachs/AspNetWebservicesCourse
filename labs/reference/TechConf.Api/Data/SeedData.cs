using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Models;

namespace TechConf.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var db = serviceProvider.GetRequiredService<AppDbContext>();

        foreach (var role in new[] { RoleNames.Organizer, RoleNames.Speaker, RoleNames.Attendee })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var organizer = await EnsureUserAsync(
            userManager,
            email: "organizer@techconf.dev",
            password: "Organizer123!",
            displayName: "Organizing Team",
            roles: [RoleNames.Organizer],
            claims:
            [
                new Claim(AppClaimTypes.ProposalReview, "true"),
                new Claim(AppClaimTypes.SpeakerProfileWrite, "true")
            ]);

        var speaker = await EnsureUserAsync(
            userManager,
            email: "sarah.speaker@techconf.dev",
            password: "Speaker123!",
            displayName: "Sarah Speaker",
            roles: [RoleNames.Speaker],
            claims:
            [
                new Claim(AppClaimTypes.SpeakerProfileWrite, "true")
            ]);

        var secondSpeaker = await EnsureUserAsync(
            userManager,
            email: "maya.api@techconf.dev",
            password: "Speaker123!",
            displayName: "Maya API",
            roles: [RoleNames.Speaker],
            claims:
            [
                new Claim(AppClaimTypes.SpeakerProfileWrite, "true")
            ]);

        await EnsureUserAsync(
            userManager,
            email: "attendee@techconf.dev",
            password: "Attendee123!",
            displayName: "Taylor Attendee",
            roles: [RoleNames.Attendee],
            claims: []);

        if (!await db.ConferenceEvents.AnyAsync())
        {
            db.ConferenceEvents.AddRange(
                new ConferenceEvent
                {
                    Slug = "techconf-2026",
                    Name = "TechConf 2026",
                    City = "Munich",
                    Location = "Science Congress Center",
                    StartDate = new DateOnly(2026, 10, 8),
                    EndDate = new DateOnly(2026, 10, 9),
                    ProposalDeadlineUtc = new DateTimeOffset(2026, 7, 15, 12, 0, 0, TimeSpan.Zero),
                    UpdatedAtUtc = DateTimeOffset.UtcNow
                },
                new ConferenceEvent
                {
                    Slug = "cloud-native-day",
                    Name = "Cloud Native Day",
                    City = "Berlin",
                    Location = "Tech Hub Berlin",
                    StartDate = new DateOnly(2026, 11, 20),
                    EndDate = new DateOnly(2026, 11, 20),
                    ProposalDeadlineUtc = new DateTimeOffset(2026, 8, 20, 12, 0, 0, TimeSpan.Zero),
                    UpdatedAtUtc = DateTimeOffset.UtcNow
                });
        }

        await db.SaveChangesAsync();

        if (!await db.SpeakerProfiles.AnyAsync())
        {
            db.SpeakerProfiles.AddRange(
                new SpeakerProfile
                {
                    UserId = speaker.Id,
                    DisplayName = "Sarah Speaker",
                    Tagline = "Designing resilient APIs that stay pleasant to consume.",
                    Bio = "Sarah helps teams turn large distributed systems into understandable REST APIs. She teaches architecture, validation, and pragmatic API design.",
                    Company = "Northwind Labs",
                    City = "Munich",
                    Email = speaker.Email!,
                    WebsiteUrl = "https://northwind.example/sarah",
                    PhotoUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=600&q=80",
                    CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-45),
                    UpdatedAtUtc = DateTimeOffset.UtcNow.AddDays(-1)
                },
                new SpeakerProfile
                {
                    UserId = secondSpeaker.Id,
                    DisplayName = "Maya API",
                    Tagline = "Turning messy integrations into dependable platform stories.",
                    Bio = "Maya focuses on observability, identity, and event-driven integration patterns. She enjoys making API ecosystems easier to navigate.",
                    Company = "Contoso Platform",
                    City = "Berlin",
                    Email = secondSpeaker.Email!,
                    WebsiteUrl = "https://contoso.example/maya",
                    PhotoUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?auto=format&fit=crop&w=600&q=80",
                    CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-30),
                    UpdatedAtUtc = DateTimeOffset.UtcNow.AddHours(-12)
                });

            await db.SaveChangesAsync();
        }

        if (!await db.Proposals.AnyAsync())
        {
            var events = await db.ConferenceEvents.OrderBy(x => x.Id).ToListAsync();
            var speakers = await db.SpeakerProfiles.OrderBy(x => x.Id).ToListAsync();

            db.Proposals.AddRange(
                new Proposal
                {
                    SpeakerProfileId = speakers[0].Id,
                    ConferenceEventId = events[0].Id,
                    Title = "REST that scales with your product",
                    Abstract = "A practical guide to versioning, consistent errors, pagination, and contract evolution.",
                    DurationMinutes = 60,
                    Track = "API Design",
                    Status = ProposalStatus.Accepted,
                    CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-20),
                    UpdatedAtUtc = DateTimeOffset.UtcNow.AddDays(-10),
                    SubmittedAtUtc = DateTimeOffset.UtcNow.AddDays(-18),
                    ReviewedAtUtc = DateTimeOffset.UtcNow.AddDays(-10),
                    ReviewedByUserId = organizer.Id,
                    DecisionNote = "Excellent fit for the architecture track."
                },
                new Proposal
                {
                    SpeakerProfileId = speakers[0].Id,
                    ConferenceEventId = events[0].Id,
                    Title = "HybridCache in real production APIs",
                    Abstract = "Learn where caching helps, where it hurts, and how to invalidate confidently in a distributed system.",
                    DurationMinutes = 45,
                    Track = "Performance",
                    Status = ProposalStatus.Submitted,
                    CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-6),
                    UpdatedAtUtc = DateTimeOffset.UtcNow.AddDays(-3),
                    SubmittedAtUtc = DateTimeOffset.UtcNow.AddDays(-3)
                },
                new Proposal
                {
                    SpeakerProfileId = speakers[1].Id,
                    ConferenceEventId = events[1].Id,
                    Title = "Secure-by-default SPA to API auth with ASP.NET Identity",
                    Abstract = "Compare cookie and token approaches, model role and claim policies, and keep your SPA UX clean.",
                    DurationMinutes = 60,
                    Track = "Security",
                    Status = ProposalStatus.Draft,
                    CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-4),
                    UpdatedAtUtc = DateTimeOffset.UtcNow.AddHours(-6)
                });

            await db.SaveChangesAsync();
        }
    }

    private static async Task<IdentityUser> EnsureUserAsync(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string displayName,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<Claim> claims)
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
                throw new InvalidOperationException($"Failed to seed user '{email}': {string.Join(", ", createResult.Errors.Select(x => x.Description))}");
            }
        }

        foreach (var role in roles)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }

        var existingClaims = await userManager.GetClaimsAsync(user);
        foreach (var claim in claims)
        {
            if (!existingClaims.Any(x => x.Type == claim.Type && x.Value == claim.Value))
            {
                await userManager.AddClaimAsync(user, claim);
            }
        }

        if (!existingClaims.Any(x => x.Type == ClaimTypes.Name))
        {
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, displayName));
        }

        return user;
    }
}
