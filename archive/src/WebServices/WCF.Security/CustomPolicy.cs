using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace WCF.Security
{
    public class CustomPolicy : IAuthorizationPolicy
    {
        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            IIdentity identity = ExtractIdentityFrom(evaluationContext);
            evaluationContext.Properties["Principal"] = new CustomPrincipal(identity);
            return true;
        }

        private IIdentity ExtractIdentityFrom(EvaluationContext context)
        {
            object obj;
            if (!context.Properties.TryGetValue("Identities", out obj))
                throw new InvalidOperationException("No Identity found");
            var identities = obj as IList<IIdentity>;
            if (identities == null || identities.Count <= 0)
                throw new InvalidOperationException("No Identity found");
            return identities[0];
        }

        public System.IdentityModel.Claims.ClaimSet Issuer
        {
            get { throw new NotImplementedException(); }
        }

        public string Id
        {
            get { throw new NotImplementedException(); }
        }
    }
}