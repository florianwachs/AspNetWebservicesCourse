var builder = DistributedApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// AI provider — GitHub Models or local Ollama
// Set AI:Provider to "GitHubModels" or "Ollama".
// ---------------------------------------------------------------------------
var aiProvider = builder.Configuration["AI:Provider"] ?? throw new InvalidOperationException(
    "AI:Provider is required. Set it to 'GitHubModels' or 'Ollama'.");

// PostgreSQL for EF Core persistence (sessions, polls, Q&A, insights)
var postgres = builder.AddPostgres("postgres")
    .WithPgWeb()
    .WithDataVolume();

var conferenceDb = postgres.AddDatabase("conferencedb");

// Qdrant for vector/embedding storage (semantic search)
var qdrant = builder.AddQdrant("qdrant")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var web = builder.AddProject<Projects.ConferenceAssistant_Web>("web")
    .WithReference(conferenceDb)
    .WithReference(qdrant)
    .WaitFor(conferenceDb)
    .WaitFor(qdrant);

switch (aiProvider.Trim().ToLowerInvariant())
{
    case "githubmodels":
    case "github-models":
    {
        var chatModel = GetConfigurationValue("AI:ChatModel", "openai/gpt-4.1-mini");
        var embeddingModel = GetConfigurationValue("AI:EmbeddingModel", "openai/text-embedding-3-small");
        var embeddingDimensions = GetConfigurationInt("AI:EmbeddingDimensions", 1536);
        var vectorCollectionName = GetConfigurationValue("AI:VectorCollectionName", "conference_knowledge_github_models");
        var githubApiKey = builder.AddParameterFromConfiguration("github-models-api-key", "AI:ApiKey", secret: true);

        var chat = builder.AddGitHubModel("github-chat", chatModel).WithApiKey(githubApiKey);
        var embedding = builder.AddGitHubModel("github-embedding", embeddingModel).WithApiKey(githubApiKey);

        web = web
            .WithReference(chat)
            .WithReference(embedding)
            .WithEnvironment("AI__Provider", "GitHubModels")
            .WithEnvironment("AI__Endpoint", chat.Resource.UriExpression)
            .WithEnvironment("AI__ApiKey", githubApiKey)
            .WithEnvironment("AI__ChatModel", chatModel)
            .WithEnvironment("AI__EmbeddingModel", embeddingModel)
            .WithEnvironment("AI__EmbeddingDimensions", embeddingDimensions.ToString())
            .WithEnvironment("AI__VectorCollectionName", vectorCollectionName);
        break;
    }

    case "ollama":
    {
        var chatModel = GetConfigurationValue("AI:ChatModel", "llama3.2:3b");
        var embeddingModel = GetConfigurationValue("AI:EmbeddingModel", "embeddinggemma");
        var embeddingDimensions = GetConfigurationInt("AI:EmbeddingDimensions", 768);
        var vectorCollectionName = GetConfigurationValue("AI:VectorCollectionName", "conference_knowledge_ollama");

        var ollama = builder.AddOllama("ollama")
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent);

        var ollamaChat = ollama.AddModel("ollama-chat", chatModel);
        var ollamaEmbedding = ollama.AddModel("ollama-embedding", embeddingModel);

        web = web
            .WithReference(ollamaChat)
            .WithReference(ollamaEmbedding)
            .WithEnvironment("AI__Provider", "Ollama")
            .WithEnvironment("AI__Endpoint", $"{ollama.Resource.UriExpression}/v1")
            .WithEnvironment("AI__ApiKey", "ollama")
            .WithEnvironment("AI__ChatModel", chatModel)
            .WithEnvironment("AI__EmbeddingModel", embeddingModel)
            .WithEnvironment("AI__EmbeddingDimensions", embeddingDimensions.ToString())
            .WithEnvironment("AI__VectorCollectionName", vectorCollectionName)
            .WaitFor(ollamaChat)
            .WaitFor(ollamaEmbedding);
        break;
    }

    default:
        throw new InvalidOperationException(
            $"Unsupported AI:Provider '{aiProvider}'. Supported providers are 'GitHubModels' and 'Ollama'.");
}

// Dev tunnel — exposes the web app via a public HTTPS URL for attendees
builder.AddDevTunnel("conference-tunnel")
    .WithReference(web)
    .WithAnonymousAccess();

builder.Build().Run();

string GetConfigurationValue(string key, string defaultValue)
{
    var value = builder.Configuration[key];
    return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
}

int GetConfigurationInt(string key, int defaultValue)
{
    var value = builder.Configuration[key];
    return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
}
