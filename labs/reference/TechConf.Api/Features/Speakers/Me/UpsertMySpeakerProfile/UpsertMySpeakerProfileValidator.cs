using FluentValidation;

namespace TechConf.Api.Features.Speakers.Me.UpsertMySpeakerProfile;

public sealed class UpsertMySpeakerProfileValidator : AbstractValidator<UpsertMySpeakerProfileCommand>
{
    public UpsertMySpeakerProfileValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Tagline).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Bio).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Company).NotEmpty().MaximumLength(120);
        RuleFor(x => x.City).NotEmpty().MaximumLength(120);
        RuleFor(x => x.WebsiteUrl)
            .Must(BeValidAbsoluteUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl))
            .WithMessage("websiteUrl must be a valid absolute URL.");
        RuleFor(x => x.PhotoUrl)
            .Must(BeValidAbsoluteUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.PhotoUrl))
            .WithMessage("photoUrl must be a valid absolute URL.");
    }

    private static bool BeValidAbsoluteUrl(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out _);
}
