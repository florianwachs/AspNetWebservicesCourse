using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DataIngestion;
using Microsoft.Extensions.Logging;
using Microsoft.ML.Tokenizers;
using ConferenceAssistant.Ingestion.Enrichers;
using ConferenceAssistant.Ingestion.Models;
using ConferenceAssistant.Ingestion.Readers;

namespace ConferenceAssistant.Ingestion.Services;

public class IngestionService : IIngestionService
{
    private readonly ISemanticSearchService _searchService;
    private readonly IChatClient _chatClient;
    private readonly IIngestionTracker? _tracker;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<IngestionService> _logger;

    public IngestionService(
        ISemanticSearchService searchService,
        IChatClient chatClient,
        ILoggerFactory loggerFactory,
        ILogger<IngestionService> logger,
        IIngestionTracker? tracker = null)
    {
        _searchService = searchService;
        _chatClient = chatClient;
        _loggerFactory = loggerFactory;
        _logger = logger;
        _tracker = tracker;
    }

    public async Task<int> IngestResponseAsync(
        string pollId, string topicId, string question, Dictionary<string, int> results, List<string>? otherResponses = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Poll: {question}");
        sb.AppendLine("Results:");
        var total = results.Values.Sum();
        foreach (var (option, count) in results)
        {
            var percentage = total > 0 ? (count * 100.0 / total).ToString("F1") : "0";
            sb.AppendLine($"  - {option}: {count} votes ({percentage}%)");
        }

        if (otherResponses is { Count: > 0 })
        {
            sb.AppendLine("\"Other\" responses:");
            foreach (var text in otherResponses)
                sb.AppendLine($"  - \"{text}\"");
        }

        await _searchService.UpsertAsync(sb.ToString(), source: "response", documentId: $"response-{pollId}");
        return 1;
    }

    public async Task<int> IngestInsightAsync(string topicId, string insightContent)
    {
        await _searchService.UpsertAsync(insightContent, source: "insight", documentId: $"insight-{topicId}");
        return 1;
    }

    public async Task<int> IngestExternalContentAsync(string source, string content)
    {
        await _searchService.UpsertAsync(content, source: source);
        return 1;
    }

    public async Task<int> IngestQuestionAsync(string questionId, string questionText, string? topicId = null)
    {
        await _searchService.UpsertAsync(
            $"Audience question: {questionText}",
            source: "question",
            documentId: $"question-{questionId}");
        return 1;
    }

    public async Task<int> IngestSessionSummaryAsync(string summaryContent)
    {
        await _searchService.UpsertAsync(summaryContent, source: "session-summary", documentId: "session-summary");
        _logger.LogInformation("Session summary ingested into knowledge base ({Length} chars)", summaryContent.Length);
        return 1;
    }

