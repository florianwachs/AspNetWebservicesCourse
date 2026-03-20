using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechConf.Api.Models;

namespace TechConf.Api.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        // TODO: Task 1 - Configure Event entity
        // - HasKey for Id
        // - Name is Required, MaxLength 200
        // - City is Required, MaxLength 100
        // - Description MaxLength 2000
        // - HasMany Sessions with cascade delete
    }
}
