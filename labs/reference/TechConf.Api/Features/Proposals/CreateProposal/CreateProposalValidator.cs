using FluentValidation;

namespace TechConf.Api.Features.Proposals.CreateProposal;

public sealed class CreateProposalValidator : AbstractValidator<CreateProposalCommand>
{
    public CreateProposalValidator()
    {
        RuleFor(x => x.EventId).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Abstract).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.DurationMinutes).InclusiveBetween(15, 90);
        RuleFor(x => x.Track).NotEmpty().MaximumLength(80);
    }
}
