using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSecurity.Infrastructure.Security
{
    // Thanks @ https://github.com/chrisfcarroll/RequireClaimAttributeAspNetCore/blob/master/RequireClaimAttribute.cs
    public class RequireClaimAttribute : AuthorizeAttribute
    {
        public static readonly string PolicyName = typeof(RequireClaimAttribute).AssemblyQualifiedName;

        public string Type { get; set; }
        public string Value { get; set; }
        public new string Policy { get; } = PolicyName;

        public RequireClaimAttribute(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException(nameof(type));
            }

            Type = type;
            base.Policy = Policy;
        }
    }

    public class RequireClaimAuthorizationHandler : AuthorizationHandler<RequireClaimAuthorizationHandler>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireClaimAuthorizationHandler requirement)
        {
            if (context.User != null
                && context.Resource is AuthorizationFilterContext mvcContext
                && mvcContext.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                var controllerClaims = actionDescriptor.ControllerTypeInfo.CustomAttributes.Where(cad => cad.AttributeType == typeof(RequireClaimAttribute));
                var actionClaims = actionDescriptor.MethodInfo.CustomAttributes.Where(cad => cad.AttributeType == typeof(RequireClaimAttribute));
                var actualClaims = context.User.Claims;
                var ids = context.User.Identities;
                var satisfiesControllerClaims = controllerClaims.All(c => actualClaims.Any(a => Satisfies(a, c)));
                var satisfiesActionClaims = actionClaims.All(c => actualClaims.Any(a => Satisfies(a, c)));
                if (satisfiesControllerClaims && satisfiesActionClaims)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }

        public static bool EqualsTypeValue(Claim left, Claim right) => left.Type == right.Type && left.Value == right.Value;

        public static bool Satisfies(Claim left, RequireClaimAttribute right) => left.Type == right.Type && left.Value == right.Value;
        public static bool Satisfies(Claim left, CustomAttributeData right)
        {
            if (right.AttributeType != typeof(RequireClaimAttribute))
            {
                return false;
            }

            var type = right.ConstructorArguments.First().Value as string;
            var value = right.NamedArguments.First(a => a.MemberName == "Value").TypedValue.Value as string;
            return left.Type == type && left.Value == value;
        }

    }

    public static class RequireClaimAuthorizationExtensions
    {
        /// <summary>
        /// Enable the use of <see cref="RequireClaimAttribute"/> to declare Claims-based Authorization in Attributes of the Controller and/or Action
        /// </summary>
        /// <param name="services"></param>
        public static void AddRequireClaimAttributeAuthorization(this IServiceCollection services) => services.AddAuthorization(o => { o.AddPolicy(RequireClaimAttribute.PolicyName, p => p.AddRequirements(new RequireClaimAuthorizationHandler())); });


    }
}
