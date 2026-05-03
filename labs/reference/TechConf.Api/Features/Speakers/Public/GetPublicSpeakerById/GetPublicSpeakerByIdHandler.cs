using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using TechConf.Api.Data;
using TechConf.Api.Infrastructure.Exceptions;
using TechConf.Api.Models;

namespace TechConf.Api.Features.Speakers.Public.GetPublicSpeakerById;

public sealed class GetPublicSpeakerByIdHandler(AppDbContext db, HybridCache cache)
    : IRequestHandler<GetPublicSpeakerByIdQuery, SpeakerDetailV2>
{
    public async Task<SpeakerDetailV2> Handle(GetPublicSpeakerByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"speakers:detail:{request.SpeakerId}";

        return await cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                var speaker = await db.SpeakerProfiles
                    .AsNoTracking()
                    .Where(profile => profile.Id == request.SpeakerId)
                    .Select(profile => new
                    {
                        profile.Id,
                        profile.DisplayName,
                        profile.Tagline,
                        profile.Bio,
                        profile.Company,
                        profile.City,
                        profile.WebsiteUrl,
                        profile.PhotoUrl,
                        profile.UpdatedAtUtc,
                        AcceptedTalkCount = profile.Proposals.Count(proposal => proposal.Status == ProposalStatus.Accepted)
                    })
                    .SingleOrDefaultAsync(ct);

                if (speaker is null)
                {
                    throw new NotFoundException("Speaker", request.SpeakerId);
                }

                var talks = await db.Proposals
                    .AsNoTracking()
                    .Where(proposal => proposal.SpeakerProfileId == request.SpeakerId && proposal.Status == ProposalStatus.Accepted)
                    .OrderByDescending(proposal => proposal.ReviewedAtUtc ?? proposal.UpdatedAtUtc)
                    .Select(proposal => proposal.Title)
                    .Take(5)
                    .ToListAsync(ct);

                return new SpeakerDetailV2(
                    speaker.Id,
                    speaker.DisplayName,
                    speaker.Tagline,
                    speaker.Bio,
                    speaker.Company,
                    speaker.City,
                    speaker.WebsiteUrl,
                    speaker.PhotoUrl,
                    speaker.AcceptedTalkCount,
                    talks,
                    speaker.UpdatedAtUtc);
            },
            tags: ["speakers"],
            cancellationToken: cancellationToken);
    }
}
