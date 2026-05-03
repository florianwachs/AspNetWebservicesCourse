using System.Threading.Channels;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using TechConf.Api.Data;
using TechConf.Api.Hubs;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Infrastructure.Exceptions;
using TechConf.Api.Models;
using TechConf.Api.Workers;

namespace TechConf.Api.Features.Proposals.ReviewProposal;

public sealed class ReviewProposalHandler(
    AppDbContext db,
    HybridCache cache,
    ICurrentUserAccessor currentUser,
    Channel<AcceptedProposalMessage> channel,
    IHubContext<ProposalNotificationsHub, IProposalNotificationsClient> hubContext)
    : IRequestHandler<ReviewProposalCommand, ProposalDetailResponse>
{
    public async Task<ProposalDetailResponse> Handle(ReviewProposalCommand request, CancellationToken cancellationToken)
    {
        var reviewerUserId = currentUser.UserId ?? throw new ForbiddenException("You must be signed in.");

        var proposal = await db.Proposals
            .Include(x => x.SpeakerProfile)
            .Include(x => x.ConferenceEvent)
            .SingleOrDefaultAsync(x => x.Id == request.ProposalId, cancellationToken);

        if (proposal is null)
        {
            throw new NotFoundException("Proposal", request.ProposalId);
        }

        if (proposal.Status != ProposalStatus.Submitted)
        {
            throw new ConflictException("Only submitted proposals can be reviewed.");
        }

        proposal.Status = request.TargetStatus;
        proposal.DecisionNote = request.Note;
        proposal.ReviewedAtUtc = DateTimeOffset.UtcNow;
        proposal.ReviewedByUserId = reviewerUserId;
        proposal.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        await cache.RemoveByTagAsync("speakers", cancellationToken);

        if (request.TargetStatus == ProposalStatus.Accepted)
        {
            await channel.Writer.WriteAsync(
                new AcceptedProposalMessage(
                    proposal.Id,
                    proposal.SpeakerProfile.DisplayName,
                    proposal.SpeakerProfile.Email,
                    proposal.Title,
                    proposal.ConferenceEvent.Name),
                cancellationToken);
        }

        var notification = new ProposalReviewNotification(
            proposal.Id,
            proposal.Title,
            proposal.Status.ToString(),
            request.Note ?? $"Your proposal was {proposal.Status.ToString().ToLowerInvariant()}.",
            proposal.ReviewedAtUtc!.Value);

        await hubContext.Clients
            .Group(ProposalNotificationsHub.GetUserGroup(proposal.SpeakerProfile.UserId))
            .ProposalReviewed(notification);

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
