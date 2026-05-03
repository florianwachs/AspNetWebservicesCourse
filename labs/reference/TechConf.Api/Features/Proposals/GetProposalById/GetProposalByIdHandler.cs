using MediatR;
using Microsoft.EntityFrameworkCore;
using TechConf.Api.Data;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Infrastructure.Exceptions;

namespace TechConf.Api.Features.Proposals.GetProposalById;

public sealed class GetProposalByIdHandler(AppDbContext db, ICurrentUserAccessor currentUser)
    : IRequestHandler<GetProposalByIdQuery, ProposalDetailResponse>
{
    public async Task<ProposalDetailResponse> Handle(GetProposalByIdQuery request, CancellationToken cancellationToken)
    {
        var proposal = await db.Proposals
            .AsNoTracking()
            .Where(x => x.Id == request.ProposalId)
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Abstract,
                x.Track,
                x.DurationMinutes,
                x.Status,
                x.CreatedAtUtc,
                x.UpdatedAtUtc,
                x.SubmittedAtUtc,
                x.ReviewedAtUtc,
                x.DecisionNote,
                SpeakerProfileId = x.SpeakerProfile.Id,
                SpeakerName = x.SpeakerProfile.DisplayName,
                SpeakerUserId = x.SpeakerProfile.UserId,
                EventId = x.ConferenceEvent.Id,
                EventName = x.ConferenceEvent.Name
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (proposal is null)
        {
            throw new NotFoundException("Proposal", request.ProposalId);
        }

        var canView = currentUser.IsInRole(RoleNames.Organizer) || proposal.SpeakerUserId == currentUser.UserId;
        if (!canView)
        {
            throw new ForbiddenException("You are not allowed to view this proposal.");
        }

        return new ProposalDetailResponse(
            proposal.Id,
            proposal.Title,
            proposal.Abstract,
            proposal.Track,
            proposal.DurationMinutes,
            proposal.Status,
            proposal.SpeakerProfileId,
            proposal.SpeakerName,
            proposal.EventId,
            proposal.EventName,
            proposal.CreatedAtUtc,
            proposal.UpdatedAtUtc,
            proposal.SubmittedAtUtc,
            proposal.ReviewedAtUtc,
            proposal.DecisionNote);
    }
}
