namespace ConferenceAssistant.Core.Models;

public class Slide
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? TopicId { get; set; }
    public int Order { get; set; }
    public SlideType Type { get; set; } = SlideType.Content;
    public SlideLayout Layout { get; set; } = SlideLayout.Default;
    public string Title { get; set; } = "";
    public string? Subtitle { get; set; }
    public List<string> Bullets { get; set; } = [];
    public string? BodyMarkdown { get; set; }
    public string? CodeSnippet { get; set; }
    public string? CodeLanguage { get; set; }
    public string SpeakerNotes { get; set; } = "";
}

public enum SlideType
{
    Title,
    Content,
    Code,
    Section,
    Poll,
    Blank
}

public enum SlideLayout
{
    Default,
    Centered,
    TwoColumn
}
