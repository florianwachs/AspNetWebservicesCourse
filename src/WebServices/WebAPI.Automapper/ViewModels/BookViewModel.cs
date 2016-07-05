using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Automapper.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string FormattedPrice { get; set; }
        public string ReleaseDate { get; set; }
    }
}