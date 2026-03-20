using FluentValidation;
using TechConf.Api.Models;

namespace TechConf.Api.Validation;

public class CreateEventValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventValidator()
    {
        // TODO: Task 5 - Add validation rules
        // - Name: NotEmpty, MaxLength 200
        // - Date: Must be in the future
        // - City: NotEmpty, MaxLength 100
    }
}
