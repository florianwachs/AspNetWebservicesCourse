using System.Security.Principal;

namespace WCF.Security
{
    public class CustomPrincipal : IPrincipal
    {
        public CustomPrincipal(IIdentity identity)
        {
            Identity = identity;
        }

        public IIdentity Identity
        {
            get;
            private set;
        }

        public bool IsInRole(string role)
        {
            return Identity.Name == "Chuck";
        }
    }
}