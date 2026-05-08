using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Npgsql;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using System.ClientModel;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion.Services;
using ConferenceAssistant.Agents.Tools;
using ConferenceAssistant.Agents.Workflows;
using ConferenceAssistant.Mcp.Clients;
using ConferenceAssistant.Web.Components;
using ConferenceAssistant.Web.Data;
using ConferenceAssistant.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Aspire Service Defaults (OpenTelemetry, health checks, service discovery)
// ---------------------------------------------------------------------------
builder.AddServiceDefaults();

// ---------------------------------------------------------------------------
// PostgreSQL — NpgsqlDataSource with dynamic JSON for EF Core JSONB columns
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<NpgsqlDataSource>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("conferencedb")
        ?? throw new InvalidOperationException("Missing 'conferencedb' connection string");
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    dataSourceBuilder.EnableDynamicJson();
    return dataSourceBuilder.Build();
});

// ---------------------------------------------------------------------------
// PostgreSQL + EF Core — persistent storage using the shared NpgsqlDataSource
// Uses DbContextFactory (singleton-safe) for fire-and-forget persistence tasks
// ---------------------------------------------------------------------------
builder.Services.AddDbContextFactory<ConferenceDbContext>((sp, options) =>
{
    var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
    options.UseNpgsql(dataSource);
});
builder.Services.AddSingleton<ISessionPersistenceService, SessionPersistenceService>();

// ---------------------------------------------------------------------------
// Blazor Interactive Server
// ---------------------------------------------------------------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ---------------------------------------------------------------------------
// Core Services — in-memory, singleton (shared state across all Blazor circuits)
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<ISessionManager, SessionManager>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IPollService, PollService>();
builder.Services.AddSingleton<IQuestionService, QuestionService>();
builder.Services.AddSingleton<IInsightService, InsightService>();

// ---------------------------------------------------------------------------
// AI Provider — GitHub Models or local Ollama via OpenAI-compatible clients
// ---------------------------------------------------------------------------
var configuredAiOptions = builder.Configuration
    .GetSection(AiProviderOptions.SectionName)
    .Get<AiProviderOptions>() ?? new AiProviderOptions();

var aiOptions = configuredAiOptions.Resolve(builder.Configuration["GITHUB_TOKEN"]);
builder.Services.AddSingleton(aiOptions);

var openAiClientOptions = new OpenAIClientOptions
{
    Endpoint = aiOptions.Endpoint
};

builder.Services.AddChatClient(_ =>
        new ChatClient(
            aiOptions.ChatModel,
            new ApiKeyCredential(aiOptions.ApiKey),
            openAiClientOptions).AsIChatClient())
    .UseFunctionInvocation()
    .UseOpenTelemetry()
    .UseLogging();

builder.Services.AddEmbeddingGenerator(_ =>
    new EmbeddingClient(
        aiOptions.EmbeddingModel,
        new ApiKeyCredential(aiOptions.ApiKey),
        openAiClientOptions).AsIEmbeddingGenerator(aiOptions.EmbeddingDimensions));

// ---------------------------------------------------------------------------
// Ingestion + VectorData — knowledge base pipeline
// Qdrant client registered via Aspire for persistent vector storage
// ---------------------------------------------------------------------------
builder.AddQdrantClient("qdrant");
builder.Services.AddSingleton<ISemanticSearchService, SemanticSearchService>();
builder.Services.AddSingleton<IIngestionTracker, IngestionTracker>();
builder.Services.AddSingleton<IIngestionService, IngestionService>();
builder.Services.AddSingleton<ISessionDraftingService, SessionDraftingService>();
builder.Services.AddSingleton<IContentImportService, ContentImportService>();
builder.Services.AddSingleton<ISlideGenerationService, SlideGenerationService>();

// ---------------------------------------------------------------------------
// Agent Tools + Workflows
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<AgentTools>();
builder.Services.AddSingleton<PollGenerationWorkflow>();
builder.Services.AddSingleton<ResponseAnalysisWorkflow>();
builder.Services.AddSingleton<SessionSummaryWorkflow>();

