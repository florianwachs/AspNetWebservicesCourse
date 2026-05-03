using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using ConferenceAssistant.Agents.Tools;

namespace ConferenceAssistant.Agents.Workflows;

/// <summary>
/// Orchestrates a fan-out/fan-in → synthesis workflow:
/// three specialized ChatClientAgents analyze polls, questions, and insights concurrently,
/// then a Synthesizer agent merges their outputs into a single session summary.
/// The entire pipeline runs as one MAF workflow.
/// </summary>
public class SessionSummaryWorkflow(IChatClient chatClient, AgentTools tools)
{
    public async Task<string> ExecuteAsync()
    {
        // Three specialized analysis agents run concurrently (fan-out)
        // Each agent gets ONLY the tools it needs
        ChatClientAgent pollAnalyst = new(
            chatClient,
            name: "PollAnalyst",
            description: "Analyzes poll results and trends",
            instructions: """
                You are a poll analyst. Use GetAllPollResults to retrieve every poll
                and its results. Summarize the key findings: which options won,
                participation levels, and trends across polls. Be data-driven — cite percentages.
                """,
            tools: [tools.GetAllPollResults]);

        ChatClientAgent questionAnalyst = new(
            chatClient,
            name: "QuestionAnalyst",
            description: "Analyzes audience questions and themes",
            instructions: """
                You are an audience question analyst. Use GetAudienceQuestions to retrieve
                all audience questions. Identify the top themes, most-upvoted questions,
                knowledge gaps (unanswered or high-interest questions), and overall curiosity patterns.
                """,
            tools: [tools.GetAudienceQuestions]);

        ChatClientAgent insightAnalyst = new(
            chatClient,
            name: "InsightAnalyst",
            description: "Analyzes generated insights and knowledge patterns",
            instructions: """
                You are an insight analyst. Use GetAllInsights and SearchKnowledge to
                gather all generated insights and knowledge base content. Identify
                overarching themes, recurring patterns, and key takeaways from the session.
                """,
            tools: [tools.GetAllInsights, tools.SearchKnowledge]);

        // Synthesizer agent: receives merged outputs from the 3 analysts
        ChatClientAgent synthesizer = new(
            chatClient,
            name: "Synthesizer",
            description: "Synthesizes multiple analyses into a cohesive summary",
            instructions: """
                You are a conference session summarizer. You will receive analyses from
                specialized analysts (polls, questions, insights). Synthesize them into
                one cohesive, narrative session summary (4-6 paragraphs). Highlight key
                audience sentiment, knowledge gaps, and actionable takeaways.
                """);

        // Build the concurrent analysis as a sub-workflow
        var analysisWorkflow = AgentWorkflowBuilder.BuildConcurrent(
            [pollAnalyst, questionAnalyst, insightAnalyst],
            MergeAgentOutputs);

        // Compose: concurrent(analysts) → sequential(synthesizer) via WorkflowBuilder
        var analysisExec = new SubworkflowBinding(analysisWorkflow, "Analysis");
        ExecutorBinding synthExec = synthesizer;

        var composedWorkflow = new WorkflowBuilder(analysisExec)
            .WithName("SessionSummaryPipeline")
            .BindExecutor(synthExec)
            .AddEdge(analysisExec, synthExec)
            .WithOutputFrom([synthExec])
            .Build();

        // Single execution call for the entire pipeline
        var run = await InProcessExecution.Default.RunAsync(
            composedWorkflow,
            "Analyze the conference session data and provide your specialized findings.");

        // Extract the synthesizer's final output
        foreach (var evt in run.NewEvents)
        {
            if (evt is ExecutorCompletedEvent completed && completed.Data is IEnumerable<ChatMessage> msgs)
            {
                var text = string.Join("\n", msgs
                    .Where(m => m.Role == ChatRole.Assistant && !string.IsNullOrWhiteSpace(m.Text))
                    .Select(m => m.Text));
                if (!string.IsNullOrWhiteSpace(text))
                    return text;
            }
        }

        return "Unable to generate session summary — no agent outputs collected.";
    }

    private static List<ChatMessage> MergeAgentOutputs(IList<List<ChatMessage>> agentResults)
    {
        var merged = new List<ChatMessage>();
        foreach (var agentMessages in agentResults)
        {
            merged.AddRange(agentMessages);
        }
        return merged;
    }
}
