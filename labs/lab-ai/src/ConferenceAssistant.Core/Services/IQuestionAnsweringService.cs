namespace ConferenceAssistant.Core.Services;

public interface IQuestionAnsweringService
{
    Task GenerateAiAnswerAsync(string questionId, string questionText, string? topicId = null);
    Task<string?> GenerateAiAnswerTextAsync(string questionText, string? topicId = null);
    Task<bool> IsQuestionSafeAsync(string questionText);
}
