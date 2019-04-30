using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.API.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Isbn { get; set; }
        [Required]
        public string Title { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public Book()
        {
        }

        public Book(string isbn, string title, DateTime? releaseDate = null)
        {
            Isbn = isbn;
            Title = title;
            ReleaseDate = releaseDate;
        }
    }
}
