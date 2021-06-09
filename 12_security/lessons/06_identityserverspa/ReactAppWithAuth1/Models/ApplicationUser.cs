using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactAppWithAuth1.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Custom Eigenschaft an dem User-Objekt
        public bool IsAdmin { get; set; }
    }
}
