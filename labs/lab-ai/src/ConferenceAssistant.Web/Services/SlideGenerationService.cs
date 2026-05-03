using System.Text;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion.Models;
using ConferenceAssistant.Ingestion.Services;
using Microsoft.Extensions.AI;

namespace ConferenceAssistant.Web.Services;

public class SlideGenerationService(
    IChatClient chatClient,
    ISemanticSearchService searchService,
    ILogger<SlideGenerationService> logger) : ISlideGenerationService
{
    private static readonly string[] ContentSlideSubtitles =
        ["Key Points", "In Practice", "Deep Dive", "Going Further"];

    private const string EnhancementSystemPrompt = """
        You are a conference slide deck enhancer. Given a basic slide deck in markdown format,
        source documents, and knowledge base context, enhance the slides.

        GROUNDING RULES:
        - Use ONLY information from the provided source documents and knowledge base context
        - Do NOT invent facts, statistics, or technical claims not found in the sources
        - When adding code examples, use only APIs and patterns mentioned in the sources
        - If insufficient context exists for a topic, keep the existing slide content

        ENHANCEMENT GOALS:
        1. More detailed and natural speaker notes grounded in the source material
        2. Code example slides when source material references specific APIs or code
        3. Better section transitions
        4. Keep ALL existing slides, only enhance and add between them

        IMPORTANT: Maintain the exact markdown format:
        - Slides separated by --- on its own line
        - <!-- topic: id --> for topic assignment
        - <!-- layout: centered --> for layout (centered, default)
        - <!-- speaker: notes --> for speaker notes
        - # H1 for main title, ## H2 for subtitle
        - - bullet for content bullets
        - ```language for code blocks

        Return ONLY the enhanced markdown, no explanations.
        """;

    public string GenerateSlideMarkdown(SessionDraft draft)
    {
        var sb = new StringBuilder();
        var slides = new List<string>();

        // Title slide
        slides.Add(BuildTitleSlide(draft));

        // Topic slides
        for (var i = 0; i < draft.Topics.Count; i++)
        {
            var topic = draft.Topics[i];
            var topicId = $"topic-{i}";

            slides.Add(BuildSectionSlide(topic, topicId));

            slides.AddRange(BuildContentSlides(topic, topicId));

            if (topic.SuggestedPolls.Count > 0)
            {
                slides.Add(BuildPollSlide(topic.SuggestedPolls[0], topicId));
            }
        }

        // Closing slide
        slides.Add(BuildClosingSlide(draft));

        sb.Append(string.Join("\n\n---\n\n", slides));
        return sb.ToString();
    }

    public async Task<string> GenerateEnhancedSlideMarkdownAsync(
        SessionDraft draft, IReadOnlyList<ImportedDocument>? documents = null)
    {
        var baseline = GenerateSlideMarkdown(draft);

        try
        {
            var userPrompt = await BuildEnhancementUserPromptAsync(baseline, draft, documents);

            var response = await chatClient.GetResponseAsync(
                [
                    new ChatMessage(ChatRole.System, EnhancementSystemPrompt),
                    new ChatMessage(ChatRole.User, userPrompt)
                ]);

            var enhanced = response.Text?.Trim();

            if (string.IsNullOrWhiteSpace(enhanced))
            {
                logger.LogWarning("AI returned empty response for slide enhancement, using baseline");
                return baseline;
            }

            // Validate the enhanced markdown produces parseable slides
            var parsed = SlideMarkdownParser.Parse(enhanced);
            if (parsed.Count == 0)
            {
                logger.LogWarning("AI-enhanced markdown produced no valid slides, using baseline");
                return baseline;
            }

            logger.LogInformation(
                "AI-enhanced slide deck: {BaselineLength} -> {EnhancedLength} chars, {SlideCount} slides",
                baseline.Length, enhanced.Length, parsed.Count);

            return enhanced;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "AI slide enhancement failed, using programmatic baseline");
            return baseline;
        }
    }

    private static string BuildTitleSlide(SessionDraft draft)
    {
        var firstSentence = GetFirstSentence(draft.Description);

        return $"""
            <!-- layout: centered -->

            # {draft.Title}

            ## {firstSentence}

            <!-- speaker: Welcome! {draft.Description} -->
            """.TrimLeadingIndentation();
    }

    private static string BuildSectionSlide(TopicDraft topic, string topicId)
    {
        return $"""
            <!-- topic: {topicId} -->
            <!-- layout: centered -->

            # {topic.Title}

            <!-- speaker: Let's explore {topic.Title}. {topic.Description}. -->
            """.TrimLeadingIndentation();
    }

    private static List<string> BuildContentSlides(TopicDraft topic, string topicId)
    {
        var slides = new List<string>();

        if (topic.TalkingPoints.Count == 0)
        {
            return slides;
        }

        var chunks = ChunkTalkingPoints(topic.TalkingPoints, maxPerSlide: 4);

        for (var j = 0; j < chunks.Count; j++)
        {
            var subtitle = ContentSlideSubtitles[j % ContentSlideSubtitles.Length];
            var bullets = string.Join("\n", chunks[j].Select(tp => $"- {tp}"));

            var slide = $"""
                <!-- topic: {topicId} -->

                ## {topic.Title} — {subtitle}

                {bullets}

                <!-- speaker: Walk through each point with the audience. -->
                """.TrimLeadingIndentation();

            slides.Add(slide);
        }

        return slides;
    }

    private static string BuildPollSlide(PollDraft poll, string topicId)
    {
        var optionsList = string.Join(", ", poll.Options);

        return $"""
            <!-- topic: {topicId} -->
            <!-- layout: centered -->

            # 📊 Poll Time!

            ## {poll.Question}

            <!-- speaker: Launch the poll for this topic. Give the audience 30-60 seconds to respond. Options: {optionsList}. -->
            """.TrimLeadingIndentation();
    }

    private static string BuildClosingSlide(SessionDraft draft)
    {
        return $"""
            <!-- layout: centered -->

            # Thank You! 🎉

            ## {draft.Title}

            <!-- speaker: Wrap up the session. Thank the audience for participating. Highlight key takeaways. -->
            """.TrimLeadingIndentation();
    }

    private async Task<string> BuildEnhancementUserPromptAsync(
        string baseline, SessionDraft draft, IReadOnlyList<ImportedDocument>? documents)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Here is the baseline slide deck to enhance:");
        sb.AppendLine();
        sb.AppendLine(baseline);

        // Search knowledge base for each topic to get grounded context
        var kbContext = new StringBuilder();
        foreach (var topic in draft.Topics)
        {
            var results = await searchService.SearchAsync(topic.Title, topK: 3);
            if (results.Count > 0)
            {
                kbContext.AppendLine($"\n### Knowledge Base — {topic.Title}:");
                foreach (var r in results)
                    kbContext.AppendLine(r.Content);
            }
        }

        if (kbContext.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Knowledge base context (use this to ground your enhancements):");
            sb.Append(kbContext);
        }

        if (documents is { Count: > 0 })
        {
            sb.AppendLine();
            sb.AppendLine("Source documents for reference:");
            sb.AppendLine();

            foreach (var doc in documents)
            {
                sb.AppendLine($"--- {doc.FilePath} ---");
                var truncated = doc.Content.Length > 300
                    ? doc.Content[..300] + "..."
                    : doc.Content;
                sb.AppendLine(truncated);
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private static string GetFirstSentence(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "";
        }

        var periodIndex = text.IndexOf('.');
        return periodIndex >= 0 ? text[..(periodIndex + 1)] : text;
    }

    private static List<List<string>> ChunkTalkingPoints(List<string> points, int maxPerSlide)
    {
        var chunks = new List<List<string>>();

        for (var i = 0; i < points.Count; i += maxPerSlide)
        {
            var count = Math.Min(maxPerSlide, points.Count - i);
            // Prefer groups of 3+ when possible
            if (count <= 1 && chunks.Count > 0)
            {
                chunks[^1].AddRange(points.GetRange(i, count));
            }
            else
            {
                chunks.Add(points.GetRange(i, count));
            }
        }

        return chunks;
    }
}

internal static class StringExtensions
{
    /// <summary>
    /// Removes consistent leading whitespace indentation from raw string literals
    /// used in interpolated strings, preserving intentional content indentation.
    /// </summary>
    internal static string TrimLeadingIndentation(this string text)
    {
        var lines = text.Split('\n');

        var minIndent = lines
            .Where(l => l.TrimEnd().Length > 0)
            .Select(l => l.Length - l.TrimStart().Length)
            .DefaultIfEmpty(0)
            .Min();

        if (minIndent == 0)
        {
            return text;
        }

        return string.Join('\n', lines.Select(l =>
            l.TrimEnd().Length > 0 && l.Length >= minIndent ? l[minIndent..] : l.TrimEnd()));
    }
}
