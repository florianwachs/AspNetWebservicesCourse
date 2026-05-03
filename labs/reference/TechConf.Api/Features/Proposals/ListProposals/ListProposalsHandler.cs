using MediatR;
using Microsoft.EntityFrameworkCore;
using TechConf.Api.Data;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Infrastructure.Exceptions;
using TechConf.Api.Infrastructure.Pagination;

namespace TechConf.Api.Features.Proposals.ListProposals;

public sealed class ListProposalsHandler(AppDbContext db, ICurrentUserAccessor currentUser)
    : IRequestHandler<ListProposalsQuery, PagedResponse<ProposalListItemResponse>>
{
    public async Task<PagedResponse<ProposalListItemResponse>> Handle(ListProposalsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new ForbiddenException("You must be signed in.");

        var query = db.Proposals.AsNoTracking().AsQueryable();

        if (!currentUser.IsInRole(RoleNames.Organizer))
        {
            query = query.Where(proposal => proposal.SpeakerProfile.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(proposal =>
                EF.Functions.ILike(proposal.Title, $"%{search}%") ||
                EF.Functions.ILike(proposal.Track, $"%{search}%") ||
                EF.Functions.ILike(proposal.SpeakerProfile.DisplayName, $"%{search}%"));
        }

        if (request.Status is not null)
        {
            query = query.Where(proposal => proposal.Status == request.Status);
        }

        query = request.Sort?.Trim().ToLowerInvariant() switch
        {
            "title" => query.OrderBy(proposal => proposal.Title),
            "status" => query.OrderBy(proposal => proposal.Status).ThenByDescending(proposal => proposal.CreatedAtUtc),
            "speaker" => query.OrderBy(proposal => proposal.SpeakerProfile.DisplayName),
            _ => query.OrderByDescending(proposal => proposal.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (request.Page - 1) * request.PageSize;
        var data = await query
            .Skip(skip)
            .Take(request.PageSize)
            .Select(proposal => new ProposalListItemResponse(
                proposal.Id,
                proposal.Title,
                proposal.Track,
                proposal.DurationMinutes,
                proposal.Status,
                proposal.SpeakerProfile.DisplayName,
                proposal.SpeakerProfileId,
                proposal.ConferenceEvent.Name,
                proposal.CreatedAtUtc,
                proposal.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)request.PageSize);
        return new PagedResponse<ProposalListItemResponse>(
            data,
            new PaginationMetadata(request.Page, request.PageSize, totalCount, totalPages));
    }
}
