using Microsoft.EntityFrameworkCore;
using ConferenceAssistant.Core.Models;
using ConferenceAssistant.Ingestion.Models;

namespace ConferenceAssistant.Web.Data;

public class ConferenceDbContext : DbContext
{
    public ConferenceDbContext(DbContextOptions<ConferenceDbContext> options) : base(options) { }

    public DbSet<ConferenceSession> Sessions => Set<ConferenceSession>();
    public DbSet<SessionTopic> Topics => Set<SessionTopic>();
    public DbSet<Slide> Slides => Set<Slide>();
    public DbSet<Poll> Polls => Set<Poll>();
    public DbSet<PollResponse> PollResponses => Set<PollResponse>();
    public DbSet<AudienceQuestion> Questions => Set<AudienceQuestion>();
    public DbSet<QuestionAnswer> QuestionAnswers => Set<QuestionAnswer>();
    public DbSet<Insight> Insights => Set<Insight>();
    public DbSet<IngestionRecord> IngestionRecords => Set<IngestionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ConferenceSession
        modelBuilder.Entity<ConferenceSession>(e =>
        {
            e.ToTable("sessions");
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.SessionCode).IsUnique();
            e.Property(s => s.SessionCode).HasMaxLength(20);
            e.Property(s => s.HostPin).HasMaxLength(10);
            e.Property(s => s.Title).HasMaxLength(500);
            e.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            e.HasMany(s => s.Topics).WithOne().HasForeignKey("SessionId").OnDelete(DeleteBehavior.Cascade);
            e.Ignore(s => s.ActiveTopicId); // Runtime state only
        });

        // SessionTopic
        modelBuilder.Entity<SessionTopic>(e =>
        {
            e.ToTable("session_topics");
            e.HasKey(t => t.Id);
            e.Property(t => t.Title).HasMaxLength(500);
            e.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            e.Property<string>("SessionId").HasMaxLength(50);
            // Store complex lists as JSON
            e.Property(t => t.TalkingPoints).HasColumnType("jsonb");
            e.Property(t => t.SuggestedPolls).HasColumnType("jsonb");
            e.Ignore(t => t.Slides); // Loaded separately, runtime only
        });

        // Slide
        modelBuilder.Entity<Slide>(e =>
        {
            e.ToTable("slides");
            e.HasKey(s => s.Id);
            e.Property<string>("SessionId").HasMaxLength(50);
            e.Property(s => s.Title).HasMaxLength(500);
            e.Property(s => s.Type).HasConversion<string>().HasMaxLength(20);
            e.Property(s => s.Layout).HasConversion<string>().HasMaxLength(20);
            // Store bullets as JSON array
            e.Property(s => s.Bullets).HasColumnType("jsonb");
        });

        // Poll
        modelBuilder.Entity<Poll>(e =>
        {
            e.ToTable("polls");
            e.HasKey(p => p.Id);
            e.Property<string>("SessionId").HasMaxLength(50);
            e.Property(p => p.Question).HasMaxLength(1000);
            e.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(p => p.Source).HasConversion<string>().HasMaxLength(20);
            e.Property(p => p.Options).HasColumnType("jsonb");
            e.Property(p => p.AllowOther).HasDefaultValue(false);
        });

        // PollResponse
        modelBuilder.Entity<PollResponse>(e =>
        {
            e.ToTable("poll_responses");
            e.HasKey(r => r.Id);
            e.Property(r => r.PollId).HasMaxLength(50);
            e.Property(r => r.SelectedOption).HasMaxLength(500);
            e.Property(r => r.OtherText).HasMaxLength(500);
        });

        // AudienceQuestion
        modelBuilder.Entity<AudienceQuestion>(e =>
        {
            e.ToTable("audience_questions");
            e.HasKey(q => q.Id);
            e.Property<string>("SessionId").HasMaxLength(50);
            e.Property(q => q.IsSafe).HasDefaultValue(true);
            e.Property(q => q.IsApprovedByPresenter).HasDefaultValue(false);
            e.Ignore(q => q.IsVisibleToAttendees);
            e.HasMany(q => q.Answers).WithOne().HasForeignKey("QuestionId").OnDelete(DeleteBehavior.Cascade);
        });

        // QuestionAnswer
        modelBuilder.Entity<QuestionAnswer>(e =>
        {
            e.ToTable("question_answers");
            e.HasKey(a => a.Id);
            e.Property<string>("QuestionId").HasMaxLength(50);
            e.Property(a => a.AuthorLabel).HasMaxLength(100);
        });

        // Insight
        modelBuilder.Entity<Insight>(e =>
        {
            e.ToTable("insights");
            e.HasKey(i => i.Id);
            e.Property<string>("SessionId").HasMaxLength(50);
            e.Property(i => i.Type).HasConversion<string>().HasMaxLength(30);
        });

        // IngestionRecord
        modelBuilder.Entity<IngestionRecord>(e =>
        {
            e.ToTable("ingestion_records");
            e.HasKey(r => r.Id);
            e.HasIndex(r => new { r.DocumentId, r.Source }).IsUnique();
            e.Property(r => r.DocumentId).HasMaxLength(500);
            e.Property(r => r.Source).HasMaxLength(200);
            e.Property(r => r.ContentHash).HasMaxLength(64);
            e.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(r => r.ErrorMessage).HasMaxLength(2000);
        });
    }
}
