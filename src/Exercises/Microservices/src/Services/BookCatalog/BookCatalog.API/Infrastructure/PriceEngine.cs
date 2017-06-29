using BookCatalog.API.Models;
using System;

namespace BookCatalog.API.Infrastructure
{
    public class PriceEngine
    {
        private static Random random = new Random();
        public Price GetPriceForBook(int bookId) => new Price { BookId = bookId, Amount = CalculateReatimePrice(), Currency = "€" };
        private static decimal CalculateReatimePrice() => Math.Round((decimal)(random.Next(10, 50) * random.NextDouble()), 2);
    }
}
