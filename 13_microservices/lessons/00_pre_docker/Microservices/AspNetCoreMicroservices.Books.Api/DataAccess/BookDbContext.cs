using AspNetCoreMicroservices.Books.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMicroservices.Books.Api.DataAccess
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
