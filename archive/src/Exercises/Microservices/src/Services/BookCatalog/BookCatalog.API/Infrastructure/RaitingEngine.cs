using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.API.Infrastructure
{
    public class RatingEngine
    {
        private static readonly Random random = new Random();
        public double GetRatingForBook(int bookId)
        {
            return random.Next(0, 11);
        }
    }
}
