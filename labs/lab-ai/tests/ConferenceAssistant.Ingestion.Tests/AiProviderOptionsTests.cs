using ConferenceAssistant.Ingestion.Services;

namespace ConferenceAssistant.Ingestion.Tests;

public class AiProviderOptionsTests
{
    [Fact]
    public void Resolve_WithoutProvider_ThrowsClearMessage()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new AiProviderOptions().Resolve());

        Assert.Contains("AI:Provider is required", exception.Message);
    }

    [Fact]
    public void Resolve_WithUnsupportedProvider_ThrowsClearMessage()
    {
        var options = new AiProviderOptions { Provider = "AzureOpenAI" };

        var exception = Assert.Throws<InvalidOperationException>(() => options.Resolve());

        Assert.Contains("Unsupported AI:Provider 'AzureOpenAI'", exception.Message);
    }

    [Fact]
    public void Resolve_GitHubModelsDefaults_UsesExpectedDimensions()
    {
        var options = new AiProviderOptions
        {
            Provider = "GitHubModels",
            ApiKey = "github_pat_test"
        };

        var resolved = options.Resolve();

        Assert.Equal(AiProvider.GitHubModels, resolved.Provider);
        Assert.Equal("openai/gpt-4.1-mini", resolved.ChatModel);
        Assert.Equal("openai/text-embedding-3-small", resolved.EmbeddingModel);
        Assert.Equal(1536, resolved.EmbeddingDimensions);
        Assert.Equal("conference_knowledge_github_models", resolved.VectorCollectionName);
    }

    [Fact]
    public void Resolve_OllamaDefaults_UsesExpectedDimensions()
    {
        var options = new AiProviderOptions { Provider = "Ollama" };

        var resolved = options.Resolve();

        Assert.Equal(AiProvider.Ollama, resolved.Provider);
        Assert.Equal("qwen2.5:0.5b-instruct", resolved.ChatModel);
        Assert.Equal("nomic-embed-text", resolved.EmbeddingModel);
        Assert.Equal(768, resolved.EmbeddingDimensions);
        Assert.Equal("conference_knowledge_ollama", resolved.VectorCollectionName);
    }
}
