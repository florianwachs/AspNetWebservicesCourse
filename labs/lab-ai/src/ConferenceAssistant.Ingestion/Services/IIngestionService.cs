using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Ingestion.Services;

public interface IIngestionService
{
    Task<int> IngestResponseAsync(string pollId, string topicId, string question, Dictionary<string, int> results, List<string>? otherResponses = null);
    Task<int> IngestInsightAsync(string topicId, string insightContent);
    Task<int> IngestExternalContentAsync(string source, string content);
    Task<int> IngestQuestionAsync(string questionId, string questionText, string? topicId = null);
    Task<int> IngestSessionSummaryAsync(string summaryContent);
    Task<GitHubImportResult> IngestGitHubRepoAsync(string owner, string repo, string? subdirectory = null, string? branch = null);
}
