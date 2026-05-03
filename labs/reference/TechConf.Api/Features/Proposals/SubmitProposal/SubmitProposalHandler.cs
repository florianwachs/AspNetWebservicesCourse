using MediatR;
using Microsoft.EntityFrameworkCore;
using TechConf.Api.Data;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Infrastructure.Exceptions;
using TechConf.Api.Models;

namespace TechConf.Api.Features.Proposals.SubmitProposal;

public sealed class SubmitProposalHandler(AppDbContext db, ICurrentUserAccessor currentUser)
    : IRequestHandler<SubmitProposalCommand, ProposalDetailResponse>
{
    public async Task<ProposalDetailResponse> Handle(SubmitProposalCommand request, CancellationToken cancellationToken)
    {
        var proposal = await db.Proposals
            .Include(x => x.SpeakerProfile)
            .Include(x => x.ConferenceEvent)
            .SingleOrDefaultAsync(x => x.Id == request.ProposalId, cancellationToken);

        if (proposal is null)
        {
            throw new NotFoundException("Proposal", request.ProposalId);
        }

        var isOwner = proposal.SpeakerProfile.UserId == currentUser.UserId;
        var isOrganizer = currentUser.IsInRole(RoleNames.Organizer);
        if (!isOwner && !isOrganizer)
        {
            throw new ForbiddenException("Only the speaker owner or an organizer can submit this proposal.");
        }

        if (proposal.Status != ProposalStatus.Draft)
        {
            throw new ConflictException("Only draft proposals can be submitted.");
        }

        proposal.Status = ProposalStatus.Submitted;
        proposal.SubmittedAtUtc = DateTimeOffset.UtcNow;
        proposal.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return new ProposalDetailResponse(
            proposal.Id,
            proposal.Title,
            proposal.Abstract,
            proposal.Track,
            proposal.DurationMinutes,
            proposal.Status,
            proposal.SpeakerProfileId,
            proposal.SpeakerProfile.DisplayName,
            proposal.ConferenceEventId,
            proposal.ConferenceEvent.Name,
            proposal.CreatedAtUtc,
            proposal.UpdatedAtUtc,
            proposal.SubmittedAtUtc,
            proposal.ReviewedAtUtc,
            proposal.DecisionNote);
    }
}
