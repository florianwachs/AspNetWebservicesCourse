using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactAppWithAuth1.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Custom Eigenschaften an dem User-Objekt
        public bool IsAdmin { get; set; }
        public bool IsPayingCustomer { get; set; }
    }
}
