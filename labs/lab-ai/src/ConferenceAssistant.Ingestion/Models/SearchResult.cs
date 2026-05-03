namespace ConferenceAssistant.Ingestion.Models;

/// <summary>
/// Lightweight search result from the vector store.
/// </summary>
public record SearchResult(
    string Content,
    string? Context = null,
    string? Source = null,
    string? DocumentId = null);
