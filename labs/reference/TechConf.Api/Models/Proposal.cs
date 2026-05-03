namespace TechConf.Api.Models;

public sealed class Proposal
{
    public int Id { get; set; }
    public int SpeakerProfileId { get; set; }
    public SpeakerProfile SpeakerProfile { get; set; } = null!;
    public int ConferenceEventId { get; set; }
    public ConferenceEvent ConferenceEvent { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Track { get; set; } = string.Empty;
    public ProposalStatus Status { get; set; } = ProposalStatus.Draft;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public DateTimeOffset? SubmittedAtUtc { get; set; }
    public DateTimeOffset? ReviewedAtUtc { get; set; }
    public string? ReviewedByUserId { get; set; }
    public string? DecisionNote { get; set; }
}
