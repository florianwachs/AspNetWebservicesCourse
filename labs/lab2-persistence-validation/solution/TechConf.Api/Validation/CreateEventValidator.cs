using FluentValidation;
using TechConf.Api.Models;

namespace TechConf.Api.Validation;

public class CreateEventValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Event name is required")
            .MaximumLength(200).WithMessage("Event name must not exceed 200 characters");

        RuleFor(x => x.Date)
            .GreaterThan(DateTime.UtcNow).WithMessage("Event date must be in the future");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");
    }
}
