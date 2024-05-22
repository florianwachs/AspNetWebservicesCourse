using Microsoft.AspNetCore.Identity;

namespace SimpleTokenAuth.Domain;

public class AppUser : IdentityUser
{
    // Wir brauchen keine weiteren Felder, außer wir möchten zusätzliche Daten speichern
}