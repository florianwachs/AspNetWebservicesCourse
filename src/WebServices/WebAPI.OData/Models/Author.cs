using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.OData.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}