using System;

namespace graphqlservice.BookReviews
{
    public class BookReview
    {
        public string Id { get; set; }
        public string BookId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }

        public NestedData[] NestedData { get; set; }
    }

    public class NestedData
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }
}