    public async Task<GitHubImportResult> IngestGitHubRepoAsync(
        string owner, string repo, string? subdirectory = null, string? branch = null)
    {
        var source = $"github:{owner}/{repo}/{branch ?? "main"}";
        using var httpClient = new HttpClient();
        var ghReader = new GitHubRepoReader(httpClient, owner, repo, subdirectory, branch,
            _loggerFactory.CreateLogger<GitHubRepoReader>());

        // Phase 1: Download GitHub files to temp directory (also collects raw docs for drafting)
        using var download = await ghReader.DownloadToDirectoryAsync();
        if (download.Files.Count == 0)
        {
            _logger.LogWarning("No markdown files downloaded from {Owner}/{Repo}", owner, repo);
            return new GitHubImportResult(0,
                download.Documents.Select(d => new ImportedDocument(d.FilePath, d.Content, d.FrontMatter)).ToList(),
                download.Errors);
        }

        // Phase 1b: Filter out unchanged files using content hashing
        var filesToProcess = new List<FileInfo>();
        foreach (var file in download.Files)
        {
            var docId = file.Name;
            var content = await File.ReadAllTextAsync(file.FullName);
            var hash = ComputeHash(content);

            if (_tracker is not null)
            {
                var existing = await _tracker.GetRecordAsync(docId, source);
                if (existing is not null && existing.ContentHash == hash && existing.Status == IngestionStatus.Completed)
                {
                    _logger.LogInformation("Skipping unchanged document: {DocId}", docId);
                    continue;
                }

                // Mark as pending before processing
                await _tracker.UpsertRecordAsync(new IngestionRecord
                {
                    DocumentId = docId,
                    Source = source,
                    ContentHash = hash,
                    Status = IngestionStatus.Pending
                });
            }

            filesToProcess.Add(file);
        }

        if (filesToProcess.Count == 0)
        {
            _logger.LogInformation("All documents unchanged for {Owner}/{Repo}, nothing to re-ingest", owner, repo);
            var allDocs = download.Documents
                .Select(d => new ImportedDocument(d.FilePath, d.Content, d.FrontMatter)).ToList();
            return new GitHubImportResult(0, allDocs, download.Errors);
        }

        _logger.LogInformation("Processing {NewCount}/{TotalCount} documents (skipped {SkipCount} unchanged)",
            filesToProcess.Count, download.Files.Count, download.Files.Count - filesToProcess.Count);

        // Register front matter for each document so FrontMatterChunkProcessor can find it
        var frontMatterProcessor = new FrontMatterChunkProcessor();
        foreach (var doc in download.Documents)
            frontMatterProcessor.AddFrontMatter(doc.FilePath, doc.FrontMatter);

        // Phase 2: Run the REAL IngestionPipeline on changed/new files
        IngestionDocumentReader reader = new MarkdownReader();

        var tokenizer = TiktokenTokenizer.CreateForModel("gpt-4o");
        var chunkerOptions = new IngestionChunkerOptions(tokenizer) { MaxTokensPerChunk = 500, OverlapTokens = 50 };
        IngestionChunker<string> chunker = new HeaderChunker(chunkerOptions);

        var enricherOptions = new EnricherOptions(_chatClient) { LoggerFactory = _loggerFactory };

        using var writer = new VectorStoreWriter<string>(
            _searchService.VectorStore,
            dimensionCount: 1536,
            new VectorStoreWriterOptions
            {
                CollectionName = "conference_knowledge",
                IncrementalIngestion = true
            });

        using IngestionPipeline<string> pipeline = new(reader, chunker, writer, new IngestionPipelineOptions(), _loggerFactory)
        {
            ChunkProcessors =
            {
                new SummaryEnricher(enricherOptions),
                new KeywordEnricher(enricherOptions, ReadOnlySpan<string>.Empty),
                frontMatterProcessor
            }
        };

        int pipelineSuccessCount = 0;
        var pipelineErrors = new List<string>(download.Errors);

        await foreach (var result in pipeline.ProcessAsync(filesToProcess))
        {
            if (result.Succeeded)
            {
                pipelineSuccessCount++;
                _logger.LogInformation("Pipeline processed: {DocId}", result.DocumentId);

                if (_tracker is not null)
                {
                    var matchedFile = filesToProcess.FirstOrDefault(f =>
                        f.FullName == result.DocumentId || f.Name == result.DocumentId);
                    var hash = "";
                    if (matchedFile is not null)
                    {
                        var content = await File.ReadAllTextAsync(matchedFile.FullName);
                        hash = ComputeHash(content);
                    }
                    await _tracker.UpsertRecordAsync(new IngestionRecord
                    {
                        DocumentId = result.DocumentId ?? "",
                        Source = source,
                        ContentHash = hash,
                        Status = IngestionStatus.Completed
                    });
                }
            }
            else
            {
                _logger.LogWarning("Pipeline failed for: {DocId}", result.DocumentId);
                pipelineErrors.Add($"{result.DocumentId}: Pipeline processing failed");

                if (_tracker is not null)
                {
                    await _tracker.UpsertRecordAsync(new IngestionRecord
                    {
                        DocumentId = result.DocumentId ?? "",
                        Source = source,
                        ContentHash = "",
                        Status = IngestionStatus.Failed,
                        ErrorMessage = "Pipeline processing failed"
                    });
                }
            }
        }

        var importedDocuments = download.Documents
            .Select(d => new ImportedDocument(d.FilePath, d.Content, d.FrontMatter))
            .ToList();

        _logger.LogInformation(
            "GitHub import complete: {DocCount} docs fetched, {PipelineCount} processed via pipeline from {Owner}/{Repo}",
            download.Documents.Count, pipelineSuccessCount, owner, repo);

        return new GitHubImportResult(pipelineSuccessCount, importedDocuments, pipelineErrors);
    }

    private static string ComputeHash(string content)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(content));
        return Convert.ToHexStringLower(bytes);
    }
}
