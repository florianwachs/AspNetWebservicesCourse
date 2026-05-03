namespace ConferenceAssistant.Ingestion.Models;

/// <summary>
/// Tracks which documents have been ingested into the vector store,
/// enabling incremental/resumable ingestion.
/// </summary>
public class IngestionRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>The document identifier used in the vector store (e.g., file path).</summary>
    public string DocumentId { get; set; } = "";

    /// <summary>Source repository or origin (e.g., "github:owner/repo/branch").</summary>
    public string Source { get; set; } = "";

    /// <summary>SHA-256 hash of the document content for change detection.</summary>
    public string ContentHash { get; set; } = "";

    /// <summary>Processing status: Pending, Completed, Failed.</summary>
    public IngestionStatus Status { get; set; } = IngestionStatus.Pending;

    /// <summary>Error message if processing failed.</summary>
    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum IngestionStatus
{
    Pending,
    Completed,
    Failed
}
