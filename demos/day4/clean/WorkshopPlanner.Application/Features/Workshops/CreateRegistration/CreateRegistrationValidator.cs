using FluentValidation;

namespace WorkshopPlanner.Application.Features.Workshops.CreateRegistration;

public sealed class CreateRegistrationValidator : AbstractValidator<CreateRegistrationCommand>
{
    public CreateRegistrationValidator()
    {
        RuleFor(command => command.AttendeeName).NotEmpty();
        RuleFor(command => command.AttendeeEmail).NotEmpty().EmailAddress();
    }
}
