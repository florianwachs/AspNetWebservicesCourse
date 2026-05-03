using MediatR;
using Microsoft.EntityFrameworkCore;
using TechConf.Api.Data;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Infrastructure.Exceptions;

namespace TechConf.Api.Features.Speakers.Me.GetMySpeakerProfile;

public sealed class GetMySpeakerProfileHandler(AppDbContext db, ICurrentUserAccessor currentUser)
    : IRequestHandler<GetMySpeakerProfileQuery, MySpeakerProfileResponse>
{
    public async Task<MySpeakerProfileResponse> Handle(GetMySpeakerProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new ForbiddenException("You must be signed in.");

        var profile = await db.SpeakerProfiles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new MySpeakerProfileResponse(
                x.Id,
                x.DisplayName,
                x.Tagline,
                x.Bio,
                x.Company,
                x.City,
                x.Email,
                x.WebsiteUrl,
                x.PhotoUrl,
                x.Proposals.Count,
                x.UpdatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);

        return profile ?? throw new NotFoundException("Speaker profile", "me");
    }
}
