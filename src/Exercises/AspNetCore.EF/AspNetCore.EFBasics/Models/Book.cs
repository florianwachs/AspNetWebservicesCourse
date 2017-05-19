using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace AspNetCore.EFBasics.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Isbn { get; set; }
        [Required]
        public string Title { get; set; }
        public decimal Price { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [IgnoreDataMember]
        public string TopSecret { get; set; }

        public ICollection<BookAuthorRel> AuthorRelations { get; set; }

        public Book()
        {
        }

        public Book(string isbn, string title, decimal price, DateTime? releaseDate = null)
        {
            Isbn = isbn;
            Title = title;
            Price = price;
            ReleaseDate = releaseDate;
        }
    }
}