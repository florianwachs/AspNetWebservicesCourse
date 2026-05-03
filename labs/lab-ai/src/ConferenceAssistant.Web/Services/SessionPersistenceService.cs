using Microsoft.EntityFrameworkCore;
using ConferenceAssistant.Core.Models;
using ConferenceAssistant.Web.Data;

namespace ConferenceAssistant.Web.Services;

public class SessionPersistenceService(
    IDbContextFactory<ConferenceDbContext> dbFactory,
    ILogger<SessionPersistenceService> logger) : ISessionPersistenceService
{
    public async Task SaveSessionAsync(SessionContext context)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            var session = context.Session;

            var existing = await db.Sessions
                .Include(s => s.Topics)
                .FirstOrDefaultAsync(s => s.Id == session.Id);

            if (existing is null)
            {
                db.Sessions.Add(session);
                foreach (var topic in session.Topics)
                {
                    db.Entry(topic).Property("SessionId").CurrentValue = session.Id;
                }
            }
            else
            {
                db.Entry(existing).CurrentValues.SetValues(session);
                foreach (var topic in session.Topics)
                {
                    var existingTopic = existing.Topics.FirstOrDefault(t => t.Id == topic.Id);
                    if (existingTopic is null)
                    {
                        db.Entry(topic).Property("SessionId").CurrentValue = session.Id;
                        db.Topics.Add(topic);
                    }
                    else
                    {
                        db.Entry(existingTopic).CurrentValues.SetValues(topic);
                    }
                }
            }

            // Save slides
            var slides = context.AllSlides;
            foreach (var slide in slides)
            {
                var existingSlide = await db.Slides.FindAsync(slide.Id);
                if (existingSlide is null)
                {
                    db.Entry(slide).Property("SessionId").CurrentValue = session.Id;
                    db.Slides.Add(slide);
                }
                else
                {
                    db.Entry(existingSlide).CurrentValues.SetValues(slide);
                }
            }

            await db.SaveChangesAsync();
            logger.LogInformation("Session {Code} saved to database", session.SessionCode);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save session to database");
        }
    }

    public async Task<ConferenceSession?> LoadSessionAsync(string sessionCode)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            return await db.Sessions
                .Include(s => s.Topics)
                .FirstOrDefaultAsync(s => s.SessionCode == sessionCode);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load session {Code}", sessionCode);
            return null;
        }
    }

    public async Task<List<ConferenceSession>> LoadAllSessionsAsync()
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            return await db.Sessions
                .Include(s => s.Topics)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load sessions from database");
            return [];
        }
    }

    public async Task<PersistedSessionData?> LoadSessionRuntimeDataAsync(string sessionId)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();

            var polls = await db.Polls
                .Where(p => EF.Property<string>(p, "SessionId") == sessionId)
                .ToListAsync();

            var pollIds = polls.Select(p => p.Id).ToList();
            var responses = await db.PollResponses
                .Where(r => pollIds.Contains(r.PollId))
                .ToListAsync();

            var questions = await db.Questions
                .Include(q => q.Answers)
                .Where(q => EF.Property<string>(q, "SessionId") == sessionId)
                .ToListAsync();

            var insights = await db.Insights
                .Where(i => EF.Property<string>(i, "SessionId") == sessionId)
                .ToListAsync();

            var slides = await db.Slides
                .Where(s => EF.Property<string>(s, "SessionId") == sessionId)
                .OrderBy(s => s.Order)
                .ToListAsync();

            return new PersistedSessionData(polls, responses, questions, insights, slides);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load runtime data for session {SessionId}", sessionId);
            return null;
        }
    }

    public async Task SavePollAsync(string sessionId, Poll poll)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            var existing = await db.Polls.FindAsync(poll.Id);
            if (existing is null)
            {
                db.Entry(poll).Property("SessionId").CurrentValue = sessionId;
                db.Polls.Add(poll);
            }
            else
            {
                db.Entry(existing).CurrentValues.SetValues(poll);
            }
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save poll {PollId}", poll.Id);
        }
    }

    public async Task SavePollResponseAsync(PollResponse response)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            db.PollResponses.Add(response);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save poll response");
        }
    }

    public async Task SaveQuestionAsync(string sessionId, AudienceQuestion question)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            var existing = await db.Questions.FindAsync(question.Id);
            if (existing is null)
            {
                db.Entry(question).Property("SessionId").CurrentValue = sessionId;
                db.Questions.Add(question);
            }
            else
            {
                db.Entry(existing).CurrentValues.SetValues(question);
            }
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save question {QuestionId}", question.Id);
        }
    }

    public async Task SaveQuestionAnswerAsync(string questionId, QuestionAnswer answer)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            db.Entry(answer).Property("QuestionId").CurrentValue = questionId;
            db.QuestionAnswers.Add(answer);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save question answer");
        }
    }

    public async Task SaveInsightAsync(string sessionId, Insight insight)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();
            db.Entry(insight).Property("SessionId").CurrentValue = sessionId;
            db.Insights.Add(insight);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save insight {InsightId}", insight.Id);
        }
    }

    public async Task ClearRuntimeDataAsync(string sessionId)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();

            // Get all poll IDs for this session to clear responses
            var pollIds = await db.Polls
                .Where(p => EF.Property<string>(p, "SessionId") == sessionId)
                .Select(p => p.Id)
                .ToListAsync();

            // Get all question IDs for this session to clear answers
            var questionIds = await db.Questions
                .Where(q => EF.Property<string>(q, "SessionId") == sessionId)
                .Select(q => q.Id)
                .ToListAsync();

            // Delete in order: answers → questions, responses → polls, insights
            await db.QuestionAnswers
                .Where(a => questionIds.Contains(EF.Property<string>(a, "QuestionId")))
                .ExecuteDeleteAsync();
            await db.Questions
                .Where(q => EF.Property<string>(q, "SessionId") == sessionId)
                .ExecuteDeleteAsync();
            await db.PollResponses
                .Where(r => pollIds.Contains(r.PollId))
                .ExecuteDeleteAsync();
            await db.Polls
                .Where(p => EF.Property<string>(p, "SessionId") == sessionId)
                .ExecuteDeleteAsync();
            await db.Insights
                .Where(i => EF.Property<string>(i, "SessionId") == sessionId)
                .ExecuteDeleteAsync();

            logger.LogInformation("Cleared runtime data for session {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to clear runtime data for session {SessionId}", sessionId);
        }
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync();

            // Delete child entities in FK order
            var pollIds = await db.Polls
                .Where(p => EF.Property<string>(p, "SessionId") == sessionId)
                .Select(p => p.Id)
                .ToListAsync();

            var questionIds = await db.Questions
                .Where(q => EF.Property<string>(q, "SessionId") == sessionId)
                .Select(q => q.Id)
                .ToListAsync();

            // answers → questions
            await db.QuestionAnswers
                .Where(a => questionIds.Contains(EF.Property<string>(a, "QuestionId")))
                .ExecuteDeleteAsync();
            await db.Questions
                .Where(q => EF.Property<string>(q, "SessionId") == sessionId)
                .ExecuteDeleteAsync();

            // responses → polls
            await db.PollResponses
                .Where(r => pollIds.Contains(r.PollId))
                .ExecuteDeleteAsync();
            await db.Polls
                .Where(p => EF.Property<string>(p, "SessionId") == sessionId)
                .ExecuteDeleteAsync();

            // insights, slides
            await db.Insights
                .Where(i => EF.Property<string>(i, "SessionId") == sessionId)
                .ExecuteDeleteAsync();
            await db.Slides
                .Where(s => EF.Property<string>(s, "SessionId") == sessionId)
                .ExecuteDeleteAsync();

            // session + topics (topics cascade via FK)
            await db.Sessions
                .Where(s => s.Id == sessionId)
                .ExecuteDeleteAsync();

            logger.LogInformation("Deleted session {SessionId} and all associated data", sessionId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete session {SessionId}", sessionId);
            throw;
        }
    }
}
