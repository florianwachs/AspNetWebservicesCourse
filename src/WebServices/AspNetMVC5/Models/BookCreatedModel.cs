using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetMVC5.Models
{
    public class BookCreatedModel
    {
        public BookModel Book { get; set; }
        public DateTime Created { get; set; }

        public BookCreatedModel(BookModel book)
        {
            Book = book;
            Created = DateTime.Now;
        }
    }
}