namespace ConferenceAssistant.Core.Models;

public class Insight
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? PollId { get; set; }
    public string? TopicId { get; set; }
    public string Content { get; set; } = "";
    public InsightType Type { get; set; }
    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;
}

public enum InsightType { PollAnalysis, AudienceTrend, KnowledgeGap, TopicSummary, SessionSummary }
