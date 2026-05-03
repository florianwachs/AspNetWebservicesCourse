using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechConf.McpServer.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=techconf.db"));

// TODO: Task 1 — Register the MCP server
// Configure the MCP server with:
//   - .AddMcpServer()
//   - .WithStdioServerTransport()     (use stdin/stdout communication)
//   - .WithToolsFromAssembly()        (auto-discover [McpServerTool] classes)
//   - .WithPromptsFromAssembly()      (auto-discover [McpServerPrompt] classes)

var app = builder.Build();

// Ensure database is created with seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

await app.RunAsync();
