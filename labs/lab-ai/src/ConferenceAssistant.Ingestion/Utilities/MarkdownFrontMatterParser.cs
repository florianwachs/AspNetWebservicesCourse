namespace ConferenceAssistant.Ingestion.Utilities;

/// <summary>
/// Parses YAML front matter from markdown content without external YAML libraries.
/// Expects the format: ---\nyaml\n---\nmarkdown body
/// </summary>
public static class MarkdownFrontMatterParser
{
    private const string FrontMatterDelimiter = "---";

    public static ParsedDocument Parse(string markdownContent)
    {
        if (string.IsNullOrEmpty(markdownContent))
        {
            return new ParsedDocument(null, markdownContent ?? string.Empty);
        }

        // Normalize line endings
        var content = markdownContent.Replace("\r\n", "\n").Replace("\r", "\n");

        // Front matter must start at the very beginning with ---
        if (!content.StartsWith(FrontMatterDelimiter + "\n", StringComparison.Ordinal))
        {
            return new ParsedDocument(null, markdownContent);
        }

        // Find closing ---
        var closingIndex = content.IndexOf("\n" + FrontMatterDelimiter + "\n", FrontMatterDelimiter.Length, StringComparison.Ordinal);
        if (closingIndex < 0)
        {
            // Also check for closing --- at end of content (no trailing newline)
            var altClosing = content.IndexOf("\n" + FrontMatterDelimiter, FrontMatterDelimiter.Length, StringComparison.Ordinal);
            if (altClosing < 0 || altClosing + 1 + FrontMatterDelimiter.Length != content.Length)
            {
                return new ParsedDocument(null, markdownContent);
            }

            closingIndex = altClosing;
        }

        var yamlBlock = content.Substring(
            FrontMatterDelimiter.Length + 1,
            closingIndex - FrontMatterDelimiter.Length - 1);

        var bodyStart = closingIndex + 1 + FrontMatterDelimiter.Length;
        var body = bodyStart < content.Length
            ? content[bodyStart..].TrimStart('\n')
            : string.Empty;

        var frontMatter = ParseFrontMatter(yamlBlock);

        return new ParsedDocument(frontMatter, body);
    }

    private static FrontMatter ParseFrontMatter(string yaml)
    {
        var lines = yaml.Split('\n');
        string? title = null;
        string? job = null;
        string? category = null;
        string? status = null;
        string? repo = null;
        var technologies = new List<string>();

        string? currentListKey = null;

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd();

            // List item (e.g. "  - MAF")
            if (currentListKey is not null && IsListItem(line, out var item))
            {
                if (currentListKey == "technologies")
                {
                    technologies.Add(item);
                }

                continue;
            }

            // No longer in a list
            currentListKey = null;

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Key-value pair
            var colonIndex = line.IndexOf(':');
            if (colonIndex < 0)
            {
                continue;
            }

            var key = line[..colonIndex].Trim().ToLowerInvariant();
            var value = line[(colonIndex + 1)..].Trim();

            // If value is empty, this might be the start of a list
            if (string.IsNullOrEmpty(value))
            {
                currentListKey = key;
                continue;
            }

            value = StripQuotes(value);

            switch (key)
            {
                case "title":
                    title = value;
                    break;
                case "job":
                    job = value;
                    break;
                case "category":
                    category = value;
                    break;
                case "status":
                    status = value;
                    break;
                case "repo":
                    repo = value;
                    break;
                case "technologies":
                    // Inline list format: technologies: [MAF, MEAI]
                    if (value.StartsWith('[') && value.EndsWith(']'))
                    {
                        var inlineItems = value[1..^1].Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var inlineItem in inlineItems)
                        {
                            technologies.Add(StripQuotes(inlineItem.Trim()));
                        }
                    }

                    break;
            }
        }

        return new FrontMatter
        {
            Title = title,
            Job = job,
            Category = category,
            Technologies = technologies,
            Status = status,
            Repo = repo
        };
    }

    private static bool IsListItem(string line, out string item)
    {
        // Match lines like "  - value" or "- value"
        var trimmed = line.TrimStart();
        if (trimmed.StartsWith("- ", StringComparison.Ordinal))
        {
            item = StripQuotes(trimmed[2..].Trim());
            return true;
        }

        item = string.Empty;
        return false;
    }

    private static string StripQuotes(string value)
    {
        if (value.Length >= 2)
        {
            if ((value[0] == '"' && value[^1] == '"') ||
                (value[0] == '\'' && value[^1] == '\''))
            {
                return value[1..^1];
            }
        }

        return value;
    }
}

public record ParsedDocument(FrontMatter? FrontMatter, string Body);

public record FrontMatter
{
    public string? Title { get; init; }
    public string? Job { get; init; }
    public string? Category { get; init; }
    public List<string> Technologies { get; init; } = [];
    public string? Status { get; init; }
    public string? Repo { get; init; }
}
