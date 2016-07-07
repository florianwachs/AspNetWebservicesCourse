using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebAPI.OData.Models
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

        public virtual ICollection<Author> Authors { get; set; }
    }
}