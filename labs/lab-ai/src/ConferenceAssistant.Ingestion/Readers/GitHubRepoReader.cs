using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DataIngestion;
using Microsoft.Extensions.Logging;

namespace ConferenceAssistant.Ingestion.Readers;

/// <summary>
/// Options for configuring which GitHub repository and path to read markdown files from.
/// </summary>
public record GitHubRepoOptions(
    string Owner,
    string Repo,
    string? Subdirectory = null,
    string? Branch = null);

/// <summary>
/// An <see cref="IngestionDocumentReader"/> that fetches markdown files from a public GitHub repository
/// and converts them into <see cref="IngestionDocument"/> objects for the ingestion pipeline.
/// </summary>
public class GitHubRepoReader : IngestionDocumentReader
{
    private const string UserAgent = "ConferencePulse/1.0";
    private const string GitHubApiBase = "https://api.github.com";
    private const string GitHubRawBase = "https://raw.githubusercontent.com";

    private readonly HttpClient _httpClient;
    private readonly string _owner;
    private readonly string _repo;
    private readonly string? _subdirectory;
    private string? _branch;
    private readonly ILogger<GitHubRepoReader>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubRepoReader"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to make requests to the GitHub API.</param>
    /// <param name="owner">The GitHub repository owner.</param>
    /// <param name="repo">The GitHub repository name.</param>
    /// <param name="subdirectory">Optional subdirectory within the repo to scope file discovery.</param>
    /// <param name="branch">The branch to read from. Defaults to "main".</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    public GitHubRepoReader(
        HttpClient httpClient,
        string owner,
        string repo,
        string? subdirectory = null,
        string? branch = null,
        ILogger<GitHubRepoReader>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentException.ThrowIfNullOrEmpty(owner);
        ArgumentException.ThrowIfNullOrEmpty(repo);

        _httpClient = httpClient;
        _owner = owner;
        _repo = repo;
        _subdirectory = subdirectory?.TrimEnd('/');
        _branch = branch;
        _logger = logger;

        if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubRepoReader"/> class from options.
    /// </summary>
    public GitHubRepoReader(HttpClient httpClient, GitHubRepoOptions options, ILogger<GitHubRepoReader>? logger = null)
        : this(httpClient, options.Owner, options.Repo, options.Subdirectory, options.Branch, logger)
    {
    }

    /// <summary>
    /// Reads a stream of markdown content and converts it to an <see cref="IngestionDocument"/>.
    /// </summary>
    public override async Task<IngestionDocument> ReadAsync(
        Stream source,
        string identifier,
        string mediaType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrEmpty(identifier);

        using var reader = new StreamReader(source, leaveOpen: true);
        var content = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

        return CreateDocumentFromMarkdown(content, identifier);
    }

    /// <summary>
    /// Replaces HTML entities with their text equivalents.
    /// The DataIngestion MarkdownReader doesn't support HtmlEntityInline nodes.
    /// </summary>
    private static string SanitizeHtmlEntities(string markdown) =>
        System.Text.RegularExpressions.Regex.Replace(markdown, @"&[a-zA-Z]+;|&#\d+;|&#x[0-9a-fA-F]+;",
            m => WebUtility.HtmlDecode(m.Value) ?? m.Value);

