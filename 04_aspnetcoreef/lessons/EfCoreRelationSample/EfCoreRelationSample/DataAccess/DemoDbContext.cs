using EfCoreRelationSample.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreRelationSample.DataAccess
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<List> Lists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ListConfiguration());
            modelBuilder.ApplyConfiguration(new SharedListConfiguration());
        }

        public class UserConfiguration : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.Property(s => s.Id).ValueGeneratedNever();
                builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
            }
        }

        public class ListConfiguration : IEntityTypeConfiguration<List>
        {
            public void Configure(EntityTypeBuilder<List> builder)
            {
                builder.Property(s => s.Id).ValueGeneratedNever();
                builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
            }
        }

        public class SharedListConfiguration : IEntityTypeConfiguration<SharedList>
        {
            public void Configure(EntityTypeBuilder<SharedList> builder)
            {
                builder.HasKey(cg => new { cg.ListId, cg.UserId });
                builder.HasOne(cg => cg.List).WithMany(s => s.SharedWith).HasForeignKey(sc => sc.ListId);
                builder.HasOne(cg => cg.User).WithMany(c => c.SharedLists).HasForeignKey(sc => sc.UserId);
                builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
            }
        }
    }
}
