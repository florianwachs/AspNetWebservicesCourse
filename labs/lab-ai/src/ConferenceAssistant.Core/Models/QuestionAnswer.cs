namespace ConferenceAssistant.Core.Models;

public class QuestionAnswer
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Text { get; set; } = "";
    public bool IsAiGenerated { get; set; }
    public string AuthorLabel { get; set; } = "Presenter";
    public DateTimeOffset AnsweredAt { get; set; } = DateTimeOffset.UtcNow;
}
