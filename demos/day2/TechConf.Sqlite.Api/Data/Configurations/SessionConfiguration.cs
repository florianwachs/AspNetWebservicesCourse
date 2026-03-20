using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechConf.Api.Models;

namespace TechConf.Api.Data.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Abstract).HasMaxLength(2000);
        builder.Property(s => s.DurationMinutes).IsRequired();

        builder.HasMany(s => s.Speakers)
            .WithMany(sp => sp.Sessions)
            .UsingEntity(j => j.ToTable("SessionSpeakers"));
    }
}
