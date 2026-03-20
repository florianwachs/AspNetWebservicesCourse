using FluentValidation;
using TechConf.Api.Models;

namespace TechConf.Api.Validation;

public class CreateEventValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Date)
            .Must(date => date.Date > DateTime.Today)
            .WithMessage("Event date must be in the future");
    }
}
