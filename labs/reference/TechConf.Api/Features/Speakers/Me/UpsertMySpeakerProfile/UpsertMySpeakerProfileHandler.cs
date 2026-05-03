using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using TechConf.Api.Data;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Infrastructure.Exceptions;
using TechConf.Api.Models;

namespace TechConf.Api.Features.Speakers.Me.UpsertMySpeakerProfile;

public sealed class UpsertMySpeakerProfileHandler(
    AppDbContext db,
    HybridCache cache,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<UpsertMySpeakerProfileCommand, MySpeakerProfileResponse>
{
    public async Task<MySpeakerProfileResponse> Handle(UpsertMySpeakerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new ForbiddenException("You must be signed in.");
        var email = currentUser.Email ?? throw new ForbiddenException("The current user does not have an email address.");

        var profile = await db.SpeakerProfiles.SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        if (profile is null)
        {
            profile = new SpeakerProfile
            {
                UserId = userId,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };
            db.SpeakerProfiles.Add(profile);
        }

        profile.DisplayName = request.DisplayName;
        profile.Tagline = request.Tagline;
        profile.Bio = request.Bio;
        profile.Company = request.Company;
        profile.City = request.City;
        profile.Email = email;
        profile.WebsiteUrl = request.WebsiteUrl;
        profile.PhotoUrl = request.PhotoUrl;
        profile.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        await cache.RemoveByTagAsync("speakers", cancellationToken);

        return new MySpeakerProfileResponse(
            profile.Id,
            profile.DisplayName,
            profile.Tagline,
            profile.Bio,
            profile.Company,
            profile.City,
            profile.Email,
            profile.WebsiteUrl,
            profile.PhotoUrl,
            await db.Proposals.CountAsync(x => x.SpeakerProfileId == profile.Id, cancellationToken),
            profile.UpdatedAtUtc);
    }
}
