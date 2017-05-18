using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using  AspNetCore.EFRepository.Models;

namespace AspNetCore.EFRepository.Infrastructure
{
    public static class DbInitializer
    {
        private static Random random = new Random();

        public static void Initialize(BookDbContext context, bool resetDb)
        {
            if (resetDb)
            {
                context.Database.EnsureDeleted();
            }

            context.Database.Migrate();

            if (context.Books.Any())
            {
                return;
            }

            AddBooks(context);
            AddAuthors(context);
            AddContactInfos(context);
            AddBookAuthorRelationship(context);
        }

        private static void AddContactInfos(BookDbContext context)
        {
            var authors = context.Authors.Include(a => a.ContactInfos).ToArray();

            foreach (var author in authors)
            {
                author.ContactInfos.Add(new ContactInfo { Type = ContactInfoTypes.Mail, Value = "test@test.de" });
            }

            context.SaveChanges();
        }

        private static void AddBooks(BookDbContext context)
        {
            var books = new[]
            {
                new Book("1430242337", "C# Pro", 30, DateTime.Now.AddYears(-2)),
                new Book("161729134X", "C# in Depth", 40, DateTime.Now.AddMonths(-2)),
                new Book("1449320104", "C# in a Nutshell", 40, DateTime.Now.AddMonths(-2)),
                new Book("0596807260", "Entity Framework 6", 20, DateTime.Now.AddMonths(-2)),
            };

            context.Books.AddRange(books);
            context.SaveChanges();
        }

        private static void AddAuthors(BookDbContext context)
        {
            var authors = new[]
            {
                new Author{ Id=1, FirstName="Chuck", LastName="Norris", Age=28},
                new Author{ Id=2, FirstName="John", LastName="Skeet", Age=40},
                new Author{ Id=3, FirstName="Julie", LastName="Lerman", Age=21},
            };

            context.Authors.AddRange(authors);
            context.SaveChanges();
        }

        private static void AddBookAuthorRelationship(BookDbContext context)
        {
            var authors = context.Authors.ToArray();
            var books = context.Books.Include(b => b.AuthorRelations).ToArray();
            foreach (var book in books)
            {
                book.AuthorRelations.Add(new BookAuthorRel { AuthorId = PickRandom(authors).Id });
            }
            context.SaveChanges();
        }

        private static Author PickRandom(Author[] authors)
        {
            return authors[random.Next(0, authors.Length)];
        }
    }
}
