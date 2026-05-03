using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using TechConf.Api.Data;
using TechConf.Api.Infrastructure.Pagination;
using TechConf.Api.Models;

namespace TechConf.Api.Features.Speakers.Public.GetPublicSpeakers;

public sealed class GetPublicSpeakersHandler(AppDbContext db, HybridCache cache)
    : IRequestHandler<GetPublicSpeakersQuery, PagedResponse<PublicSpeakerSummary>>
{
    public async Task<PagedResponse<PublicSpeakerSummary>> Handle(GetPublicSpeakersQuery request, CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();
        var company = request.Company?.Trim();
        var sort = request.Sort?.Trim().ToLowerInvariant();
        var cacheKey = $"speakers:list:{search}:{company}:{sort}:{request.Page}:{request.PageSize}";

        return await cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                var query = db.SpeakerProfiles.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(profile =>
                        EF.Functions.ILike(profile.DisplayName, $"%{search}%") ||
                        EF.Functions.ILike(profile.Tagline, $"%{search}%") ||
                        EF.Functions.ILike(profile.Bio, $"%{search}%"));
                }

                if (!string.IsNullOrWhiteSpace(company))
                {
                    query = query.Where(profile => EF.Functions.ILike(profile.Company, $"%{company}%"));
                }

                query = sort switch
                {
                    "company" => query.OrderBy(profile => profile.Company).ThenBy(profile => profile.DisplayName),
                    "talks" => query.OrderByDescending(profile => profile.Proposals.Count(proposal => proposal.Status == ProposalStatus.Accepted))
                        .ThenBy(profile => profile.DisplayName),
                    _ => query.OrderBy(profile => profile.DisplayName)
                };

                var totalCount = await query.CountAsync(ct);
                var skip = (request.Page - 1) * request.PageSize;

                var page = await query
                    .Skip(skip)
                    .Take(request.PageSize)
                    .Select(profile => new
                    {
                        profile.Id,
                        profile.DisplayName,
                        profile.Tagline,
                        profile.Company,
                        profile.City,
                        profile.UpdatedAtUtc,
                        AcceptedTalkCount = profile.Proposals.Count(proposal => proposal.Status == ProposalStatus.Accepted)
                    })
                    .ToListAsync(ct);

                var speakerIds = page.Select(x => x.Id).ToArray();
                var recentTalkLookup = (await db.Proposals
                        .AsNoTracking()
                        .Where(proposal => speakerIds.Contains(proposal.SpeakerProfileId) && proposal.Status == ProposalStatus.Accepted)
                        .OrderByDescending(proposal => proposal.ReviewedAtUtc ?? proposal.UpdatedAtUtc)
                        .Select(proposal => new
                        {
                            proposal.SpeakerProfileId,
                            proposal.Title
                        })
                        .ToListAsync(ct))
                    .GroupBy(x => x.SpeakerProfileId)
                    .ToDictionary(
                        group => group.Key,
                        group => (IReadOnlyList<string>)group.Select(x => x.Title).Distinct().Take(2).ToArray());

                var data = page.Select(profile => new PublicSpeakerSummary(
                        profile.Id,
                        profile.DisplayName,
                        profile.Tagline,
                        profile.Company,
                        profile.City,
                        profile.AcceptedTalkCount,
                        recentTalkLookup.GetValueOrDefault(profile.Id, Array.Empty<string>()),
                        profile.UpdatedAtUtc))
                    .ToList();

                var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)request.PageSize);
                return new PagedResponse<PublicSpeakerSummary>(
                    data,
                    new PaginationMetadata(request.Page, request.PageSize, totalCount, totalPages));
            },
            tags: ["speakers"],
            cancellationToken: cancellationToken);
    }
}
