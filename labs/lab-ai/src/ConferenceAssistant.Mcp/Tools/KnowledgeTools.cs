using System.ComponentModel;
using System.Text;
using ConferenceAssistant.Ingestion.Services;
using ModelContextProtocol.Server;

namespace ConferenceAssistant.Mcp.Tools;

[McpServerToolType]
public class KnowledgeTools
{
    [McpServerTool(Name = "search_knowledge", ReadOnly = true),
     Description("Searches the conference knowledge base (vector store) for content matching the query.")]
    public static async Task<string> SearchKnowledge(
        ISemanticSearchService searchService,
        [Description("The search query to find relevant content in the knowledge base.")] string query)
    {
        var results = await searchService.SearchAsync(query, topK: 5);
        if (results.Count == 0)
            return $"No knowledge base results found for: '{query}'";

        var sb = new StringBuilder();
        sb.AppendLine($"# Knowledge Base Search: \"{query}\"");
        sb.AppendLine($"Found {results.Count} result(s).");
        sb.AppendLine();

        for (var i = 0; i < results.Count; i++)
        {
            var record = results[i];
            sb.AppendLine($"## {i + 1}. [{record.Source}]");
            sb.AppendLine(record.Content);
            if (!string.IsNullOrEmpty(record.Context))
                sb.AppendLine($"*Context: {record.Context}*");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_knowledge_stats", ReadOnly = true),
     Description("Returns statistics about the knowledge base including total record count.")]
    public static async Task<string> GetKnowledgeStats(ISemanticSearchService searchService)
    {
        var recordCount = await searchService.GetRecordCountAsync();

        var sb = new StringBuilder();
        sb.AppendLine("# Knowledge Base Statistics");
        sb.AppendLine($"**Total Records:** {recordCount}");

        return sb.ToString();
    }
}
