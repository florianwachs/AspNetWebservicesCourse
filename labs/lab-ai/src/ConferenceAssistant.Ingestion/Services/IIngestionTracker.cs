using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Ingestion.Services;

/// <summary>
/// Tracks ingestion progress for incremental/resumable imports.
/// </summary>
public interface IIngestionTracker
{
    Task<IngestionRecord?> GetRecordAsync(string documentId, string source);
    Task UpsertRecordAsync(IngestionRecord record);
    Task<IReadOnlyList<IngestionRecord>> GetRecordsForSourceAsync(string source);
}
