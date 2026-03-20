using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechConf.Api.Models;

namespace TechConf.Api.Data.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Title).IsRequired().HasMaxLength(300);
        builder.HasMany(s => s.Speakers)
            .WithMany(sp => sp.Sessions);
    }
}