// ---------------------------------------------------------------------------
// MCP Server — expose tools via Streamable HTTP
// ---------------------------------------------------------------------------
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "ConferencePulse",
            Version = "1.0.0"
        };
    })
    .WithToolsFromAssembly(typeof(ConferenceAssistant.Mcp.Tools.ConferenceTools).Assembly)
    .WithHttpTransport();

// ---------------------------------------------------------------------------
// AI-powered Q&A — auto-answer audience questions
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<IQuestionAnsweringService, QuestionAnsweringService>();

// ---------------------------------------------------------------------------
// AI-powered Insight Generation — auto-generate on topic complete + poll close
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<IInsightGenerationService, InsightGenerationService>();

// ---------------------------------------------------------------------------
// MCP Client — consume external MCP servers
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<IMcpContentClient, McpContentClient>();

// ---------------------------------------------------------------------------
// Build app
// ---------------------------------------------------------------------------
var app = builder.Build();

// Ensure database schema is created
{
    var dbFactory = app.Services.GetRequiredService<IDbContextFactory<ConferenceDbContext>>();
    await using var db = await dbFactory.CreateDbContextAsync();
    await db.Database.EnsureCreatedAsync();

    // EnsureCreatedAsync is a no-op when the DB already exists, so new tables
    // added after initial creation need explicit CREATE TABLE IF NOT EXISTS.
    await db.Database.ExecuteSqlRawAsync("""
        CREATE TABLE IF NOT EXISTS ingestion_records (
            "Id" text NOT NULL PRIMARY KEY,
            "DocumentId" varchar(500) NOT NULL,
            "Source" varchar(200) NOT NULL,
            "ContentHash" varchar(64) NOT NULL,
            "Status" varchar(20) NOT NULL,
            "ErrorMessage" varchar(2000),
            "CreatedAt" timestamp with time zone NOT NULL,
            "UpdatedAt" timestamp with time zone NOT NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS "IX_ingestion_records_DocumentId_Source"
            ON ingestion_records ("DocumentId", "Source");

        -- Add new columns to existing tables (safe for fresh + existing DBs)
        ALTER TABLE polls ADD COLUMN IF NOT EXISTS "AllowOther" boolean NOT NULL DEFAULT false;
        ALTER TABLE poll_responses ADD COLUMN IF NOT EXISTS "OtherText" varchar(500);
        """);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// MCP Streamable HTTP endpoint at /mcp
app.MapMcp("/mcp");

// Aspire health check endpoints
app.MapDefaultEndpoints();

// ---------------------------------------------------------------------------
// Wire up AI-powered pipelines via SessionCreated event
// When any session is created (startup demo or user-created), wire AI handlers
// ---------------------------------------------------------------------------
var sessionManager = app.Services.GetRequiredService<ISessionManager>();
var questionAnswering = app.Services.GetRequiredService<IQuestionAnsweringService>();
var ingestionService = app.Services.GetRequiredService<IIngestionService>();
var insightGen = app.Services.GetRequiredService<IInsightGenerationService>();

sessionManager.SessionCreated += ctx =>
{
    app.Logger.LogInformation("Session created: {Code} — {Title}", ctx.Session.SessionCode, ctx.Session.Title);

    var persistence = app.Services.GetRequiredService<ISessionPersistenceService>();

    // Save the new session to database
    _ = Task.Run(() => persistence.SaveSessionAsync(ctx));

    // Auto-answer audience questions via AI (use ctx directly to avoid GetDefaultContext mismatch)
    ctx.QuestionReceived += q =>
    {
        _ = Task.Run(async () =>
        {
            if (!await questionAnswering.IsQuestionSafeAsync(q.Text))
            {
                app.Logger.LogWarning("Question {QuestionId} flagged as inappropriate — hidden from attendees", q.Id);
                ctx.MarkQuestionUnsafe(q.Id);
                return;
            }
            var answer = await questionAnswering.GenerateAiAnswerTextAsync(q.Text, q.TopicId);
            if (!string.IsNullOrWhiteSpace(answer))
            {
                ctx.AnswerQuestion(q.Id, answer, isAiGenerated: true, authorLabel: "AI");
            }
        });
    };

    // When presenter approves an unsafe question, generate AI answer
    ctx.QuestionModerated += q =>
    {
        if (!q.IsApprovedByPresenter) return;
        _ = Task.Run(async () =>
        {
            var answer = await questionAnswering.GenerateAiAnswerTextAsync(q.Text, q.TopicId);
            if (!string.IsNullOrWhiteSpace(answer))
            {
                ctx.AnswerQuestion(q.Id, answer, isAiGenerated: true, authorLabel: "AI");
            }
        });
    };

    // Ingest poll results when a poll is closed + generate poll insights
    ctx.PollClosed += poll =>
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var results = ctx.GetPollResults(poll.Id);
                if (results.Count > 0)
                {
                    var otherResponses = ctx.GetOtherResponses(poll.Id);
                    await ingestionService.IngestResponseAsync(poll.Id, poll.TopicId ?? "", poll.Question, results, otherResponses);
                    app.Logger.LogInformation("Ingested poll results for {PollId} into knowledge base", poll.Id);
                }
                await insightGen.GeneratePollInsightsAsync(poll.Id, ctx.Session.SessionCode);
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(ex, "Failed to process poll close for {PollId}", poll.Id);
            }
        });
    };

    // Ingest Q&A pairs when a question is answered
    ctx.QuestionAnswered += q =>
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var latestAnswer = q.Answers.LastOrDefault();
                if (latestAnswer is null) return;
                var badge = latestAnswer.IsAiGenerated ? "[AI]" : "[Human]";
                var content = $"Q: {q.Text}\nA {badge}: {latestAnswer.Text}";
                await ingestionService.IngestExternalContentAsync("qa", content);
                app.Logger.LogInformation("Ingested Q&A pair into knowledge base ({Badge})", badge);
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(ex, "Failed to ingest Q&A pair");
            }
        });
    };

    // Ingest insights into the knowledge base when generated
    ctx.InsightGenerated += insight =>
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await ingestionService.IngestInsightAsync(insight.TopicId ?? "general", insight.Content);
                app.Logger.LogInformation("Ingested insight into knowledge base");
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(ex, "Failed to ingest insight");
            }
        });
    };

    // Generate topic summary + gap insights when a topic is completed
    ctx.TopicCompleted += topicId =>
    {
        _ = Task.Run(() => insightGen.GenerateTopicInsightsAsync(topicId, ctx.Session.SessionCode));
    };

    // Ingest questions immediately when received (before they're answered)
    ctx.QuestionReceived += q =>
    {
        _ = Task.Run(async () =>
        {
            try
            {
                if (!await questionAnswering.IsQuestionSafeAsync(q.Text))
                {
                    app.Logger.LogWarning("Question {QuestionId} flagged as inappropriate — skipping ingestion", q.Id);
                    return;
                }
                await ingestionService.IngestQuestionAsync(q.Id, q.Text, q.TopicId);
                app.Logger.LogInformation("Ingested question {QuestionId} into knowledge base", q.Id);

                // Auto-generate question-based insights (debounced internally)
                if (q.TopicId is not null)
                    await insightGen.GenerateQuestionInsightsAsync(q.TopicId, ctx.Session.SessionCode);
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(ex, "Failed to ingest question {QuestionId}", q.Id);
            }
        });
    };

    // Generate session summary on session end
    ctx.SessionEnded += () =>
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var workflow = app.Services.GetRequiredService<SessionSummaryWorkflow>();
                var summary = await workflow.ExecuteAsync();
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    await ingestionService.IngestSessionSummaryAsync(summary);
                    app.Logger.LogInformation("Session summary generated and ingested into knowledge base");
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(ex, "Failed to generate/ingest session summary");
            }
        });
    };

    // Persist to database on mutations
    ctx.PollActivated += poll => _ = Task.Run(() => persistence.SavePollAsync(ctx.Session.Id, poll));
    ctx.PollClosed += poll => _ = Task.Run(() => persistence.SavePollAsync(ctx.Session.Id, poll));
    ctx.ResponseReceived += response => _ = Task.Run(() => persistence.SavePollResponseAsync(response));
    ctx.QuestionReceived += q => _ = Task.Run(() => persistence.SaveQuestionAsync(ctx.Session.Id, q));
    ctx.QuestionAnswered += q =>
    {
        var latestAnswer = q.Answers.LastOrDefault();
        if (latestAnswer is not null)
            _ = Task.Run(() => persistence.SaveQuestionAnswerAsync(q.Id, latestAnswer));
    };
    ctx.InsightGenerated += insight => _ = Task.Run(() => persistence.SaveInsightAsync(ctx.Session.Id, insight));
};

