using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rating.API.Models;

namespace Rating.API.Infrastructure
{
    public class DbInitializer
    {
        private static Random random = new Random();

        public static void Initialize(RatingContext context, bool resetDb)
        {
            if (resetDb)
                context.Database.EnsureDeleted();

            context.Database.Migrate();

            if (context.BookRatings.Any())
                return;

            AddRatings(context);
        }

        private static void AddRatings(RatingContext context)
        {
            var ratings = Enumerable.Range(0, 100).Select(bookId => new BookRating { BookId = bookId, Rating = random.Next(0, 11) });

            context.BookRatings.AddRange(ratings);
            context.SaveChanges();
        }
    }
}
