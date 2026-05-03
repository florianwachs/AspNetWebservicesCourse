using System.Collections.Concurrent;
using System.Text;
using ConferenceAssistant.Core.Models;
using ConferenceAssistant.Core.Services;
using ConferenceAssistant.Ingestion.Services;
using Microsoft.Extensions.AI;

namespace ConferenceAssistant.Web.Services;

public class InsightGenerationService(
    IChatClient chatClient,
    ISessionManager sessionManager,
    ISemanticSearchService searchService,
    ILogger<InsightGenerationService> logger) : IInsightGenerationService
{
    private readonly ConcurrentDictionary<string, DateTime> _lastInsightTime = new();
    private static readonly TimeSpan InsightCooldown = TimeSpan.FromSeconds(30);

    private SessionContext GetContext(string sessionCode) =>
        sessionManager.GetSession(sessionCode)
            ?? throw new InvalidOperationException($"Session '{sessionCode}' not found.");

    public async Task GeneratePollInsightsAsync(string pollId, string sessionCode)
    {
        try
        {
            var ctx = GetContext(sessionCode);
            var poll = ctx.GetPoll(pollId);
            if (poll is null)
            {
                logger.LogError("Cannot generate poll insights: poll {PollId} not found in session {Code}", pollId, sessionCode);
                return;
            }

            var results = ctx.GetPollResults(pollId);
            if (results.Count == 0) return;

            var sb = new StringBuilder();
            sb.AppendLine($"Poll: {poll.Question}");
            var total = results.Values.Sum();
            foreach (var (option, count) in results.OrderByDescending(r => r.Value))
            {
                var pct = total > 0 ? (count * 100.0 / total).ToString("F0") : "0";
                sb.AppendLine($"  - {option}: {count} votes ({pct}%)");
            }
            sb.AppendLine($"Total responses: {total}");

            var otherResponses = ctx.GetOtherResponses(pollId);
            if (otherResponses.Count > 0)
            {
                sb.AppendLine("\"Other\" responses from attendees:");
                foreach (var text in otherResponses)
                    sb.AppendLine($"  - \"{text}\"");
            }

            var prompt = $"""
                Analyze these live poll results from a conference session. Provide 1-2 short, 
                actionable insights about what the audience thinks. Be specific and reference 
                the actual numbers. If there are "Other" responses, identify themes or patterns 
                in the free-text answers. Keep it under 3 sentences.

                {sb}
                """;

            var response = await chatClient.GetResponseAsync(
            [
                new(ChatRole.System, "You are a conference analytics assistant generating real-time insights from audience data."),
                new(ChatRole.User, prompt)
            ]);

            var content = response.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(content))
            {
                ctx.AddInsight(new Insight
                {
                    TopicId = poll.TopicId,
                    PollId = pollId,
                    Content = content,
                    Type = InsightType.PollAnalysis
                });
                logger.LogInformation("Generated poll insight for poll {PollId} in session {Code}", pollId, sessionCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate poll insight for {PollId}", pollId);
        }
    }

    public async Task GenerateTopicInsightsAsync(string topicId, string sessionCode)
    {
        try
        {
            var ctx = GetContext(sessionCode);
            var topic = ctx.Session.Topics.FirstOrDefault(t => t.Id == topicId);
            if (topic is null) return;

            var sb = new StringBuilder();
            sb.AppendLine($"# Topic: {topic.Title}");

            var polls = ctx.GetPollsForTopic(topicId);
            if (polls.Count > 0)
            {
                sb.AppendLine("\n## Polls:");
                foreach (var poll in polls)
                {
                    var results = ctx.GetPollResults(poll.Id);
                    var total = results.Values.Sum();
                    sb.AppendLine($"\nQ: {poll.Question} ({total} responses)");
                    foreach (var (option, count) in results.OrderByDescending(r => r.Value))
                    {
                        var pct = total > 0 ? (count * 100.0 / total).ToString("F0") : "0";
                        sb.AppendLine($"  - {option}: {count} ({pct}%)");
                    }
                }
            }

            var questions = ctx.GetQuestionsForTopic(topicId);
            if (questions.Count > 0)
            {
                sb.AppendLine("\n## Audience Questions:");
                foreach (var q in questions.OrderByDescending(q => q.Upvotes).Take(10))
                {
                    sb.AppendLine($"- [{q.Upvotes} votes] {q.Text}");
                    if (q.Answers.Count > 0)
                    {
                        foreach (var a in q.Answers)
                        {
                            var badge = a.IsAiGenerated ? "[AI]" : "[Human]";
                            sb.AppendLine($"  Answer {badge}: {a.Text}");
                        }
                    }
                }
            }

            var kbResults = await searchService.SearchAsync($"topic {topic.Title}", topK: 3);
            if (kbResults.Count > 0)
            {
                sb.AppendLine("\n## Session Context:");
                foreach (var r in kbResults)
                    sb.AppendLine(r.Content);
            }

            var prompt = $"""
                The following topic just completed in a live conference session. Summarize the key 
                audience takeaways: what interested them most (based on poll results), what gaps 
                they have (based on questions), and any trends you see. Keep it to 3-4 sentences.

                {sb}
                """;

            var response = await chatClient.GetResponseAsync(
            [
                new(ChatRole.System, "You are a conference analytics assistant. Generate a concise topic summary that captures audience sentiment and engagement."),
                new(ChatRole.User, prompt)
            ]);

            var content = response.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(content))
            {
                ctx.AddInsight(new Insight
                {
                    TopicId = topicId,
                    Content = content,
                    Type = InsightType.TopicSummary
                });
                logger.LogInformation("Generated topic summary insight for {TopicId}", topicId);
            }

            // Detect knowledge gaps from unanswered/highly-upvoted questions
            var gapQuestions = questions.Where(q => q.Upvotes >= 2 || q.Answers.Count == 0).ToList();
            if (gapQuestions.Count > 0)
            {
                var gapPrompt = $"""
                    Based on these audience questions (sorted by upvotes), identify the top 1-2 
                    knowledge gaps the audience has. Be concise (1-2 sentences).

                    {string.Join("\n", gapQuestions.Select(q => $"[{q.Upvotes} votes] {q.Text}"))}
                    """;

                var gapResponse = await chatClient.GetResponseAsync(gapPrompt);
                var gapContent = gapResponse.Text?.Trim();
                if (!string.IsNullOrWhiteSpace(gapContent))
                {
                    ctx.AddInsight(new Insight
                    {
                        TopicId = topicId,
                        Content = gapContent,
                        Type = InsightType.KnowledgeGap
                    });
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate topic insights for {TopicId}", topicId);
        }
    }

    public async Task GenerateQuestionInsightsAsync(string topicId, string sessionCode)
    {
        if (_lastInsightTime.TryGetValue(topicId, out var lastTime)
            && DateTime.UtcNow - lastTime < InsightCooldown)
        {
            logger.LogDebug("Skipping question insight for {TopicId} — cooldown active", topicId);
            return;
        }

        try
        {
            var ctx = GetContext(sessionCode);
            var topic = ctx.Session.Topics.FirstOrDefault(t => t.Id == topicId);
            if (topic is null) return;

            var questions = ctx.GetQuestionsForTopic(topicId);
            if (questions.Count < 3) return;

            var questionList = string.Join("\n", questions
                .OrderByDescending(q => q.Upvotes)
                .Take(10)
                .Select(q => $"[{q.Upvotes} votes] {q.Text}"));

            var prompt = $"""
                These are audience questions from a live conference session on "{topic.Title}".
                Identify the main theme or pattern in what the audience is asking about.
                Provide a brief, actionable insight (1-2 sentences) about what the audience
                wants to learn more about.

                {questionList}
                """;

            var response = await chatClient.GetResponseAsync(
            [
                new(ChatRole.System, "You are a conference analytics assistant. Generate a brief insight about audience curiosity patterns."),
                new(ChatRole.User, prompt)
            ]);

            var content = response.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(content))
            {
                ctx.AddInsight(new Insight
                {
                    TopicId = topicId,
                    Content = content,
                    Type = InsightType.AudienceTrend
                });
                _lastInsightTime[topicId] = DateTime.UtcNow;
                logger.LogInformation("Generated question-based insight for topic {TopicId}", topicId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate question insight for {TopicId}", topicId);
        }
    }
}
