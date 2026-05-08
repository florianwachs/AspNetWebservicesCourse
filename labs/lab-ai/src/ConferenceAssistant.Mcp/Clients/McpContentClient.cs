using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Microsoft.Extensions.Logging;

namespace ConferenceAssistant.Mcp.Clients;

public class McpContentClient(ILoggerFactory loggerFactory) : IMcpContentClient
{
    private McpClient? _learnClient;
    private McpClient? _deepWikiClient;
    private readonly ILogger _logger = loggerFactory.CreateLogger<McpContentClient>();

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        // Connect to Microsoft Learn MCP Server (docs search, fetch, code samples)
        try
        {
            var learnTransport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Endpoint = new Uri("https://learn.microsoft.com/api/mcp"),
                TransportMode = HttpTransportMode.StreamableHttp
            }, loggerFactory);
            _learnClient = await McpClient.CreateAsync(learnTransport, null, loggerFactory, ct);
            _logger.LogInformation("Connected to Microsoft Learn MCP server");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to Microsoft Learn MCP — doc-augmented answers unavailable");
        }

        // Connect to DeepWiki MCP Server (GitHub repo knowledge)
        try
        {
            var deepWikiTransport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Endpoint = new Uri("https://mcp.deepwiki.com/mcp"),
                TransportMode = HttpTransportMode.StreamableHttp
            }, loggerFactory);
            _deepWikiClient = await McpClient.CreateAsync(deepWikiTransport, null, loggerFactory, ct);
            _logger.LogInformation("Connected to DeepWiki MCP server");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to DeepWiki MCP — wiki-augmented answers unavailable");
        }
    }

    public async Task<string?> SearchDocsAsync(string query, CancellationToken ct = default)
    {
        if (_learnClient is null) return null;
        try
        {
            var result = await _learnClient.CallToolAsync(
                "microsoft_docs_search",
                new Dictionary<string, object?> { ["query"] = query },
                cancellationToken: ct);
            return ExtractText(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MS Learn search failed for: {Query}", query);
            return null;
        }
    }

    public async Task<string?> FetchDocAsync(string url, CancellationToken ct = default)
    {
        if (_learnClient is null) return null;
        try
        {
            var result = await _learnClient.CallToolAsync(
                "microsoft_docs_fetch",
                new Dictionary<string, object?> { ["url"] = url },
                cancellationToken: ct);
            return ExtractText(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MS Learn fetch failed for: {Url}", url);
            return null;
        }
    }

    public async Task<string?> AskDeepWikiAsync(string repo, string question, CancellationToken ct = default)
    {
        if (_deepWikiClient is null) return null;
        try
        {
            var result = await _deepWikiClient.CallToolAsync(
                "ask_question",
                new Dictionary<string, object?> { ["repoName"] = repo, ["question"] = question },
                cancellationToken: ct);
            return ExtractText(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "DeepWiki ask failed for repo {Repo}", repo);
            return null;
        }
    }

    private static string? ExtractText(CallToolResult result)
    {
        foreach (var block in result.Content)
        {
            if (block is TextContentBlock text)
                return text.Text;
        }
        return null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_learnClient is not null) await _learnClient.DisposeAsync();
        if (_deepWikiClient is not null) await _deepWikiClient.DisposeAsync();
    }
}
