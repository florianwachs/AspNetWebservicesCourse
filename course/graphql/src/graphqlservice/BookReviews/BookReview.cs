using System;

namespace graphqlservice.BookReviews
{
    public class BookReview
    {
        public string Id { get; set; }
        public string BookId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}