using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using TechConf.McpServer.Data;

namespace TechConf.McpServer.Tools;

// TODO: Task 2 — Implement the search_events tool
//
// This tool lets AI assistants search for TechConf events by keyword.
//
// Requirements:
// 1. Decorate the class with [McpServerToolType]
// 2. Create a static async method with [McpServerTool(Name = "search_events")] and
//    [Description("Search TechConf events by keyword in titles and descriptions")]
// 3. Parameters (use [Description] on each):
//    - AppDbContext db (injected via DI)
//    - string query — "Search keyword for event titles or descriptions"
//    - int maxResults = 10 — "Maximum number of results to return"
//    - CancellationToken ct
// 4. Implementation:
//    - Filter events where Title or Description contains the query
//    - Order by StartDate, take maxResults
//    - Select: Id, Title, StartDate, Location, Status
//    - Return JSON string (use JsonSerializer.Serialize)
//    - If no results, return a friendly message

public static class SearchEventsTool
{
    public static Task<string> ExecuteAsync()
    {
        throw new NotImplementedException("Implement search_events tool");
    }
}
