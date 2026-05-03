using FluentValidation;
using TechConf.Api.Models;

namespace TechConf.Api.Features.Proposals.ReviewProposal;

public sealed class ReviewProposalValidator : AbstractValidator<ReviewProposalCommand>
{
    public ReviewProposalValidator()
    {
        RuleFor(x => x.ProposalId).GreaterThan(0);
        RuleFor(x => x.TargetStatus)
            .Must(status => status is ProposalStatus.Accepted or ProposalStatus.Rejected)
            .WithMessage("targetStatus must be Accepted or Rejected.");
        RuleFor(x => x.Note).MaximumLength(1000);
    }
}
