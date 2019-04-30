using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvancedLanguageFeatures
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ContactData Contact { get; set; }

    }
    public class ContactData
    {
        public List<Address> Addresses { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}
