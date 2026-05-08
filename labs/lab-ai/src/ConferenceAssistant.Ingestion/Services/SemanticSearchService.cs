using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Ingestion.Services;

public class SemanticSearchService : ISemanticSearchService
{
    private readonly QdrantClient _qdrantClient;
    private readonly QdrantVectorStoreOptions _storeOptions;
    private readonly QdrantVectorStore _vectorStore;
    private readonly ResolvedAiProviderOptions _aiOptions;
    private VectorStoreCollection<object, Dictionary<string, object?>>? _collection;
    private int _recordCount;

    public SemanticSearchService(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        QdrantClient qdrantClient,
        ResolvedAiProviderOptions aiOptions)
    {
        _qdrantClient = qdrantClient;
        _aiOptions = aiOptions;
        _storeOptions = new QdrantVectorStoreOptions { EmbeddingGenerator = embeddingGenerator };
        // Pass embedding generator so dynamic collections can auto-embed string → vector
        _vectorStore = new QdrantVectorStore(qdrantClient, ownsClient: false, _storeOptions);
    }

    /// <summary>
    /// Creates a new VectorStore instance for pipeline use. Each call returns a fresh instance
    /// so callers (e.g. VectorStoreWriter) can safely dispose it without affecting shared state.
    /// </summary>
    public VectorStore VectorStore => new QdrantVectorStore(_qdrantClient, ownsClient: false, _storeOptions);

    private async Task<VectorStoreCollection<object, Dictionary<string, object?>>> GetCollectionAsync()
    {
        if (_collection is not null)
            return _collection;

        // Schema matches VectorStoreWriter<string>'s dynamic layout + extra "source" field
        var definition = new VectorStoreCollectionDefinition();
        definition.Properties.Add(new VectorStoreKeyProperty("key", typeof(Guid)));
        definition.Properties.Add(new VectorStoreVectorProperty("embedding", typeof(string), _aiOptions.EmbeddingDimensions)
        {
            DistanceFunction = DistanceFunction.CosineSimilarity
        });
        definition.Properties.Add(new VectorStoreDataProperty("content", typeof(string)));
        definition.Properties.Add(new VectorStoreDataProperty("context", typeof(string)));
        definition.Properties.Add(new VectorStoreDataProperty("documentid", typeof(string)));
        definition.Properties.Add(new VectorStoreDataProperty("source", typeof(string)));

        _collection = _vectorStore.GetDynamicCollection(_aiOptions.VectorCollectionName, definition);
        await _collection.EnsureCollectionExistsAsync();
        return _collection;
    }

    public async Task<IReadOnlyList<SearchResult>> SearchAsync(
        string query, int topK = 5, string? sourceFilter = null)
    {
        var collection = await GetCollectionAsync();

        var results = collection.SearchAsync(query, topK);

        var records = new List<SearchResult>();
        await foreach (var result in results)
        {
            var dict = result.Record;
            var content = dict.TryGetValue("content", out var c) ? c as string : null;
            var context = dict.TryGetValue("context", out var ctx) ? ctx as string : null;
            var source = dict.TryGetValue("source", out var src) ? src as string : null;
            var docId = dict.TryGetValue("documentid", out var did) ? did as string : null;

            if (sourceFilter is not null && source != sourceFilter)
                continue;

            if (!string.IsNullOrWhiteSpace(content))
                records.Add(new SearchResult(content, context, source, docId));
        }

        return records;
    }

    public async Task UpsertAsync(string content, string? source = null, string? documentId = null)
    {
        var collection = await GetCollectionAsync();

        var record = new Dictionary<string, object?>
        {
            ["key"] = Guid.NewGuid(),
            ["content"] = content,
            ["embedding"] = content,  // auto-embedded via IEmbeddingGenerator
            ["context"] = "",
            ["documentid"] = documentId ?? Guid.NewGuid().ToString(),
            ["source"] = source ?? ""
        };

        await collection.UpsertAsync(record);
        Interlocked.Increment(ref _recordCount);
    }

    public async Task<int> GetRecordCountAsync()
    {
        await GetCollectionAsync();
        return _recordCount;
    }
}
