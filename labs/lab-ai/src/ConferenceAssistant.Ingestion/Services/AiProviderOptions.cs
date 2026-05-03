namespace ConferenceAssistant.Ingestion.Services;

public enum AiProvider
{
    GitHubModels,
    Ollama
}

public sealed class AiProviderOptions
{
    public const string SectionName = "AI";

    public string? Provider { get; set; }
    public string? Endpoint { get; set; }
    public string? ApiKey { get; set; }
    public string? ChatModel { get; set; }
    public string? EmbeddingModel { get; set; }
    public int? EmbeddingDimensions { get; set; }
    public string? VectorCollectionName { get; set; }

    public ResolvedAiProviderOptions Resolve(string? fallbackGitHubToken = null)
    {
        if (string.IsNullOrWhiteSpace(Provider))
        {
            throw new InvalidOperationException(
                "AI:Provider is required. Set it to 'GitHubModels' or 'Ollama'.");
        }

        if (!Enum.TryParse<AiProvider>(Provider, ignoreCase: true, out var provider))
        {
            throw new InvalidOperationException(
                $"Unsupported AI:Provider '{Provider}'. Supported providers are 'GitHubModels' and 'Ollama'.");
        }

        return provider switch
        {
            AiProvider.GitHubModels => ResolveGitHubModels(fallbackGitHubToken),
            AiProvider.Ollama => ResolveOllama(),
            _ => throw new InvalidOperationException($"Unsupported AI provider '{provider}'.")
        };
    }

    private ResolvedAiProviderOptions ResolveGitHubModels(string? fallbackGitHubToken)
    {
        var apiKey = FirstNonEmpty(ApiKey, fallbackGitHubToken);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "AI:ApiKey is required for GitHub Models. Use a GitHub token with 'models: read' permission.");
        }

        return new ResolvedAiProviderOptions(
            AiProvider.GitHubModels,
            Endpoint: CreateEndpoint(Endpoint, "https://models.github.ai/inference"),
            ApiKey: apiKey,
            ChatModel: FirstNonEmpty(ChatModel, "openai/gpt-4.1-mini"),
            EmbeddingModel: FirstNonEmpty(EmbeddingModel, "openai/text-embedding-3-small"),
            EmbeddingDimensions: ValidateDimensions(EmbeddingDimensions, 1536),
            VectorCollectionName: FirstNonEmpty(VectorCollectionName, "conference_knowledge_github_models"));
    }

    private ResolvedAiProviderOptions ResolveOllama()
    {
        return new ResolvedAiProviderOptions(
            AiProvider.Ollama,
            Endpoint: CreateEndpoint(Endpoint, "http://localhost:11434/v1"),
            ApiKey: FirstNonEmpty(ApiKey, "ollama"),
            ChatModel: FirstNonEmpty(ChatModel, "llama3.2:3b"),
            EmbeddingModel: FirstNonEmpty(EmbeddingModel, "embeddinggemma"),
            EmbeddingDimensions: ValidateDimensions(EmbeddingDimensions, 768),
            VectorCollectionName: FirstNonEmpty(VectorCollectionName, "conference_knowledge_ollama"));
    }

    private static Uri CreateEndpoint(string? configuredEndpoint, string defaultEndpoint)
    {
        var endpoint = FirstNonEmpty(configuredEndpoint, defaultEndpoint);
        return Uri.TryCreate(endpoint, UriKind.Absolute, out var uri)
            ? uri
            : throw new InvalidOperationException($"AI:Endpoint '{endpoint}' must be an absolute URI.");
    }

    private static int ValidateDimensions(int? configuredDimensions, int defaultDimensions)
    {
        var dimensions = configuredDimensions ?? defaultDimensions;
        if (dimensions <= 0)
        {
            throw new InvalidOperationException("AI:EmbeddingDimensions must be greater than zero.");
        }

        return dimensions;
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        throw new InvalidOperationException("Expected at least one non-empty value.");
    }
}

public sealed record ResolvedAiProviderOptions(
    AiProvider Provider,
    Uri Endpoint,
    string ApiKey,
    string ChatModel,
    string EmbeddingModel,
    int EmbeddingDimensions,
    string VectorCollectionName);
