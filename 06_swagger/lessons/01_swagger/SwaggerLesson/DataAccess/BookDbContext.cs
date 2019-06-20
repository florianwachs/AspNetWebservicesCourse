using Microsoft.EntityFrameworkCore;
using SwaggerLesson.Models;

namespace SwaggerLesson.DataAccess
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
