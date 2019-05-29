using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ChuckNorrisService.Models
{
    public class Joke
    {
        public string Id { get; set; }
        [Required]
        [StringLength(500)]
        public string JokeText { get; set; }
        public IReadOnlyCollection<JokeCategory> Categories { get; set; }


    }

    public class JokeValidator : AbstractValidator<Joke>
    {
        public JokeValidator()
        {
            RuleFor(j => j.JokeText).NotEmpty().MaximumLength(500);
            When(j => j.Categories != null && j.Categories.Any(), () =>
              {
                  RuleFor(j => j.Categories).Must(BeUnique).WithMessage("Categories must be unique");
              });
        }

        private bool BeUnique(IReadOnlyCollection<JokeCategory> categories)
        {
            return categories.Count == categories.Distinct().Count();
        }
    }

    public class JokeDto
    {
        public string Id { get; set; }
        public string JokeText { get; set; }
        public string[] Category { get; set; }
    }
}
