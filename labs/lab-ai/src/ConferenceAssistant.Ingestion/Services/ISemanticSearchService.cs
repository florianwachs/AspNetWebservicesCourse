using ConferenceAssistant.Ingestion.Models;
using Microsoft.Extensions.VectorData;

namespace ConferenceAssistant.Ingestion.Services;

public interface ISemanticSearchService
{
    Task<IReadOnlyList<SearchResult>> SearchAsync(string query, int topK = 5, string? sourceFilter = null);
    Task UpsertAsync(string content, string? source = null, string? documentId = null);
    Task<int> GetRecordCountAsync();
    VectorStore VectorStore { get; }
}
