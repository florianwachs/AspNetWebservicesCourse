using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AspNetSignalR.Models
{
    public class Book
    {
        public Book()
        {
        }

        public Book(int id, string isbn, string title, decimal price, string[] authors = null, DateTime? releaseDate = null)
        {
            this.Id = id;
            this.Isbn = isbn;
            this.Title = title;
            this.Price = price;
            this.Authors = authors;
            this.ReleaseDate = releaseDate;
        }

        public int Id { get; set; }
        [Required]
        public string Isbn { get; set; }
        [Required]
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string[] Authors { get; set; }
        public DateTime? ReleaseDate { get; set; }

    }
}