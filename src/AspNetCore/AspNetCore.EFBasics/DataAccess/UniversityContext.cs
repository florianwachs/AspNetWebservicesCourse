using Microsoft.EntityFrameworkCore;
using NetCore.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.EF
{
    public class UniversityContext : DbContext
    {
        // Die Options enthalten Informationen für die DB-Connection mit der das EF-Framework
        // auf die DB zugreifen soll
        public UniversityContext(DbContextOptions<UniversityContext> options)
            : base(options)
        {
        }

        // Die Entitäten die direkt abgefragt werden können sollen,
        // werden über DbSets angegeben. Es müssen nicht alle Entitäten
        // angegeben werden
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        // Hier können noch Konfigurationen an Entitäten
        // und Conventions durchgeführt werden, bevor
        // das Modell benutzbar ist

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Student>().Property(s => s.Joined).HasColumnType("datetime2");

        }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //    // Das Mapping von CLR-Typ auf SQL-Column-Typ kann über Attribute in 
        //    // den Entitäten gesteuert werden, über Entitätkonfigurationen,
        //    // oder global
        //    modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

        //    modelBuilder.Configurations.Add(new StudentConfiguration());
        //}
    }
}
