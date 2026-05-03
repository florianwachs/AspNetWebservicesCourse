using System.Text;
using Microsoft.Extensions.AI;
using ConferenceAssistant.Core.Models;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion.Services;

namespace ConferenceAssistant.Agents.Tools;

public class AgentTools
{
    private readonly IPollService _pollService;
    private readonly IInsightService _insightService;
    private readonly IQuestionService _questionService;
    private readonly ISessionService _sessionService;
    private readonly ISemanticSearchService _searchService;

    public AITool SearchKnowledge { get; }
    public AITool GetCurrentTopic { get; }
    public AITool GetPollResults { get; }
    public AITool CreatePoll { get; }
    public AITool SaveInsight { get; }
    public AITool GetAudienceQuestions { get; }
    public AITool GetAllInsights { get; }
    public AITool GetAllPollResults { get; }

    /// <summary>All tools as a collection.</summary>
    public IList<AITool> All =>
        [SearchKnowledge, GetCurrentTopic, GetPollResults, CreatePoll,
         SaveInsight, GetAudienceQuestions, GetAllInsights, GetAllPollResults];

    public AgentTools(
        IPollService pollService,
        IInsightService insightService,
        IQuestionService questionService,
        ISessionService sessionService,
        ISemanticSearchService searchService)
    {
        _pollService = pollService;
        _insightService = insightService;
        _questionService = questionService;
        _sessionService = sessionService;
        _searchService = searchService;

        SearchKnowledge = AIFunctionFactory.Create(SearchKnowledgeCore, new AIFunctionFactoryOptions
        {
            Name = nameof(SearchKnowledge),
            Description = "Search the session knowledge base for content related to the query"
        });
        GetCurrentTopic = AIFunctionFactory.Create(GetCurrentTopicCore, new AIFunctionFactoryOptions
        {
            Name = nameof(GetCurrentTopic),
            Description = "Get the current active topic being discussed"
        });
        GetPollResults = AIFunctionFactory.Create(GetPollResultsCore, new AIFunctionFactoryOptions
        {
            Name = nameof(GetPollResults),
            Description = "Get results for a specific poll showing vote counts per option"
        });
        CreatePoll = AIFunctionFactory.Create(CreatePollCore, new AIFunctionFactoryOptions
        {
            Name = nameof(CreatePoll),
            Description = "Create a new poll for the audience to vote on. Set allowOther=true to let attendees provide free-text responses beyond the listed options."
        });
        SaveInsight = AIFunctionFactory.Create(SaveInsightCore, new AIFunctionFactoryOptions
        {
            Name = nameof(SaveInsight),
            Description = "Save an AI-generated insight about the session"
        });
        GetAudienceQuestions = AIFunctionFactory.Create(GetAudienceQuestionsCore, new AIFunctionFactoryOptions
        {
            Name = nameof(GetAudienceQuestions),
            Description = "Get all audience questions, optionally filtered by topic"
        });
        GetAllInsights = AIFunctionFactory.Create(GetAllInsightsCore, new AIFunctionFactoryOptions
        {
            Name = nameof(GetAllInsights),
            Description = "Get all insights generated during the session"
        });
        GetAllPollResults = AIFunctionFactory.Create(GetAllPollResultsCore, new AIFunctionFactoryOptions
        {
            Name = nameof(GetAllPollResults),
            Description = "Get all poll results for every poll in the session"
        });
    }

    private async Task<string> SearchKnowledgeCore(string query, int maxResults = 5)
    {
        var results = await _searchService.SearchAsync(query, maxResults);
        if (results.Count == 0) return "No relevant content found.";
        return string.Join("\n\n---\n\n", results.Select(r => $"[{r.Source}] {r.Content}"));
    }

    private string GetCurrentTopicCore()
    {
        var topic = _sessionService.GetActiveTopic();
        if (topic is null) return "No active topic.";
        return $"Topic: {topic.Title}\nDescription: {topic.Description}\nTalking Points:\n{string.Join("\n- ", topic.TalkingPoints)}";
    }

    private string GetPollResultsCore(string pollId)
    {
        var results = _pollService.GetPollResults(pollId);
        if (results.Count == 0) return "No results yet.";
        var total = results.Values.Sum();
        return string.Join("\n", results.Select(kv =>
            $"- {kv.Key}: {kv.Value} votes ({(total > 0 ? 100 * kv.Value / total : 0)}%)"));
    }

    private async Task<string> CreatePollCore(string? topicId, string question, string[] options, bool allowOther = true)
    {
        var poll = await _pollService.CreatePollAsync(topicId, question, options.ToList(), PollSource.Generated, allowOther);
        return $"Poll created with ID: {poll.Id}";
    }

    private async Task<string> SaveInsightCore(string content, string insightType, string? pollId = null, string? topicId = null)
    {
        var type = Enum.TryParse<InsightType>(insightType, true, out var t) ? t : InsightType.PollAnalysis;
        var insight = new Insight
        {
            Content = content,
            Type = type,
            PollId = pollId,
            TopicId = topicId
        };
        await _insightService.AddInsightAsync(insight);
        return $"Insight saved: {insight.Id}";
    }

    private string GetAudienceQuestionsCore(string? topicId = null)
    {
        var questions = topicId is not null
            ? _questionService.GetQuestionsForTopic(topicId)
            : _questionService.GetTopQuestions(20);
        if (questions.Count == 0) return "No audience questions yet.";
        return string.Join("\n", questions.Select(q => $"- [{q.Upvotes} votes] {q.Text}"));
    }

    private string GetAllInsightsCore()
    {
        var insights = _insightService.GetAllInsights();
        if (insights.Count == 0) return "No insights generated yet.";
        return string.Join("\n\n", insights.Select(i => $"[{i.Type}] {i.Content}"));
    }

    private string GetAllPollResultsCore()
    {
        var session = _sessionService.CurrentSession;
        if (session is null) return "No active session.";

        var sb = new StringBuilder();
        foreach (var topic in session.Topics)
        {
            var polls = _pollService.GetPollsForTopic(topic.Id);
            foreach (var poll in polls)
            {
                var results = _pollService.GetPollResults(poll.Id);
                var total = results.Values.Sum();
                sb.AppendLine($"## {poll.Question} (Topic: {topic.Title})");
                foreach (var kv in results)
                    sb.AppendLine($"  - {kv.Key}: {kv.Value} ({(total > 0 ? 100 * kv.Value / total : 0)}%)");

                var otherResponses = _pollService.GetOtherResponses(poll.Id);
                if (otherResponses.Count > 0)
                {
                    sb.AppendLine("  \"Other\" responses:");
                    foreach (var text in otherResponses)
                        sb.AppendLine($"    - \"{text}\"");
                }

                sb.AppendLine();
            }
        }
        return sb.Length > 0 ? sb.ToString() : "No polls completed yet.";
    }
}
