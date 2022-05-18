using EfCoreCqrs.Domain;
using Microsoft.EntityFrameworkCore;

namespace EfCoreCqrs.DataAccess;

public class BookDbContext : DbContext
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();

    public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
