using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TechConf.Api.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "https://speaker-portal.techconf.dev";
    public string Audience { get; init; } = "speaker-portal-api";
    public string Key { get; init; } = "reference-solution-dev-key-change-me-32chars-minimum";
    public int AccessTokenExpirationMinutes { get; init; } = 120;

    public SymmetricSecurityKey GetSecurityKey() =>
        new(Encoding.UTF8.GetBytes(Key));
}
