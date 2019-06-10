using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreTesting.Domain.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCoreTesting.Infrastructure.DataAccess
{
    public class UniversityDbContext : DbContext
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseGrade> CourseGrades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StudentConfiguration())
                .ApplyConfiguration(new ProfessorConfiguration())
                .ApplyConfiguration(new CourseConfiguration())
                .ApplyConfiguration(new CourseGradeConfiguration());
        }
    }

    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }

    public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
    {
        public void Configure(EntityTypeBuilder<Professor> builder)
        {
            builder.HasMany(p => p.AssignedCourses).WithOne(c => c.Professor);
            builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }

    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasOne(c => c.Professor);
            builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }

    public class CourseGradeConfiguration : IEntityTypeConfiguration<CourseGrade>
    {
        public void Configure(EntityTypeBuilder<CourseGrade> builder)
        {
            builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }

}
