using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AspNetCore.EFBasics.Models
{
    public class Author
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public ICollection<ContactInfo> ContactInfos { get; set; }
        public ICollection<BookAuthorRel> BookRelations { get; set; }
    }
}