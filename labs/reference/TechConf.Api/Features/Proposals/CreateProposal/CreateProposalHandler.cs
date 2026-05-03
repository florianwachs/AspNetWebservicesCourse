using MediatR;
using Microsoft.EntityFrameworkCore;
using TechConf.Api.Data;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Infrastructure.Exceptions;
using TechConf.Api.Models;

namespace TechConf.Api.Features.Proposals.CreateProposal;

public sealed class CreateProposalHandler(AppDbContext db, ICurrentUserAccessor currentUser)
    : IRequestHandler<CreateProposalCommand, ProposalDetailResponse>
{
    public async Task<ProposalDetailResponse> Handle(CreateProposalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new ForbiddenException("You must be signed in.");

        var speakerProfile = await db.SpeakerProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (speakerProfile is null)
        {
            throw new ForbiddenException("Create your speaker profile before submitting a proposal.");
        }

        var conferenceEvent = await db.ConferenceEvents
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken);

        if (conferenceEvent is null)
        {
            throw new NotFoundException("Conference event", request.EventId);
        }

        if (conferenceEvent.ProposalDeadlineUtc < DateTimeOffset.UtcNow)
        {
            throw new ConflictException("The proposal deadline for this event has already passed.");
        }

        var proposal = new Proposal
        {
            SpeakerProfileId = speakerProfile.Id,
            ConferenceEventId = conferenceEvent.Id,
            Title = request.Title,
            Abstract = request.Abstract,
            DurationMinutes = request.DurationMinutes,
            Track = request.Track,
            Status = ProposalStatus.Draft,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        db.Proposals.Add(proposal);
        await db.SaveChangesAsync(cancellationToken);

        return new ProposalDetailResponse(
            proposal.Id,
            proposal.Title,
            proposal.Abstract,
            proposal.Track,
            proposal.DurationMinutes,
            proposal.Status,
            speakerProfile.Id,
            speakerProfile.DisplayName,
            conferenceEvent.Id,
            conferenceEvent.Name,
            proposal.CreatedAtUtc,
            proposal.UpdatedAtUtc,
            proposal.SubmittedAtUtc,
            proposal.ReviewedAtUtc,
            proposal.DecisionNote);
    }
}
