using FluentValidation;

namespace WorkshopPlanner.Application.Features.Workshops.AddSession;

public sealed class AddSessionValidator : AbstractValidator<AddSessionCommand>
{
    public AddSessionValidator()
    {
        RuleFor(command => command.Title).NotEmpty();
        RuleFor(command => command.SpeakerName).NotEmpty();
        RuleFor(command => command.DurationMinutes).InclusiveBetween(30, 180);
    }
}
