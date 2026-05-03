namespace ConferenceAssistant.Mcp.Clients;

public interface IMcpContentClient : IAsyncDisposable
{
    Task InitializeAsync(CancellationToken ct = default);
    Task<string?> SearchDocsAsync(string query, CancellationToken ct = default);
    Task<string?> FetchDocAsync(string url, CancellationToken ct = default);
    Task<string?> AskDeepWikiAsync(string repo, string question, CancellationToken ct = default);
}