// ---------------------------------------------------------------------------
// Startup: Restore persisted sessions, load default demo session, ingest outline
// ---------------------------------------------------------------------------

// Restore persisted sessions from database
var persistenceService = app.Services.GetRequiredService<ISessionPersistenceService>();
var savedSessions = await persistenceService.LoadAllSessionsAsync();
if (savedSessions.Count > 0)
{
    foreach (var saved in savedSessions)
    {
        if (sessionManager.GetSession(saved.SessionCode) is not null) continue;

        app.Logger.LogInformation("Restoring session: {Code} — {Title}", saved.SessionCode, saved.Title);
        var ctx = sessionManager.CreateSession(saved.Title, saved.HostPin, saved.SessionCode, saved.Description);

        // Overwrite the generated ID with the persisted one
        ctx.Session.Id = saved.Id;
        ctx.Session.Status = saved.Status;
        ctx.Session.CreatedAt = saved.CreatedAt;
        ctx.Session.StartedAt = saved.StartedAt;
        ctx.Session.EndedAt = saved.EndedAt;

        // Restore topics (replace the empty default ones)
        ctx.Session.Topics.Clear();
        foreach (var topic in saved.Topics.OrderBy(t => t.Order))
        {
            ctx.Session.Topics.Add(topic);
        }

        // Rehydrate runtime data (polls, responses, questions, insights, slides)
        var runtimeData = await persistenceService.LoadSessionRuntimeDataAsync(saved.Id);
        if (runtimeData is not null)
        {
            ctx.RestoreState(
                polls: runtimeData.Polls,
                responses: runtimeData.Responses,
                questions: runtimeData.Questions,
                insights: runtimeData.Insights,
                slides: runtimeData.Slides);

            app.Logger.LogInformation(
                "Rehydrated session {Code}: {Polls} polls, {Questions} questions, {Insights} insights, {Slides} slides",
                saved.SessionCode, runtimeData.Polls.Count, runtimeData.Questions.Count,
                runtimeData.Insights.Count, runtimeData.Slides.Count);
        }
    }
}

