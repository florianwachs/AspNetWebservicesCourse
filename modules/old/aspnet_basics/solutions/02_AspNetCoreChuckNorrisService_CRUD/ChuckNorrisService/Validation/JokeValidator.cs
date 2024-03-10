using ChuckNorrisService.Models;
using FluentValidation;

namespace ChuckNorrisService.Validation
{
    public class JokeValidator : AbstractValidator<Joke>
    {
        public JokeValidator(bool needsId)
        {
            if (needsId)
            {
                RuleFor(j => j.Id).NotEmpty();
            }

            RuleFor(j => j.Value).NotEmpty().MaximumLength(500);
        }
    }
}
