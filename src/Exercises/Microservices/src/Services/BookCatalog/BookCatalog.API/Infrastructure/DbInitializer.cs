using BookCatalog.API.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.API.Infrastructure
{
    public static class DbInitializer
    {
        private static Random random = new Random();

        public static void Initialize(BookDbContext context, bool resetDb)
        {
            if (resetDb)
                context.Database.EnsureDeleted();

            context.Database.Migrate();

            if (context.Books.Any())
                return;

            AddBooks(context);
        }

        private static void AddBooks(BookDbContext context)
        {
            var books = new[]
            {
                new Book("1430242337", "C# Pro", DateTime.Now.AddYears(-2)),
                new Book("161729134X", "C# in Depth", DateTime.Now.AddMonths(-2)),
                new Book("1449320104", "C# in a Nutshell", DateTime.Now.AddMonths(-2)),
                new Book("0596807260", "Entity Framework 6", DateTime.Now.AddMonths(-2)),
            };

            context.Books.AddRange(books);
            context.SaveChanges();
        }
    }
}
