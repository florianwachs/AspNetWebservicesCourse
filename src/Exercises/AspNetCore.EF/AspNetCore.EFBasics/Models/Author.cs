using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AspNetCore.EFBasics.Models
{
    public class Author
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Range(0, 150)]
        public int Age { get; set; }

        public ICollection<ContactInfo> ContactInfos { get; set; }
        public ICollection<BookAuthorRel> BookRelations { get; set; }
    }
}