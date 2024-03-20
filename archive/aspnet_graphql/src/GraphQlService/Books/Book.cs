using System;

namespace graphqlservice.Books
{
    public class Book
    {
        public string Id { get; set; }
        public string Isbn { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal Price { get; set; }
    }
}