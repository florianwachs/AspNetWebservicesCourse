using FluentValidation;
using TechConf.Api.Models;

namespace TechConf.Api.Validation;

public class UpdateEventValidator : AbstractValidator<UpdateEventRequest>
{
    public UpdateEventValidator()
    {
        RuleFor(x => x.Date)
            .Must(date => date.Date > DateTime.Today)
            .WithMessage("Event date must be in the future");
    }
}
