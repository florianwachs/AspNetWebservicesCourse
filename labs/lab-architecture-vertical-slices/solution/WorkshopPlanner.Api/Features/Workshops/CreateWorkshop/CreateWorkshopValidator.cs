using FluentValidation;

namespace WorkshopPlanner.Api.Features.Workshops.CreateWorkshop;

public sealed class CreateWorkshopValidator : AbstractValidator<CreateWorkshopCommand>
{
    public CreateWorkshopValidator()
    {
        RuleFor(command => command.Title).NotEmpty();
        RuleFor(command => command.City).NotEmpty();
        RuleFor(command => command.MaxAttendees).GreaterThanOrEqualTo(5);
    }
}
