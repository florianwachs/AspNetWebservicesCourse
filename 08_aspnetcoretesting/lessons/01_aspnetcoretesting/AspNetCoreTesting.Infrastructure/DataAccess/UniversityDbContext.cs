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
        public DbSet<CourseRequest> CourseRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new StudentConfiguration())
                .ApplyConfiguration(new ProfessorConfiguration())
                .ApplyConfiguration(new CourseConfiguration())
                .ApplyConfiguration(new CourseGradeConfiguration())
                .ApplyConfiguration(new StudentCourseConfiguration())
                .ApplyConfiguration(new CourseRequestConfiguration());
        }
    }

    #region Entity Configurations
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(s => s.Id).ValueGeneratedNever();
            builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }

    public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
    {
        public void Configure(EntityTypeBuilder<Professor> builder)
        {
            builder.Property(p => p.Id).ValueGeneratedNever();
            builder.HasMany(p => p.AssignedCourses).WithOne(c => c.Professor);
            builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }

    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.HasOne(c => c.Professor);
            builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }

    public class CourseGradeConfiguration : IEntityTypeConfiguration<CourseGrade>
    {
        public void Configure(EntityTypeBuilder<CourseGrade> builder)
        {
            builder.HasKey(cg => new { cg.CourseId, cg.StudentId });
            builder.HasOne(cg => cg.Student).WithMany(s => s.Grades).HasForeignKey(sc => sc.StudentId);
            builder.HasOne(cg => cg.Course).WithMany(c => c.Grades).HasForeignKey(sc => sc.CourseId);
            builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }

    public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
    {
        public void Configure(EntityTypeBuilder<StudentCourse> builder)
        {
            builder.HasKey(sc => new { sc.CourseId, sc.StudentId });
            builder.HasOne(sc => sc.Student).WithMany(s => s.EnrolledCourses).HasForeignKey(sc => sc.StudentId);
            builder.HasOne(sc => sc.Course).WithMany(c => c.Students).HasForeignKey(sc => sc.CourseId);
            builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }

    public class CourseRequestConfiguration : IEntityTypeConfiguration<CourseRequest>
    {
        public void Configure(EntityTypeBuilder<CourseRequest> builder)
        {
            builder.HasKey(sc => new { sc.RequestedCourseId, sc.StudentId });
            builder.HasOne(sc => sc.Student).WithMany(s => s.CoursesRequests).HasForeignKey(sc => sc.StudentId);
            builder.Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }

    #endregion
}
