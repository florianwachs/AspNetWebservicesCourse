using EFCoreSample1.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSample1.DataAccess
{
    public class BookDbContext : DbContext
    {
        // Die Options enthalten Informationen für die DB-Connection mit der das EF-Framework
        // auf die DB zugreifen soll
        public BookDbContext(DbContextOptions<BookDbContext> options)
            : base(options)
        {
        }

        // Die Entitäten die direkt abgefragt werden können sollen,
        // werden über DbSets angegeben. Es müssen nicht alle Entitäten
        // angegeben werden
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        // Hier können noch Konfigurationen an Entitäten
        // und Conventions durchgeführt werden, bevor
        // das Modell benutzbar ist
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //                                                        👇 EF versucht automatisch den passenden Datentypen für die Tabellenspalte zu erkennen, dies kann hier festgelegt werden.
            modelBuilder.Entity<Book>().Property(b => b.ReleaseDate).HasColumnType("datetime2");
        }
    }
}
