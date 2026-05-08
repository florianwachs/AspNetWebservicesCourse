using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Web.Services;

public interface ISlideGenerationService
{
    /// <summary>
    /// Generates slide markdown deterministically from a session draft.
    /// </summary>
    string GenerateSlideMarkdown(SessionDraft draft);

    /// <summary>
    /// Generates AI-enhanced slide markdown with richer speaker notes and code examples.
    /// </summary>
    Task<string> GenerateEnhancedSlideMarkdownAsync(
        SessionDraft draft, IReadOnlyList<ImportedDocument>? documents = null);
}
