using EFCoreSample1.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            //modelBuilder.Entity<Book>().Property(b => b.ReleaseDate).HasColumnType("datetime2");

            // m:n Relationen können aktuell von EF nicht automatisch erkannt werden
            // Daher muss die Beziehung manuell definiert werden und eine Zwischentabelle für das Mapping
            // angelegt werden
            modelBuilder.Entity<BookAuthorRel>()
                .HasKey(t => new { t.BookId, t.AuthorId }); // 👈 Definition eines zusammengesetzten Schlüssels (Composite-Key)

            modelBuilder.Entity<BookAuthorRel>()
                .HasOne(pt => pt.Book)
                .WithMany(p => p.Authors)
                .HasForeignKey(pt => pt.BookId);

            modelBuilder.Entity<BookAuthorRel>()
                .HasOne(pt => pt.Author)
                .WithMany(t => t.Books)
                .HasForeignKey(pt => pt.AuthorId);

            //👇 die Konfiguration kann auch in eigene Klassen ausgelagert werden
            modelBuilder.ApplyConfiguration(new BookConfiguration());

        }
    }

    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(b => b.Isbn).IsRequired();
            builder.Property(b => b.Title).IsRequired().HasMaxLength(500);
        }
    }

}
