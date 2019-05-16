using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebAPI.OData.Models;

namespace WebAPI.OData.DataAccess
{
    public class BookDbContext : DbContext
    {
        public BookDbContext() : base("BookDbContext")
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}