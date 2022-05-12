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
            //                                                        👇 EF versucht automatisch den passenden Datentypen für die Tabellenspalte zu erkennen, dies kann hier festgelegt werden. ACHTUNG: nicht jeder Typ exisitiert in jeder DB-Technologie
            modelBuilder.Entity<Book>().Property(b => b.ReleaseDate).HasColumnType("datetime2");

            // Falls gewünscht kann eine manuelle Mapping-Tabelle angeleget werden
            //modelBuilder.Entity<BookAuthorRel>()
            //    .HasKey(t => new { t.BookId, t.AuthorId }); // 👈 Definition eines zusammengesetzten Schlüssels (Composite-Key)

            //modelBuilder.Entity<BookAuthorRel>()
            //    .HasOne(pt => pt.Book)
            //    .WithMany(p => p.Authors)
            //    .HasForeignKey(pt => pt.BookId);

            //modelBuilder.Entity<BookAuthorRel>()
            //    .HasOne(pt => pt.Author)
            //    .WithMany(t => t.Books)
            //    .HasForeignKey(pt => pt.AuthorId);

            //👇 die Konfiguration kann auch in eigene Klassen ausgelagert werden
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
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

    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasMany(a => a.ContactInfos);
            builder.HasMany(a => a.Books).WithMany(b => b.Authors);
        }
    }

}
