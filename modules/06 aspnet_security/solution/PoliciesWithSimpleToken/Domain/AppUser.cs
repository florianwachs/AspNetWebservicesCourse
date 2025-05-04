using Microsoft.AspNetCore.Identity;

namespace PoliciesWithSimpleToken.Domain;

public class AppUser : IdentityUser
{
    public bool IsAdmin { get; set; }
    public int Age { get; set; }
    public bool IsContentManager { get; set; }
}