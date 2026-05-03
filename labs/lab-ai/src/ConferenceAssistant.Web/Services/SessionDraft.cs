namespace ConferenceAssistant.Web.Services;

public record SessionDraft
{
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
    public List<TopicDraft> Topics { get; init; } = [];
}

public record TopicDraft
{
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
    public List<string> TalkingPoints { get; init; } = [];
    public List<PollDraft> SuggestedPolls { get; init; } = [];
}

public record PollDraft
{
    public string Question { get; init; } = "";
    public List<string> Options { get; init; } = [];
}
