using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSample1.Domain.Models
{
    public enum ContactInfoTypes
    {
        Email,
        Phone,
        Postal,
    }
    public class ContactInfo
    {
        public int Id { get; set; }
        public int Description { get; set; }
        public ContactInfoTypes Type { get; set; }
    }
}