    /// <summary>
    /// Downloads all markdown files from the repository to a temp directory.
    /// Returns the temp directory path, file list, and collected documents for drafting.
    /// The caller is responsible for deleting the temp directory.
    /// </summary>
    public async Task<GitHubDownloadResult> DownloadToDirectoryAsync(
        CancellationToken cancellationToken = default)
    {
        var filePaths = await GetMarkdownFilePathsAsync(cancellationToken).ConfigureAwait(false);

        _logger?.LogInformation(
            "Found {Count} markdown files in {Owner}/{Repo}/{Branch}{Subdirectory}",
            filePaths.Count, _owner, _repo, _branch,
            _subdirectory is not null ? $"/{_subdirectory}" : string.Empty);

        var tempDir = Path.Combine(Path.GetTempPath(), $"conference-pulse-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        var downloadedFiles = new List<FileInfo>();
        var documents = new List<GitHubDownloadedDocument>();
        var errors = new List<string>();

        foreach (var path in filePaths)
        {
            try
            {
                var content = await DownloadFileContentAsync(path, cancellationToken).ConfigureAwait(false);
                if (content is null) continue;

                // Sanitize HTML entities that MarkdownReader's parser doesn't support
                content = SanitizeHtmlEntities(content);

                // Write to temp file (preserve directory structure)
                var localPath = Path.Combine(tempDir, path.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
                await File.WriteAllTextAsync(localPath, content, cancellationToken).ConfigureAwait(false);
                downloadedFiles.Add(new FileInfo(localPath));

                // Collect raw content for AI drafting
                var parsed = Utilities.MarkdownFrontMatterParser.Parse(content);
                documents.Add(new GitHubDownloadedDocument(path, content, parsed.FrontMatter));

                _logger?.LogDebug("Downloaded {Path} to temp directory", path);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _logger?.LogWarning("GitHub API rate limit reached while downloading {Path}. Stopping.", path);
                errors.Add($"{path}: Rate limit exceeded");
                break;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger?.LogWarning(ex, "Failed to download {Path}, skipping.", path);
                errors.Add($"{path}: {ex.Message}");
            }
        }

        _logger?.LogInformation("Downloaded {Count} files to {TempDir}", downloadedFiles.Count, tempDir);
        return new GitHubDownloadResult(tempDir, downloadedFiles, documents, errors);
    }

    /// <summary>
    /// Fetches all markdown files from the configured GitHub repository and yields
    /// an <see cref="IngestionDocument"/> for each file. This is the primary entry point
    /// for bulk-reading GitHub content into the ingestion pipeline.
    /// </summary>
    public async IAsyncEnumerable<IngestionDocument> ReadAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var filePaths = await GetMarkdownFilePathsAsync(cancellationToken).ConfigureAwait(false);

        _logger?.LogInformation(
            "Found {Count} markdown files in {Owner}/{Repo}/{Branch}{Subdirectory}",
            filePaths.Count, _owner, _repo, _branch,
            _subdirectory is not null ? $"/{_subdirectory}" : string.Empty);

        foreach (var path in filePaths)
        {
            IngestionDocument? document = null;
            try
            {
                var content = await DownloadFileContentAsync(path, cancellationToken).ConfigureAwait(false);
                if (content is not null)
                {
                    document = CreateDocumentFromMarkdown(content, path);
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _logger?.LogWarning(
                    "GitHub API rate limit reached while downloading {Path}. Stopping enumeration.", path);
                yield break;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger?.LogDebug("File not found, skipping: {Path}", path);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger?.LogWarning(ex, "Failed to download {Path}, skipping.", path);
            }

            if (document is not null)
            {
                yield return document;
            }
        }
    }

    /// <summary>
    /// Fetches the repository file tree from the GitHub API and returns paths of markdown files.
    /// </summary>
    private async Task<IReadOnlyList<string>> GetMarkdownFilePathsAsync(CancellationToken cancellationToken)
    {
        // Auto-detect default branch if not explicitly provided
        if (string.IsNullOrEmpty(_branch))
        {
            _branch = await ResolveDefaultBranchAsync(cancellationToken).ConfigureAwait(false);
            _logger?.LogInformation("Auto-detected default branch: {Branch} for {Owner}/{Repo}", _branch, _owner, _repo);
        }

        var url = $"{GitHubApiBase}/repos/{_owner}/{_repo}/git/trees/{_branch}?recursive=1";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd(UserAgent);

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            _logger?.LogWarning("GitHub API rate limit reached when fetching repository tree.");
            return [];
        }

        response.EnsureSuccessStatusCode();

        var tree = await response.Content.ReadFromJsonAsync<GitHubTreeResponse>(
            JsonOptions, cancellationToken).ConfigureAwait(false);

        if (tree?.Tree is null)
        {
            return [];
        }

        var paths = new List<string>();
        foreach (var item in tree.Tree)
        {
            if (item.Type != "blob" || item.Path is null)
            {
                continue;
            }

            if (!item.Path.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (_subdirectory is not null &&
                !item.Path.StartsWith(_subdirectory + "/", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            paths.Add(item.Path);
        }

        return paths;
    }

    /// <summary>
    /// Resolves the repository's default branch by querying the GitHub API.
    /// Falls back to "main" if the API call fails.
    /// </summary>
    private async Task<string> ResolveDefaultBranchAsync(CancellationToken cancellationToken)
    {
        try
        {
            var url = $"{GitHubApiBase}/repos/{_owner}/{_repo}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd(UserAgent);

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var repoInfo = await response.Content.ReadFromJsonAsync<GitHubRepoResponse>(
                JsonOptions, cancellationToken).ConfigureAwait(false);

            return repoInfo?.DefaultBranch ?? "main";
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to detect default branch for {Owner}/{Repo}, falling back to 'main'", _owner, _repo);
            return "main";
        }
    }

    /// <summary>
    /// Downloads the raw content of a single file from GitHub.
    /// </summary>
    private async Task<string?> DownloadFileContentAsync(string path, CancellationToken cancellationToken)
    {
        var url = $"{GitHubRawBase}/{_owner}/{_repo}/{_branch}/{path}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd(UserAgent);

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            _logger?.LogWarning("GitHub API rate limit reached while downloading {Path}.", path);
            throw new HttpRequestException("Rate limit exceeded", null, HttpStatusCode.Forbidden);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger?.LogDebug("File not found on GitHub: {Path}", path);
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates an <see cref="IngestionDocument"/> from raw markdown content.
    /// The full text is wrapped in a single section/paragraph so that downstream
    /// chunkers (e.g. <see cref="HeaderChunker"/>) can split it appropriately.
    /// </summary>
    private static IngestionDocument CreateDocumentFromMarkdown(string markdownContent, string identifier)
    {
        var document = new IngestionDocument(identifier);
        var section = new IngestionDocumentSection();
        section.Elements.Add(new IngestionDocumentParagraph(markdownContent) { Text = markdownContent });
        document.Sections.Add(section);
        return document;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class GitHubTreeResponse
    {
        [JsonPropertyName("sha")]
        public string? Sha { get; set; }

        [JsonPropertyName("tree")]
        public List<GitHubTreeItem>? Tree { get; set; }

        [JsonPropertyName("truncated")]
        public bool Truncated { get; set; }
    }

    private sealed class GitHubRepoResponse
    {
        [JsonPropertyName("default_branch")]
        public string? DefaultBranch { get; set; }
    }

    private sealed class GitHubTreeItem
    {
        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("mode")]
        public string? Mode { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("sha")]
        public string? Sha { get; set; }

        [JsonPropertyName("size")]
        public long? Size { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}

/// <summary>
/// Result of downloading GitHub files to a local temp directory.
/// </summary>
public record GitHubDownloadResult(
    string TempDirectory,
    IReadOnlyList<FileInfo> Files,
    IReadOnlyList<GitHubDownloadedDocument> Documents,
    IReadOnlyList<string> Errors) : IDisposable
{
    public void Dispose()
    {
        try { if (Directory.Exists(TempDirectory)) Directory.Delete(TempDirectory, recursive: true); }
        catch { /* best effort cleanup */ }
    }
}

public record GitHubDownloadedDocument(
    string FilePath,
    string Content,
    ConferenceAssistant.Ingestion.Utilities.FrontMatter? FrontMatter);
