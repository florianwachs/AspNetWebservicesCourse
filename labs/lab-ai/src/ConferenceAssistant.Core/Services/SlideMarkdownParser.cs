using System.Text.RegularExpressions;
using ConferenceAssistant.Core.Models;

namespace ConferenceAssistant.Core.Services;

public static class SlideMarkdownParser
{
    private static readonly Regex SpeakerNotesRegex = new(
        @"<!--\s*speaker\s*:(.*?)-->",
        RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex TopicRegex = new(
        @"<!--\s*topic\s*:\s*(.*?)\s*-->",
        RegexOptions.Compiled);

    private static readonly Regex LayoutRegex = new(
        @"<!--\s*layout\s*:\s*(.*?)\s*-->",
        RegexOptions.Compiled);

    private static readonly Regex HtmlCommentRegex = new(
        @"<!--.*?-->",
        RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex FencedCodeBlockRegex = new(
        @"^```(\w*)\s*\n(.*?)^```",
        RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex H1Regex = new(
        @"^#\s+(.+)$", RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex H2Regex = new(
        @"^##\s+(.+)$", RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex HeadingRegex = new(
        @"^#{1,6}\s+(.+)$", RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex BulletRegex = new(
        @"^[\-\*]\s+(.+)$", RegexOptions.Multiline | RegexOptions.Compiled);

    public static List<Slide> Parse(string markdown)
    {
        var blocks = SplitIntoBlocks(markdown);
        var slides = new List<Slide>();
        string? currentTopicId = null;

        foreach (var block in blocks)
        {
            var content = block;

            // Extract topic (sticky across slides)
            var topicMatch = TopicRegex.Match(content);
            if (topicMatch.Success)
            {
                var topicValue = topicMatch.Groups[1].Value.Trim();
                currentTopicId = string.IsNullOrEmpty(topicValue) ? null : topicValue;
            }
            content = TopicRegex.Replace(content, "");

            // Extract layout
            SlideLayout? explicitLayout = null;
            var layoutMatch = LayoutRegex.Match(content);
            if (layoutMatch.Success)
            {
                explicitLayout = ParseLayout(layoutMatch.Groups[1].Value.Trim());
            }
            content = LayoutRegex.Replace(content, "");

            // Extract speaker notes
            var speakerNotes = new List<string>();
            foreach (Match m in SpeakerNotesRegex.Matches(content))
            {
                speakerNotes.Add(m.Groups[1].Value.Trim());
            }
            content = SpeakerNotesRegex.Replace(content, "");

            // Check if all visible content is just HTML comments (blank slide detection)
            var contentWithoutComments = HtmlCommentRegex.Replace(content, "").Trim();

            // Also check original block (before removing topic/layout/speaker) for blank detection
            // At this point, content has had topic, layout, and speaker notes removed.
            // If remaining content (after also removing any other HTML comments) is empty,
            // but we had speaker notes, it's a Blank slide.
            var visibleContent = contentWithoutComments;

            if (string.IsNullOrWhiteSpace(visibleContent))
            {
                // If there were speaker notes, emit a Blank slide; otherwise skip
                if (speakerNotes.Count == 0)
                    continue;

                slides.Add(new Slide
                {
                    Id = $"slide-{slides.Count}",
                    Order = slides.Count,
                    TopicId = currentTopicId,
                    Type = SlideType.Blank,
                    Layout = explicitLayout ?? SlideLayout.Default,
                    SpeakerNotes = string.Join("\n", speakerNotes)
                });
                continue;
            }

            var slide = new Slide
            {
                Id = $"slide-{slides.Count}",
                Order = slides.Count,
                TopicId = currentTopicId,
                SpeakerNotes = string.Join("\n", speakerNotes)
            };

            // Remove remaining non-semantic HTML comments from visible content
            visibleContent = HtmlCommentRegex.Replace(content, "").Trim();

            ClassifySlide(slide, visibleContent, explicitLayout);
            slides.Add(slide);
        }

        return slides;
    }

    public static async Task<List<Slide>> ParseFileAsync(string filePath)
    {
        var markdown = await File.ReadAllTextAsync(filePath);
        return Parse(markdown);
    }

    private static List<string> SplitIntoBlocks(string markdown)
    {
        // Normalize line endings
        markdown = markdown.Replace("\r\n", "\n").Replace("\r", "\n");

        // Split on lines that are exactly "---" (with optional surrounding whitespace)
        var parts = Regex.Split(markdown, @"^---\s*$", RegexOptions.Multiline);

        var blocks = new List<string>();

        // Detect and skip YAML frontmatter: if the document starts with "---",
        // the first part (before the first ---) will be empty/whitespace,
        // and the second part is the frontmatter body. Skip both.
        int startIndex = 0;
        if (parts.Length >= 3)
        {
            var beforeFirst = parts[0].Trim();
            if (string.IsNullOrEmpty(beforeFirst))
            {
                // parts[0] = empty (before first ---), parts[1] = frontmatter, parts[2+] = slides
                startIndex = 2;
            }
        }

        for (int i = startIndex; i < parts.Length; i++)
        {
            var trimmed = parts[i].Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                blocks.Add(parts[i]);
            }
        }

        return blocks;
    }

    private static void ClassifySlide(Slide slide, string visibleContent, SlideLayout? explicitLayout)
    {
        var h1Match = H1Regex.Match(visibleContent);
        var h2Match = H2Regex.Match(visibleContent);
        var codeMatch = FencedCodeBlockRegex.Match(visibleContent);
        var bulletMatches = BulletRegex.Matches(visibleContent);
        var headingMatch = HeadingRegex.Match(visibleContent);

        bool hasH1 = h1Match.Success;
        bool hasCode = codeMatch.Success;
        bool hasBullets = bulletMatches.Count > 0;

        if (hasH1)
        {
            slide.Title = h1Match.Groups[1].Value.Trim();

            // Content after removing the H1 line and any H2 line
            var remaining = H1Regex.Replace(visibleContent, "", 1).Trim();

            if (h2Match.Success)
            {
                slide.Subtitle = h2Match.Groups[1].Value.Trim();
                remaining = H2Regex.Replace(remaining, "", 1).Trim();
            }

            // Check if it's a title slide (H1, possibly H2, and no other significant content)
            var remainingForCheck = remaining;
            // Remove code blocks and bullets to check if there's other content
            if (string.IsNullOrWhiteSpace(remainingForCheck))
            {
                // Only heading(s), no other content
                if (h2Match.Success || !hasCode && !hasBullets)
                {
                    // If it has both H1 and H2 with nothing else -> Title
                    // If it has only H1 with nothing else -> could be Section or Title
                    if (h2Match.Success)
                    {
                        slide.Type = SlideType.Title;
                        slide.Layout = explicitLayout ?? SlideLayout.Centered;
                        return;
                    }
                    else
                    {
                        // Only H1, no other content -> Section
                        slide.Type = SlideType.Section;
                        slide.Layout = explicitLayout ?? SlideLayout.Default;
                        return;
                    }
                }
            }

            // H1 present with additional content - classify by content type
            if (hasCode)
            {
                slide.Type = SlideType.Code;
                slide.CodeLanguage = string.IsNullOrEmpty(codeMatch.Groups[1].Value)
                    ? null
                    : codeMatch.Groups[1].Value;
                slide.CodeSnippet = codeMatch.Groups[2].Value.TrimEnd();
                slide.Layout = explicitLayout ?? SlideLayout.Default;
                return;
            }

            if (hasBullets)
            {
                slide.Type = SlideType.Content;
                slide.Bullets = bulletMatches
                    .Select(m => m.Groups[1].Value.Trim())
                    .ToList();
                slide.Layout = explicitLayout ?? SlideLayout.Default;
                return;
            }

            // Has heading + other body content
            slide.Type = SlideType.Content;
            slide.BodyMarkdown = remaining;
            slide.Layout = explicitLayout ?? SlideLayout.Default;
            return;
        }

        // No H1 heading

        // Check for any heading (##, ###, etc.)
        if (headingMatch.Success)
        {
            slide.Title = headingMatch.Groups[1].Value.Trim();
            var remaining = HeadingRegex.Replace(visibleContent, "", 1).Trim();

            if (hasCode)
            {
                slide.Type = SlideType.Code;
                slide.CodeLanguage = string.IsNullOrEmpty(codeMatch.Groups[1].Value)
                    ? null
                    : codeMatch.Groups[1].Value;
                slide.CodeSnippet = codeMatch.Groups[2].Value.TrimEnd();
                slide.Layout = explicitLayout ?? SlideLayout.Default;
                return;
            }

            if (hasBullets)
            {
                slide.Type = SlideType.Content;
                slide.Bullets = bulletMatches
                    .Select(m => m.Groups[1].Value.Trim())
                    .ToList();
                slide.Layout = explicitLayout ?? SlideLayout.Default;
                return;
            }

            if (string.IsNullOrWhiteSpace(remaining))
            {
                slide.Type = SlideType.Section;
                slide.Layout = explicitLayout ?? SlideLayout.Default;
                return;
            }

            slide.Type = SlideType.Content;
            slide.BodyMarkdown = remaining;
            slide.Layout = explicitLayout ?? SlideLayout.Default;
            return;
        }

        // No heading at all
        if (hasCode)
        {
            slide.Type = SlideType.Code;
            slide.CodeLanguage = string.IsNullOrEmpty(codeMatch.Groups[1].Value)
                ? null
                : codeMatch.Groups[1].Value;
            slide.CodeSnippet = codeMatch.Groups[2].Value.TrimEnd();
            slide.Layout = explicitLayout ?? SlideLayout.Default;
            return;
        }

        if (hasBullets)
        {
            slide.Type = SlideType.Content;
            slide.Bullets = bulletMatches
                .Select(m => m.Groups[1].Value.Trim())
                .ToList();
            slide.Layout = explicitLayout ?? SlideLayout.Default;
            return;
        }

        // Fallback: generic content
        slide.Type = SlideType.Content;
        slide.BodyMarkdown = visibleContent.Trim();
        slide.Layout = explicitLayout ?? SlideLayout.Default;
    }

    private static SlideLayout ParseLayout(string value) => value.ToLowerInvariant() switch
    {
        "centered" => SlideLayout.Centered,
        "two-column" => SlideLayout.TwoColumn,
        _ => SlideLayout.Default
    };
}
