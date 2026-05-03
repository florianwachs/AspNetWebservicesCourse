using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using TechConf.McpServer.Data;

namespace TechConf.McpServer.Tools;

[McpServerToolType]
public static class SearchEventsTool
{
    [McpServerTool(Name = "search_events"), Description("Search TechConf events by keyword in titles and descriptions")]
    public static async Task<string> ExecuteAsync(
        AppDbContext db,
        [Description("Search keyword for event titles or descriptions")] string query,
        [Description("Maximum number of results to return")] int maxResults = 10,
        CancellationToken ct = default)
    {
        var results = await db.Events
            .Where(e => e.Title.Contains(query) || (e.Description != null && e.Description.Contains(query)))
            .OrderBy(e => e.StartDate)
            .Take(maxResults)
            .Select(e => new { e.Id, e.Title, e.StartDate, e.Location, e.Status })
            .ToListAsync(ct);

        return results.Count == 0
            ? $"No events found matching '{query}'"
            : JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
    }
}
