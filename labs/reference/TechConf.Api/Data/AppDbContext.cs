using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechConf.Api.Models;

namespace TechConf.Api.Data;

public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ConferenceEvent> ConferenceEvents => Set<ConferenceEvent>();
    public DbSet<SpeakerProfile> SpeakerProfiles => Set<SpeakerProfile>();
    public DbSet<Proposal> Proposals => Set<Proposal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ConferenceEvent>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Slug).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.City).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Location).HasMaxLength(200).IsRequired();
            entity.HasIndex(x => x.Slug).IsUnique();
        });

        modelBuilder.Entity<SpeakerProfile>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).HasMaxLength(450).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Company).HasMaxLength(120).IsRequired();
            entity.Property(x => x.City).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Tagline).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Bio).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.WebsiteUrl).HasMaxLength(400);
            entity.Property(x => x.PhotoUrl).HasMaxLength(400);
            entity.HasIndex(x => x.UserId).IsUnique();
        });

        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Abstract).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.Track).HasMaxLength(80).IsRequired();
            entity.Property(x => x.DecisionNote).HasMaxLength(1000);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.CreatedAtUtc);

            entity.HasOne(x => x.SpeakerProfile)
                .WithMany(x => x.Proposals)
                .HasForeignKey(x => x.SpeakerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ConferenceEvent)
                .WithMany(x => x.Proposals)
                .HasForeignKey(x => x.ConferenceEventId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
