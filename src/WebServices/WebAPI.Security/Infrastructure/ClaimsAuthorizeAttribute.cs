using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace WebAPI.Security.Infrastructure
{
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private string issuer;
        public string Issuer
        {
            get
            {
                return issuer ?? ClaimsIdentity.DefaultIssuer;
            }
            set
            {
                issuer = value;
            }
        }

        private string claimType;
        public string ClaimType
        {
            get
            {
                return claimType ?? AppClaimTypes.Permission;
            }
            set
            {
                claimType = value;
            }
        }
        public string Value { get; set; }

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.RequestContext.Principal == null
                || !actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                return false;
            }

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
            if (principal == null)
            {
                return false;
            }

            var authorized = principal
                .HasClaim(claim =>
                    claim.Issuer == Issuer
                    && claim.Type == ClaimType
                    && claim.Value == Value);
            return authorized;
        }
    }
}