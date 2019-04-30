using  AspNetCore.EFRepository.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.EFRepository
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
            modelBuilder.Entity<Book>().Property(b => b.ReleaseDate).HasColumnType("datetime2");


            modelBuilder.Entity<BookAuthorRel>()
            .HasKey(t => new { t.BookId, t.AuthorId });

            modelBuilder.Entity<BookAuthorRel>()
                .HasOne(pt => pt.Book)
                .WithMany(p => p.AuthorRelations)
                .HasForeignKey(pt => pt.BookId);

            modelBuilder.Entity<BookAuthorRel>()
                .HasOne(pt => pt.Author)
                .WithMany(t => t.BookRelations)
                .HasForeignKey(pt => pt.AuthorId);
        }
    }
}
