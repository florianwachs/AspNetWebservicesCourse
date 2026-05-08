using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Web.Services;

public interface IContentImportService
{
    Task<ImportDraftResult> ImportAndDraftAsync(
        string repoUrl, string? sessionTitle = null, string? branch = null, string? subdirectory = null,
        GenerationOptions? options = null);
}

public record ImportDraftResult(
    GitHubImportResult ImportResult,
    SessionDraft Draft,
    string SlideMarkdown);
