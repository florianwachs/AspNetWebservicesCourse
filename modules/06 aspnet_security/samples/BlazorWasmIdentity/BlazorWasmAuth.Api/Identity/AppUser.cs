using Microsoft.AspNetCore.Identity;

namespace BlazorWasamAuth.Api.Identity;

public class AppUser : IdentityUser
{
    public bool IsAdmin { get; set; }
    public int Age { get; set; }
}