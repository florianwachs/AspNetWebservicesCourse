using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReactAppWithAuth1.Models
{
    public abstract class AppClaims
    {
        protected const string ClaimTypePrefix = "https://throsenheim.de/ws/";


        public static IReadOnlyCollection<Claim> GetUserClaims()
        {
            return new[]
             {
                CanReadWeatherClaim.Create(),
            };
        }

        public static IReadOnlyCollection<Claim> GetAdminClaims()
        {
            return new[]
             {
                CanAddWeatherClaim.Create(),
            };
        }
    }

    public class CanReadWeatherClaim : AppClaims
    {
        public const string Type = ClaimTypePrefix + "CanReadWeather";
        public static Claim Create() => new Claim(Type, true.ToString());
    }

    public class CanAddWeatherClaim : AppClaims
    {
        public const string Type = ClaimTypePrefix + "CanAddWeather";
        public static Claim Create() => new Claim(Type, true.ToString());
    }

    public class IsPremiumUserClaim : AppClaims
    {
        public const string Type = ClaimTypePrefix + "IsPremiumUser";
        public static Claim Create() => new Claim(Type, true.ToString());
    }

    public static class AppPolicies
    {
        public const string CanReadWeather = "CanReadWeather";
        public const string CanAddWeather = "CanAddWeather";
        public const string CanReadTemp = "CanReadTemp";
    }

}
