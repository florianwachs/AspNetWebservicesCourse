using System.Runtime.CompilerServices;
using Microsoft.Extensions.DataIngestion;
using ConferenceAssistant.Ingestion.Utilities;

namespace ConferenceAssistant.Ingestion.Enrichers;

/// <summary>
/// An <see cref="IngestionChunkProcessor{T}"/> that enriches chunks with metadata from
/// YAML front matter. Unlike SummaryEnricher/KeywordEnricher (which call an LLM), this is
/// a purely metadata-based enricher — no AI required.
/// </summary>
/// <remarks>
/// Front matter is registered per-document via <see cref="AddFrontMatter"/> before processing.
/// During processing, the enricher looks up front matter by <c>chunk.Document.Identifier</c>
/// and adds technology keywords, category, and job description to chunk metadata.
/// </remarks>
public class FrontMatterChunkProcessor : IngestionChunkProcessor<string>
{
    /// <summary>Metadata key for technology keywords extracted from front matter.</summary>
    public static string TechnologiesKey => "front_matter_technologies";

    /// <summary>Metadata key for category extracted from front matter.</summary>
    public static string CategoryKey => "front_matter_category";

    /// <summary>Metadata key for job/description extracted from front matter.</summary>
    public static string JobKey => "front_matter_job";

    private readonly Dictionary<string, FrontMatter> _frontMatterByKey = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers front matter for a document identifier. Call this before processing chunks.
    /// </summary>
    public void AddFrontMatter(string documentIdentifier, FrontMatter? frontMatter)
    {
        if (frontMatter is not null)
            _frontMatterByKey[documentIdentifier] = frontMatter;
    }

    /// <inheritdoc />
    public override async IAsyncEnumerable<IngestionChunk<string>> ProcessAsync(
        IAsyncEnumerable<IngestionChunk<string>> chunks,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var chunk in chunks.WithCancellation(cancellationToken))
        {
            if (_frontMatterByKey.TryGetValue(chunk.Document.Identifier, out var fm))
            {
                if (fm.Technologies.Count > 0)
                    chunk.Metadata[TechnologiesKey] = fm.Technologies.ToArray();

                if (!string.IsNullOrWhiteSpace(fm.Category))
                    chunk.Metadata[CategoryKey] = fm.Category;

                if (!string.IsNullOrWhiteSpace(fm.Job))
                    chunk.Metadata[JobKey] = fm.Job;
            }

            yield return chunk;
        }
    }
}
