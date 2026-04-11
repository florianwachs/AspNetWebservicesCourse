using Microsoft.EntityFrameworkCore;

namespace AspireOpenTelemetry.Server.Tariffs;

public sealed class TariffDbContext(DbContextOptions<TariffDbContext> options) : DbContext(options)
{
    public DbSet<TariffBatchEntity> TariffBatches => Set<TariffBatchEntity>();

    public DbSet<TariffQuoteEntity> TariffQuotes => Set<TariffQuoteEntity>();

    public DbSet<AuditRunEntity> AuditRuns => Set<AuditRunEntity>();

    public DbSet<TradeDisputeEntity> TradeDisputes => Set<TradeDisputeEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TariffBatchEntity>(entity =>
        {
            entity.ToTable("tariff_batches");
            entity.HasKey(batch => batch.Id);
            entity.Property(batch => batch.Scenario).HasMaxLength(120);
            entity.Property(batch => batch.Headline).HasMaxLength(280);
            entity.HasIndex(batch => batch.GeneratedAt);
            entity.HasMany(batch => batch.Quotes)
                .WithOne(quote => quote.Batch)
                .HasForeignKey(quote => quote.BatchId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(batch => batch.Audits)
                .WithOne(audit => audit.Batch)
                .HasForeignKey(audit => audit.BatchId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(batch => batch.Disputes)
                .WithOne(dispute => dispute.Batch)
                .HasForeignKey(dispute => dispute.BatchId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TariffQuoteEntity>(entity =>
        {
            entity.ToTable("tariff_quotes");
            entity.HasKey(quote => quote.Id);
            entity.Property(quote => quote.CountryCode).HasMaxLength(8);
            entity.Property(quote => quote.CountryName).HasMaxLength(120);
            entity.Property(quote => quote.ExportSpecialty).HasMaxLength(180);
            entity.Property(quote => quote.TariffBand).HasMaxLength(120);
            entity.Property(quote => quote.RiskLevel).HasMaxLength(64);
            entity.Property(quote => quote.Volatility).HasMaxLength(64);
            entity.Property(quote => quote.SillyReason).HasMaxLength(512);
            entity.Property(quote => quote.MinisterNote).HasMaxLength(280);
            entity.HasIndex(quote => new { quote.BatchId, quote.CountryCode }).IsUnique();
        });

        modelBuilder.Entity<AuditRunEntity>(entity =>
        {
            entity.ToTable("audit_runs");
            entity.HasKey(audit => audit.Id);
            entity.Property(audit => audit.FocusCountryCode).HasMaxLength(8);
            entity.Property(audit => audit.FocusCountryName).HasMaxLength(120);
            entity.Property(audit => audit.HighestCountryCode).HasMaxLength(8);
            entity.Property(audit => audit.HighestCountryName).HasMaxLength(120);
            entity.Property(audit => audit.HighestExportSpecialty).HasMaxLength(180);
            entity.Property(audit => audit.HighestTariffBand).HasMaxLength(120);
            entity.Property(audit => audit.HighestRiskLevel).HasMaxLength(64);
            entity.Property(audit => audit.HighestVolatility).HasMaxLength(64);
            entity.Property(audit => audit.HighestSillyReason).HasMaxLength(512);
            entity.Property(audit => audit.HighestMinisterNote).HasMaxLength(280);
            entity.Property(audit => audit.Verdict).HasMaxLength(280);
            entity.Property(audit => audit.DashboardHint).HasMaxLength(280);
            entity.Property(audit => audit.FindingsJson).HasColumnType("jsonb");
            entity.HasIndex(audit => new { audit.BatchId, audit.CreatedAt });
        });

        modelBuilder.Entity<TradeDisputeEntity>(entity =>
        {
            entity.ToTable("trade_disputes");
            entity.HasKey(dispute => dispute.Id);
            entity.Property(dispute => dispute.CountryCode).HasMaxLength(8);
            entity.Property(dispute => dispute.CountryName).HasMaxLength(120);
            entity.Property(dispute => dispute.Complaint).HasMaxLength(280);
            entity.Property(dispute => dispute.Outcome).HasMaxLength(280);
            entity.Property(dispute => dispute.Severity).HasMaxLength(32);
            entity.Property(dispute => dispute.Ruling).HasMaxLength(280);
            entity.Property(dispute => dispute.DashboardHint).HasMaxLength(280);
            entity.HasIndex(dispute => new { dispute.BatchId, dispute.CreatedAt });
        });
    }
}
