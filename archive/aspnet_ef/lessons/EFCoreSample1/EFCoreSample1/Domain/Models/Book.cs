using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSample1.Domain.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public DateTime ReleaseDate { get; set; }
        public ICollection<Author> Authors { get; set; }

        // Manuelle m:n Beziehung
        //public ICollection<BookAuthorRel> Authors { get; set; }
    }
}
