using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AspNetMVC5.Models
{
    public class BookModel
    {
        [Required]
        public string Isbn { get; set; }
        [Required]
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}