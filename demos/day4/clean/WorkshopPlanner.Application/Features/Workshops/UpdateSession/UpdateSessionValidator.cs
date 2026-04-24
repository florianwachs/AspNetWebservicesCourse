using FluentValidation;

namespace WorkshopPlanner.Application.Features.Workshops.UpdateSession;

public sealed class UpdateSessionValidator : AbstractValidator<UpdateSessionCommand>
{
    public UpdateSessionValidator()
    {
        RuleFor(command => command.Title).NotEmpty();
        RuleFor(command => command.SpeakerName).NotEmpty();
        RuleFor(command => command.DurationMinutes).InclusiveBetween(30, 180);
    }
}
