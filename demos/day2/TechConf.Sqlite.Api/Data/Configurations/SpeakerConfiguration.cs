using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechConf.Api.Models;

namespace TechConf.Api.Data.Configurations;

public class SpeakerConfiguration : IEntityTypeConfiguration<Speaker>
{
    public void Configure(EntityTypeBuilder<Speaker> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(120);
        builder.Property(s => s.Bio).HasMaxLength(2000);
        builder.Property(s => s.Company).HasMaxLength(120);
    }
}
