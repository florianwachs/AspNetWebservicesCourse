# Troubleshooting

Common issues and solutions when running Conference Pulse. If you're setting up for the first time, start with the [Getting Started](getting-started.md) guide.

## Build & Startup Issues

### .NET 10 SDK not found

**Symptom:** `dotnet build` fails with target framework error

**Fix:** Install the .NET 10 SDK from <https://dotnet.microsoft.com/download/dotnet/10.0>

**Verify:** `dotnet --list-sdks` should show a 10.x version

### Aspire CLI not installed

**Symptom:** `aspire run` command not recognized, or `aspire: command not found`

**Fix:** Install the Aspire CLI:
- **Windows (PowerShell):** `irm https://aspire.dev/install.ps1 | iex`
- **Linux / macOS:** `curl -fsSL https://aspire.dev/install.sh | bash`

**Verify:** `aspire --version` should print a version number. Run `aspire doctor` to check your full environment.

### Docker not running

**Symptom:** PostgreSQL and Qdrant containers fail to start in Aspire dashboard

**Fix:** Start Docker Desktop. Ensure it's fully running before `aspire run`.

**Verify:** `docker info` should return without errors

### Build fails with package restore errors

**Symptom:** NuGet restore fails for preview packages

**Fix:** Ensure `nuget.config` in repo root points to `https://api.nuget.org/v3/index.json`. Run `dotnet restore`.

## Azure OpenAI Issues

### DefaultAzureCredential authentication fails

**Symptom:** 401/403 errors when app tries to call Azure OpenAI

**Fix:** Run `az login` and ensure your account has access to the Azure OpenAI resource. Check that the subscription ID in user secrets matches.

**Verify:** `az account show` should show the correct subscription

### Deployment not found

**Symptom:** 404 errors or "deployment not found" in logs

**Fix:** Ensure your Azure OpenAI resource has deployments named exactly `chat` and `embedding`. If your deployment names differ, update `src/ConferenceAssistant.AppHost/AppHost.cs` lines 18-19.

### User secrets not configured

**Symptom:** App starts but AI features don't work; OpenAI resource shows "Unhealthy" in Aspire dashboard

**Fix:** Set all four required user secrets in the AppHost project:

```bash
cd src/ConferenceAssistant.AppHost
dotnet user-secrets set "Azure:SubscriptionId" "<value>"
dotnet user-secrets set "Azure:Location" "eastus"
dotnet user-secrets set "AzureOpenAI:Name" "<value>"
dotnet user-secrets set "AzureOpenAI:ResourceGroup" "<value>"
```

### AI responses are slow (>15 seconds)

**Symptom:** Poll generation, Q&A answers, or insights take a long time

**Possible causes:**

- Azure OpenAI rate limiting (check deployment TPM quota)
- Cold start on first request
- Complex prompts with large context

**Fix:** Increase TPM quota in Azure Portal, or wait for initial warm-up.

## Container Issues

### PostgreSQL container won't start

**Symptom:** `postgres` resource shows "Failed" in Aspire dashboard

**Fix:** Check Docker has enough resources. Try removing old volumes: `docker volume prune`. Restart Docker Desktop.

### Qdrant container won't start

**Symptom:** `qdrant` resource shows "Failed" in Aspire dashboard

**Fix:** Same as PostgreSQL. Qdrant uses a persistent lifetime — it may be left from a previous run. Check `docker ps -a` for stopped Qdrant containers.

### Port conflicts

**Symptom:** Resource fails with "port already in use"

**Fix:** Check what's using the port: `netstat -ano | findstr :PORT`. Stop the conflicting process or restart Docker.

## Runtime Issues

### SignalR connection drops

**Symptom:** Display or attendee view shows "Reconnecting..." or stops updating

**Fix:** Refresh the browser page. If persistent, check the Aspire dashboard for the web resource health.

### Presenter dashboard shows "Session not found"

**Symptom:** Navigating to `/presenter/DOTNETAI-CONF` shows an error

**Possible causes:**

- App hasn't finished startup (seed data not loaded yet)
- Session code is case-sensitive — use uppercase

**Fix:** Wait for startup to complete (check Aspire dashboard logs). Use the home page (`/`) to navigate.

### Polls don't appear on attendee view

**Symptom:** Poll launched from presenter but not visible on `/session/{code}`

**Fix:** Ensure the session is "Live" (not just "Created"). Check SignalR connection. Refresh the attendee page.

### Knowledge base shows 0 records

**Symptom:** Knowledge base stats show no records after startup

**Possible causes:**

- Qdrant container not ready when ingestion ran
- Embedding generation failed (Azure OpenAI issue)

**Fix:** Check Aspire dashboard logs for ingestion errors. Restart the app (`aspire run` again).

### Insights not generating after poll closes

**Symptom:** Poll closes but no insight appears

**Possible causes:**

- IChatClient not configured (AI disabled mode)
- Azure OpenAI rate limiting

**Fix:** Check Aspire dashboard for the web resource logs. Look for error messages from InsightGenerationService.

## MCP Issues

### MCP tools not discovered by Copilot

**Symptom:** VS Code Copilot Chat doesn't show Conference Pulse tools

**Fix:**

1. Check `.vscode/mcp.json` has the correct port (from Aspire dashboard)
2. Verify the app is running: `curl http://localhost:{PORT}/mcp` should return MCP response
3. Reload VS Code window
4. Check VS Code Output panel for MCP errors

### MCP tool calls return errors

**Symptom:** Tool calls fail with 500 errors

**Fix:** Check if the session is loaded. Some tools require an active session (e.g., `get_session_status`). Check Aspire dashboard logs for detailed errors.

## Slide Issues

### Slides not appearing on display

**Symptom:** `/display/{code}` shows no slides

**Fix:** Ensure `data/slides.md` exists and is valid. Check for unclosed fenced code blocks or missing `---` separators. Restart the app.

### Speaker notes visible on display

**Symptom:** `<!-- speaker: -->` content showing on the big screen

**Fix:** This should not happen — notes are stripped by the parser. Check that `<!-- speaker:` has a matching `-->`. If multi-line, ensure the closing `-->` is on its own line.

### Slides split unexpectedly

**Symptom:** One slide becomes two

**Fix:** Check for stray `---` in slide content (outside fenced code blocks). The `---` separator must be on its own line.

## Running Without AI

Conference Pulse can run without Azure OpenAI configured. In this mode:

- ✅ Session creation and management
- ✅ Manual poll creation and voting
- ✅ Question submission and upvoting
- ✅ Slide navigation
- ❌ AI poll generation
- ❌ AI question answering
- ❌ Insight generation
- ❌ Semantic search
- ❌ Session summary generation

This is useful for testing the UI and session flow without Azure costs.

## Getting Help

- Check the Aspire dashboard logs for detailed error messages
- Look at the web resource's console output for stack traces
- Use PgWeb (accessible from Aspire dashboard) to inspect PostgreSQL data
- File issues on the GitHub repository
