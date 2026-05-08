using System.ComponentModel;
using System.Text;
using ConferenceAssistant.Core.Models;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion.Services;
using ModelContextProtocol.Server;

namespace ConferenceAssistant.Mcp.Tools;

[McpServerToolType]
public class ConferenceTools
{
    [McpServerTool(Name = "get_session_status", ReadOnly = true),
     Description("Returns the current conference session status including title, status, active topic, and all topics with their statuses.")]
    public static string GetSessionStatus(ISessionService sessionService)
    {
        var session = sessionService.CurrentSession;
        if (session is null)
            return "No active conference session.";

        var sb = new StringBuilder();
        sb.AppendLine($"# {session.Title}");
        sb.AppendLine($"**Status:** {session.Status}");
        sb.AppendLine($"**Description:** {session.Description}");

        if (session.StartedAt.HasValue)
            sb.AppendLine($"**Started:** {session.StartedAt.Value:HH:mm:ss}");
        if (session.EndedAt.HasValue)
            sb.AppendLine($"**Ended:** {session.EndedAt.Value:HH:mm:ss}");

        var activeTopic = sessionService.GetActiveTopic();
        if (activeTopic is not null)
        {
            sb.AppendLine();
            sb.AppendLine($"## Active Topic: {activeTopic.Title}");
            sb.AppendLine(activeTopic.Description);
        }

        sb.AppendLine();
        sb.AppendLine("## Topics");
        foreach (var topic in session.Topics.OrderBy(t => t.Order))
        {
            var marker = topic.Status == TopicStatus.Active ? "▶" :
                         topic.Status == TopicStatus.Completed ? "✓" : "○";
            sb.AppendLine($"- {marker} **{topic.Title}** ({topic.Status})");
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_active_poll", ReadOnly = true),
     Description("Returns the currently active poll with its question, options, and current vote counts.")]
    public static string GetActivePoll(IPollService pollService)
    {
        var poll = pollService.GetActivePoll();
        if (poll is null)
            return "No poll is currently active.";

        var results = pollService.GetPollResults(poll.Id);
        var totalVotes = results.Values.Sum();

        var sb = new StringBuilder();
        sb.AppendLine("# Active Poll");
        sb.AppendLine($"**Question:** {poll.Question}");
        sb.AppendLine($"**Total Votes:** {totalVotes}");
        sb.AppendLine();

        foreach (var option in poll.Options)
        {
            var count = results.GetValueOrDefault(option, 0);
            var percentage = totalVotes > 0 ? (count * 100.0 / totalVotes).ToString("F1") : "0.0";
            sb.AppendLine($"- **{option}**: {count} votes ({percentage}%)");
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_poll_results", ReadOnly = true),
     Description("Returns detailed results for a specific poll including question, options, vote counts, and percentages.")]
    public static string GetPollResults(
        IPollService pollService,
        [Description("The unique identifier of the poll to get results for.")] string pollId)
    {
        var results = pollService.GetPollResults(pollId);
        if (results.Count == 0)
            return $"No results found for poll '{pollId}'.";

        var totalVotes = results.Values.Sum();

        var sb = new StringBuilder();
        sb.AppendLine("# Poll Results");
        sb.AppendLine($"**Poll ID:** {pollId}");
        sb.AppendLine($"**Total Responses:** {totalVotes}");
        sb.AppendLine();

        foreach (var (option, count) in results.OrderByDescending(r => r.Value))
        {
            var percentage = totalVotes > 0 ? (count * 100.0 / totalVotes).ToString("F1") : "0.0";
            var bar = new string('█', totalVotes > 0 ? (int)(count * 20.0 / totalVotes) : 0);
            sb.AppendLine($"- **{option}**: {count} ({percentage}%) {bar}");
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "search_session_knowledge", ReadOnly = true),
     Description("Searches the session knowledge base (vector store) for content relevant to the query.")]
    public static async Task<string> SearchSessionKnowledge(
        ISemanticSearchService searchService,
        [Description("The search query to find relevant session content.")] string query,
        [Description("Maximum number of results to return. Defaults to 5.")] int maxResults = 5)
    {
        var results = await searchService.SearchAsync(query, maxResults);
        if (results.Count == 0)
            return $"No results found for query: '{query}'";

        var sb = new StringBuilder();
        sb.AppendLine($"# Search Results for: \"{query}\"");
        sb.AppendLine($"Found {results.Count} result(s).");
        sb.AppendLine();

        for (var i = 0; i < results.Count; i++)
        {
            var record = results[i];
            sb.AppendLine($"## Result {i + 1}");
            sb.AppendLine($"**Source:** {record.Source}");
            sb.AppendLine($"**Content:** {record.Content}");
            if (!string.IsNullOrEmpty(record.Context))
                sb.AppendLine($"**Context:** {record.Context}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_audience_questions", ReadOnly = true),
     Description("Returns the top audience questions ordered by upvotes.")]
    public static string GetAudienceQuestions(
        IQuestionService questionService,
        [Description("Number of top questions to return. Defaults to 10.")] int count = 10)
    {
        var questions = questionService.GetTopQuestions(count);
        if (questions.Count == 0)
            return "No audience questions have been submitted yet.";

        var sb = new StringBuilder();
        sb.AppendLine($"# Top {questions.Count} Audience Questions");
        sb.AppendLine();

        for (var i = 0; i < questions.Count; i++)
        {
            var q = questions[i];
            sb.AppendLine($"### {i + 1}. {q.Text}");
            sb.AppendLine($"👍 {q.Upvotes} upvotes | Asked at {q.AskedAt:HH:mm:ss}");
            if (q.Answers.Count > 0)
            {
                foreach (var a in q.Answers)
                {
                    var badge = a.IsAiGenerated ? "🤖 AI" : $"💬 {a.AuthorLabel}";
                    sb.AppendLine($"**Answer ({badge}):** {a.Text}");
                }
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_topic_insights", ReadOnly = true),
     Description("Returns all AI-generated insights for a specific topic.")]
    public static string GetTopicInsights(
        IInsightService insightService,
        [Description("The unique identifier of the topic to get insights for.")] string topicId)
    {
        var insights = insightService.GetInsightsForTopic(topicId);
        if (insights.Count == 0)
            return $"No insights available for topic '{topicId}'.";

        var sb = new StringBuilder();
        sb.AppendLine($"# Insights for Topic: {topicId}");
        sb.AppendLine($"Total: {insights.Count} insight(s)");
        sb.AppendLine();

        foreach (var insight in insights)
        {
            sb.AppendLine($"### {insight.Type}");
            sb.AppendLine(insight.Content);
            sb.AppendLine($"*Generated at {insight.GeneratedAt:HH:mm:ss}*");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_all_insights", ReadOnly = true),
     Description("Returns all AI-generated insights from the entire session.")]
    public static string GetAllInsights(IInsightService insightService)
    {
        var insights = insightService.GetAllInsights();
        if (insights.Count == 0)
            return "No insights have been generated yet.";

        var sb = new StringBuilder();
        sb.AppendLine("# All Session Insights");
        sb.AppendLine($"Total: {insights.Count} insight(s)");
        sb.AppendLine();

        foreach (var group in insights.GroupBy(i => i.Type))
        {
            sb.AppendLine($"## {group.Key}");
            foreach (var insight in group)
            {
                sb.Append($"- {insight.Content}");
                var meta = new List<string>();
                if (!string.IsNullOrEmpty(insight.TopicId))
                    meta.Add($"Topic: {insight.TopicId}");
                if (!string.IsNullOrEmpty(insight.PollId))
                    meta.Add($"Poll: {insight.PollId}");
                if (meta.Count > 0)
                    sb.Append($" *({string.Join(", ", meta)})*");
                sb.AppendLine();
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "generate_session_summary", ReadOnly = true),
     Description("Generates a comprehensive summary of the entire conference session including all polls, insights, audience questions, and knowledge base statistics.")]
    public static async Task<string> GenerateSessionSummary(
        ISessionService sessionService,
        IPollService pollService,
        IInsightService insightService,
        IQuestionService questionService,
        ISemanticSearchService searchService)
    {
        var session = sessionService.CurrentSession;
        if (session is null)
            return "No conference session data available.";

        var sb = new StringBuilder();

        // Session overview
        sb.AppendLine("# 📋 Conference Session Summary");
        sb.AppendLine($"## {session.Title}");
        sb.AppendLine($"**Description:** {session.Description}");
        sb.AppendLine($"**Status:** {session.Status}");

        if (session.StartedAt.HasValue)
        {
            sb.AppendLine($"**Started:** {session.StartedAt.Value:yyyy-MM-dd HH:mm:ss}");
            var endTime = session.EndedAt ?? DateTimeOffset.UtcNow;
            var duration = endTime - session.StartedAt.Value;
            sb.AppendLine($"**Duration:** {duration.Hours}h {duration.Minutes}m {duration.Seconds}s");
        }

        sb.AppendLine();

        // Topics covered
        sb.AppendLine("## 📌 Topics");
        foreach (var topic in session.Topics.OrderBy(t => t.Order))
        {
            var marker = topic.Status == TopicStatus.Completed ? "✅" :
                         topic.Status == TopicStatus.Active ? "▶️" : "⏳";
            sb.AppendLine($"### {marker} {topic.Title}");
            sb.AppendLine(topic.Description);

            if (topic.TalkingPoints.Count > 0)
            {
                sb.AppendLine("**Talking Points:**");
                foreach (var point in topic.TalkingPoints)
                    sb.AppendLine($"  - {point}");
            }

            // Polls for this topic
            var topicPolls = pollService.GetPollsForTopic(topic.Id);
            if (topicPolls.Count > 0)
            {
                sb.AppendLine($"**Polls ({topicPolls.Count}):**");
                foreach (var poll in topicPolls)
                {
                    var results = pollService.GetPollResults(poll.Id);
                    var totalVotes = results.Values.Sum();
                    sb.AppendLine($"  - *{poll.Question}* ({totalVotes} votes, {poll.Status})");
                    foreach (var (option, count) in results.OrderByDescending(r => r.Value))
                    {
                        var pct = totalVotes > 0 ? (count * 100.0 / totalVotes).ToString("F1") : "0.0";
                        sb.AppendLine($"    - {option}: {count} ({pct}%)");
                    }
                }
            }

            // Insights for this topic
            var topicInsights = insightService.GetInsightsForTopic(topic.Id);
            if (topicInsights.Count > 0)
            {
                sb.AppendLine($"**Insights ({topicInsights.Count}):**");
                foreach (var insight in topicInsights)
                    sb.AppendLine($"  - [{insight.Type}] {insight.Content}");
            }

            sb.AppendLine();
        }

        // All insights summary
        var allInsights = insightService.GetAllInsights();
        if (allInsights.Count > 0)
        {
            sb.AppendLine("## 💡 All Insights");
            foreach (var group in allInsights.GroupBy(i => i.Type))
            {
                sb.AppendLine($"### {group.Key} ({group.Count()})");
                foreach (var insight in group)
                    sb.AppendLine($"- {insight.Content}");
                sb.AppendLine();
            }
        }

        // Audience questions
        var topQuestions = questionService.GetTopQuestions(20);
        if (topQuestions.Count > 0)
        {
            sb.AppendLine("## ❓ Audience Questions");
            sb.AppendLine($"Showing top {topQuestions.Count} question(s) by upvotes:");
            sb.AppendLine();
            foreach (var q in topQuestions)
            {
                sb.AppendLine($"- **{q.Text}** (👍 {q.Upvotes})");
                if (q.Answers.Count > 0)
                {
                    foreach (var a in q.Answers)
                    {
                        var badge = a.IsAiGenerated ? "AI" : a.AuthorLabel;
                        sb.AppendLine($"  - *Answer [{badge}]: {a.Text}*");
                    }
                }
            }
            sb.AppendLine();
        }

        // Knowledge base stats
        var recordCount = await searchService.GetRecordCountAsync();
        sb.AppendLine("## 📚 Knowledge Base");
        sb.AppendLine($"**Total Records:** {recordCount}");
        sb.AppendLine();

        // Final stats
        var totalPolls = session.Topics.Sum(t => pollService.GetPollsForTopic(t.Id).Count);
        sb.AppendLine("## 📊 Session Statistics");
        sb.AppendLine($"- **Topics:** {session.Topics.Count}");
        sb.AppendLine($"- **Polls Conducted:** {totalPolls}");
        sb.AppendLine($"- **Insights Generated:** {allInsights.Count}");
        sb.AppendLine($"- **Audience Questions:** {topQuestions.Count}");
        sb.AppendLine($"- **Knowledge Base Records:** {recordCount}");

        return sb.ToString();
    }
}
