namespace TechConf.Api.Models;

public sealed class ConferenceEvent
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTimeOffset ProposalDeadlineUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public List<Proposal> Proposals { get; set; } = [];
}
