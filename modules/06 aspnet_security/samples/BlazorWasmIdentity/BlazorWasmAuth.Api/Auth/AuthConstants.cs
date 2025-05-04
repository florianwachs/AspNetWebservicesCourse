namespace BlazorWasamAuth.Api.Auth;

public class AuthConstants
{   
    public static class Policies
    {
        public const string Admin = "Admin";
        public const string AllowedToReadChuckNorrisBooks = "AllowedToReadChuckNorrisBooks";
    }

    public static class ClaimTypes
    {
        public const string IsAdmin = "IsAdmin";
        public const string Age = "Age";
        public const string FavoriteFood = "FavoriteFood";  
    }
}