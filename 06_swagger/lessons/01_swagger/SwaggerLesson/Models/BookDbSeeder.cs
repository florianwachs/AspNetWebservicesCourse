using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SwaggerLesson.DataAccess;

namespace SwaggerLesson.Models
{
    public static class BookDbSeeder
    {
        private static readonly Random rnd = new Random();
        private static readonly string BookFilePath = Path.Combine("Data", "books.json");

        private static readonly List<Author> DummyAuthors = new List<Author>
        {
            Author.NewFrom("Chuck", "Norris"),
            Author.NewFrom("Jason", "Bourne"),
            Author.NewFrom("Jean Claude", "Van Damme"),
            Author.NewFrom("Arnold", "Schwarzenegger")
        };

        public static async Task Seed(BookDbContext dbContext)
        {
            if (await dbContext.Books.AnyAsync())
                return;

            var books = GetBooksFromDtos();
            await dbContext.Books.AddRangeAsync(books);
            await dbContext.SaveChangesAsync();
        }

        private static List<Book> GetBooksFromDtos()
        {
            var rawJson = File.ReadAllText(BookFilePath);
            var bookDtos = JsonConvert.DeserializeObject<List<BookDto>>(rawJson);
            var books = GetBooksFromDtos(bookDtos);
            return books;
        }

        private static List<Book> GetBooksFromDtos(List<BookDto> bookDtos)
        {
            var allCategoryNames = bookDtos.SelectMany(dto => dto.Category).Distinct();
            var categoryMap = allCategoryNames.Select(BookCategory.FromName).ToDictionary(k => k.Name);
            var titleId = 1;
            return bookDtos.Select(dto => new Book
            {
                Id = dto.Id,
                Title = "Book " + titleId++,
                Description = dto.Description,
                Categories = GetMatchingCategories(dto),
                Author = DummyAuthors[rnd.Next(0, DummyAuthors.Count)]
            }).ToList();

            List<BookCategory> GetMatchingCategories(BookDto dto) =>
                dto.Category?.Select(cat => categoryMap[cat]).ToList();
        }
    }
}