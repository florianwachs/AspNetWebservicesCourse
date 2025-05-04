namespace PoliciesWithSimpleToken.Auth;

public class AuthConstants
{
    public static class Policies
    {
        public const string Admin = "Admin";
        public const string AllowedToReadChuckNorrisBooks = "AllowedToReadChuckNorrisBooks";
        public const string CanDeleteAuthor = "CanDeleteAuthor";
    }

    public static class ClaimTypes
    {
        public const string IsAdmin = "IsAdmin";
        public const string Age = "Age";
        public const string IsContentManager = "IsContentManager";
    }
}