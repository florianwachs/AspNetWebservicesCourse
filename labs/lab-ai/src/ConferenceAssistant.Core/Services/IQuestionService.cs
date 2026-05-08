using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public interface IQuestionService
{
    event Action<AudienceQuestion>? QuestionReceived;
    event Action<AudienceQuestion>? QuestionAnswered;
    event Action<AudienceQuestion>? QuestionUpvoted;

    Task<AudienceQuestion> SubmitQuestionAsync(string text, string? topicId = null, string? attendeeId = null);
    Task<AudienceQuestion?> AnswerQuestionAsync(string questionId, string answer, bool isAiGenerated = false, string authorLabel = "Presenter");
    Task UpvoteQuestionAsync(string questionId);
    IReadOnlyList<AudienceQuestion> GetQuestionsForTopic(string topicId);
    IReadOnlyList<AudienceQuestion> GetTopQuestions(int count = 10);
}
