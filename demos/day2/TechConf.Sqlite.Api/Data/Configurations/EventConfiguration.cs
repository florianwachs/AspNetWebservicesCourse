using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechConf.Api.Models;

namespace TechConf.Api.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.City).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(2000);

        builder.HasMany(e => e.Sessions)
            .WithOne(s => s.Event)
            .HasForeignKey(s => s.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
