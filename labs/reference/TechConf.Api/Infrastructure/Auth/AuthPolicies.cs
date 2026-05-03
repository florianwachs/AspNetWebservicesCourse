namespace TechConf.Api.Infrastructure.Auth;

public static class RoleNames
{
    public const string Organizer = "Organizer";
    public const string Speaker = "Speaker";
    public const string Attendee = "Attendee";
}

public static class AppClaimTypes
{
    public const string ProposalReview = "portal:proposal_review";
    public const string SpeakerProfileWrite = "portal:speaker_profile_write";
}

public static class PolicyNames
{
    public const string SpeakerAccess = "SpeakerAccess";
    public const string SpeakerProfileWrite = "SpeakerProfileWrite";
    public const string ProposalReview = "ProposalReview";
}