var dataRoot = Path.Combine(app.Environment.ContentRootPath, "..", "..", "data");

// Create the default demo session only if not already restored from database
var sessionService = app.Services.GetRequiredService<ISessionService>();
if (sessionManager.GetSession("DOTNETAI-CONF") is null)
{
    await sessionService.LoadSessionAsync(Path.Combine(dataRoot, "seed-topics.json"));
    app.Logger.LogInformation("Default session loaded: {Title} (code: {Code}, PIN: {Pin})",
        sessionService.CurrentSession?.Title,
        sessionService.CurrentSession?.SessionCode,
        "0000");
}
else
{
    // Set the default session code so SessionService points to the restored session
    sessionService.SetDefaultSession("DOTNETAI-CONF");
    app.Logger.LogInformation("Session restored from database: {Title} (code: {Code})",
        sessionService.CurrentSession?.Title,
        sessionService.CurrentSession?.SessionCode);
}

// Load slides from Markdown
var slidesPath = Path.Combine(dataRoot, "slides.md");
if (File.Exists(slidesPath))
{
    await sessionService.LoadSlidesAsync(slidesPath);
    app.Logger.LogInformation("Slides loaded: {Count} slides", sessionService.TotalSlides);
}
else
{
    app.Logger.LogWarning("No slides.md found at {Path} — slide features disabled", slidesPath);
}

// Initialize MCP client connections (Microsoft Learn + DeepWiki) in the background
_ = Task.Run(async () =>
{
    try
    {
        var mcpClient = app.Services.GetRequiredService<IMcpContentClient>();
        await mcpClient.InitializeAsync();
        app.Logger.LogInformation("MCP clients initialized (Microsoft Learn + DeepWiki)");
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "MCP client initialization failed — doc-augmented answers unavailable");
    }
});

app.Run();
