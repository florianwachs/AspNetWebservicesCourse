using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreTesting.Domain.Domain;
using Microsoft.EntityFrameworkCore;

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
            modelBuilder.Entity<Course>().Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
            modelBuilder.Entity<Professor>().Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
            modelBuilder.Entity<Student>().Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
            modelBuilder.Entity<CourseGrade>().Metadata.SetNavigationAccessMode(PropertyAccessMode.Field);
        }
    }
}
