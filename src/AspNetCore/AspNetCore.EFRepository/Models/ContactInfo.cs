using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  AspNetCore.EFRepository.Models
{
    public enum ContactInfoTypes
    {
        Telephone,
        Mail,
        Url,
    }

    public class ContactInfo
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public ContactInfoTypes Type { get; set; }
        public string Value { get; set; }
    }
}
