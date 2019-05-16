using AdoNET.EFCodeFirst.Configurations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNET.EFCodeFirst.Domain
{
    public class UniversityContext : DbContext
    {
        // Es kann auch direkt ein ConnectionString angegeben werden,
        // besser ist jedoch fast immer die Configuration in .config
        public UniversityContext()
            : base("UniversityContext")
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
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            // Das Mapping von CLR-Typ auf SQL-Column-Typ kann über Attribute in 
            // den Entitäten gesteuert werden, über Entitätkonfigurationen,
            // oder global
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Configurations.Add(new StudentConfiguration());
        }
    }
}
