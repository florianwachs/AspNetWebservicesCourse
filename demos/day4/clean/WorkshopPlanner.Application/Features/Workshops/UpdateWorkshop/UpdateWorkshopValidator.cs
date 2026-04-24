using FluentValidation;

namespace WorkshopPlanner.Application.Features.Workshops.UpdateWorkshop;

public sealed class UpdateWorkshopValidator : AbstractValidator<UpdateWorkshopCommand>
{
    public UpdateWorkshopValidator()
    {
        RuleFor(command => command.Title).NotEmpty();
        RuleFor(command => command.City).NotEmpty();
        RuleFor(command => command.MaxAttendees).GreaterThanOrEqualTo(5);
    }
}
