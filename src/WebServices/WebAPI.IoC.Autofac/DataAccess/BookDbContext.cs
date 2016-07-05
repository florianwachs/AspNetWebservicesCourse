using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebAPI.IoC.Autofac.Models;

namespace WebAPI.IoC.Autofac.DataAccess
{
    public class BookDbContext : DbContext
    {
        public BookDbContext() : base("BookDbContext")
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}