namespace WebAPI.OData.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WebAPI.OData.DataAccess.BookDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebAPI.OData.DataAccess.BookDbContext context)
        {
            if (context.Authors.Any())
                return;

            var authors = new[]
            {
                new Author { FirstName="Chuck", LastName="Norris", Age=18 },
                new Author { FirstName="Jason", LastName="Bourne", Age=30 },
                new Author { FirstName="Steven", LastName="Segal", Age=110 },
                new Author { FirstName="Jean Claude", LastName="van Damme", Age=91 }
            };

            context.Authors.AddRange(authors);
            context.SaveChanges();

            var books = new[]
            {
                new Book { Isbn = Guid.NewGuid().ToString(), Price =99.99m, Title="Web Services done right C#", ReleaseDate=DateTime.Today.AddYears(1), TopSecret="TopSecret", Authors = new List<Author> { authors[0] } },
                new Book { Isbn = Guid.NewGuid().ToString(), Price =19.95m, Title="Roundhouse Kick C#", ReleaseDate=DateTime.Today.AddYears(-11), TopSecret="TopSecret", Authors = new List<Author> { authors[0] } },
                new Book { Isbn = Guid.NewGuid().ToString(), Price =20m, Title="Asp.Net Core", ReleaseDate=DateTime.Today.AddMonths(1), TopSecret="TopSecret", Authors = new List<Author> { authors[1], authors[2] } },
                new Book { Isbn = Guid.NewGuid().ToString(), Price =45.95m, Title="Expert F#", ReleaseDate=DateTime.Today.AddYears(-2), TopSecret="TopSecret", Authors = new List<Author> { authors[3] } },
                new Book { Isbn = Guid.NewGuid().ToString(), Price =0.99m, Title="The Expendable Web Services", ReleaseDate=DateTime.Today.AddYears(-3), TopSecret="TopSecret", Authors = new List<Author> { authors[2], authors[3] } },
                new Book { Isbn = Guid.NewGuid().ToString(), Price =2.99m, Title="Mein Reisetagebuch", ReleaseDate=DateTime.Today.AddYears(1), TopSecret="TopSecret", Authors = new List<Author> { authors[1] } },
            };

            context.Books.AddRange(books);
            context.SaveChanges();
        }

    }
}
