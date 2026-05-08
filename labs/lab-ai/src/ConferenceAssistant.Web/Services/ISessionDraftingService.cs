using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Web.Services;

public interface ISessionDraftingService
{
    Task<SessionDraft> DraftSessionAsync(IReadOnlyList<ImportedDocument> documents, string? sessionTitle = null);
}
