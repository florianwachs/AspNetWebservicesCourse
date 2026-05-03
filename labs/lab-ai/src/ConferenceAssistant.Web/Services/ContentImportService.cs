using System.Text.RegularExpressions;
using ConferenceAssistant.Ingestion.Models;
using ConferenceAssistant.Ingestion.Services;

namespace ConferenceAssistant.Web.Services;

public partial class ContentImportService(
    IIngestionService ingestionService,
    ISessionDraftingService draftingService,
    ISlideGenerationService slideGenerationService,
    ILogger<ContentImportService> logger) : IContentImportService
{
    public async Task<ImportDraftResult> ImportAndDraftAsync(
        string repoUrl, string? sessionTitle = null, string? branch = null, string? subdirectory = null,
        GenerationOptions? options = null)
    {
        options ??= new GenerationOptions();
        var parsed = ParseRepoUrl(repoUrl);
        // Explicit params override URL-parsed values
        var effectiveBranch = branch ?? parsed.Branch;
        var effectiveSubdir = subdirectory ?? parsed.Subdirectory;

        logger.LogInformation("Starting GitHub import for {Owner}/{Repo} (branch={Branch}, subdir={Subdir})",
            parsed.Owner, parsed.Repo, effectiveBranch ?? "auto-detect", effectiveSubdir ?? "root");

        var importResult = await ingestionService.IngestGitHubRepoAsync(
            parsed.Owner, parsed.Repo, effectiveSubdir, effectiveBranch);
        logger.LogInformation("Imported {Count} documents from {Owner}/{Repo} ({Errors} errors)",
            importResult.RecordCount, parsed.Owner, parsed.Repo, importResult.Errors.Count);

        var draft = await draftingService.DraftSessionAsync(importResult.Documents, sessionTitle);
        logger.LogInformation("AI draft generated: {TopicCount} topics for {Owner}/{Repo}",
            draft.Topics.Count, parsed.Owner, parsed.Repo);

        // Strip polls if not requested
        if (!options.GeneratePolls)
        {
            draft = draft with
            {
                Topics = draft.Topics
                    .Select(t => t with { SuggestedPolls = [] })
                    .ToList()
            };
        }

        // Strip talking points if not requested
        if (!options.GenerateTalkingPoints)
        {
            draft = draft with
            {
                Topics = draft.Topics
                    .Select(t => t with { TalkingPoints = [] })
                    .ToList()
            };
        }

        // Generate slide deck from draft
        string slideMarkdown;
        if (!options.GenerateSlides)
        {
            slideMarkdown = "";
        }
        else
        {
            try
            {
                slideMarkdown = await slideGenerationService.GenerateEnhancedSlideMarkdownAsync(
                    draft, importResult.Documents);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "AI slide enhancement failed, using programmatic slides");
                slideMarkdown = slideGenerationService.GenerateSlideMarkdown(draft);
            }
        }

        return new ImportDraftResult(importResult, draft, slideMarkdown);
    }

    private static ParsedRepoUrl ParseRepoUrl(string repoUrl)
    {
        if (string.IsNullOrWhiteSpace(repoUrl))
            throw new ArgumentException("Repository URL cannot be empty.", nameof(repoUrl));

        var url = repoUrl.Trim().TrimEnd('/');

        // Remove .git suffix
        if (url.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
            url = url[..^4];

        // Full URL with optional /tree/branch/path:
        // https://github.com/owner/repo/tree/branch/optional/path
        var fullMatch = FullRepoUrlPattern().Match(url);
        if (fullMatch.Success)
        {
            var branch = fullMatch.Groups["branch"].Success ? fullMatch.Groups["branch"].Value : null;
            var subdir = fullMatch.Groups["path"].Success && fullMatch.Groups["path"].Value.Length > 0
                ? fullMatch.Groups["path"].Value : null;
            return new ParsedRepoUrl(fullMatch.Groups["owner"].Value, fullMatch.Groups["repo"].Value, branch, subdir);
        }

        // Simple: owner/repo or github.com/owner/repo
        var simpleMatch = SimpleRepoPattern().Match(url);
        if (simpleMatch.Success)
            return new ParsedRepoUrl(simpleMatch.Groups["owner"].Value, simpleMatch.Groups["repo"].Value, null, null);

        throw new ArgumentException(
            $"Invalid GitHub repository URL: '{repoUrl}'. Expected formats: 'https://github.com/owner/repo', 'https://github.com/owner/repo/tree/branch', or 'owner/repo'.",
            nameof(repoUrl));
    }

    private record ParsedRepoUrl(string Owner, string Repo, string? Branch, string? Subdirectory);

    // Matches: https://github.com/owner/repo/tree/branch/optional/path
    [GeneratedRegex(@"^(?:https?://)?github\.com/(?<owner>[A-Za-z0-9_.\-]+)/(?<repo>[A-Za-z0-9_.\-]+)/tree/(?<branch>[^/]+)(?:/(?<path>.+))?$")]
    private static partial Regex FullRepoUrlPattern();

    // Matches: github.com/owner/repo or just owner/repo
    [GeneratedRegex(@"^(?:https?://)?(?:github\.com/)?(?<owner>[A-Za-z0-9_.\-]+)/(?<repo>[A-Za-z0-9_.\-]+)$")]
    private static partial Regex SimpleRepoPattern();
}
