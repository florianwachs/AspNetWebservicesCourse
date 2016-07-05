using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;

namespace WCF.Security
{
    public class CustomSecurityService : ICustomSecurityService
    {
        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        public string DoWork()
        {
            return System.Threading.Thread.CurrentPrincipal.Identity.Name +
                ": Chuck Norris protocol design method has no status, requests or responses, only commands.";
        }
    }
}
