namespace ConferenceAssistant.Core.Models;

public class SessionTopic
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int Order { get; set; }
    public TopicStatus Status { get; set; } = TopicStatus.Upcoming;
    public List<string> TalkingPoints { get; set; } = [];
    public List<Slide> Slides { get; set; } = [];
    public List<SuggestedPoll> SuggestedPolls { get; set; } = [];
}

public enum TopicStatus { Upcoming, Active, Completed }

public class SuggestedPoll
{
    public string Question { get; set; } = "";
    public List<string> Options { get; set; } = [];
}
