using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Security.Infrastructure
{
    public static class AppClaimTypes
    {
        public const string Department = "http://schemas.fhrwebservices.com/ws/2015/01/identity/claims/department";
        public const string Permission = "http://schemas.fhrwebservices.com/ws/2015/01/identity/claims/permission";
    }

    public static class AppPermissions
    {
        public const string CanCreatePerson = "CanCreatePerson";
        public const string CanUpdatePerson = "CanUpdatePerson";
        public const string CanDeletePerson = "CanDeletePerson";
        public const string CanReadPersonAge = "CanReadPersonAge";
    }
}