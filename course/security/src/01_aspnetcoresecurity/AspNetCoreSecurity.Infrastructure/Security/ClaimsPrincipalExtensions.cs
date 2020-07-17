using System.Security.Claims;
using AspNetCoreSecurity.Domain.Domain;

namespace AspNetCoreSecurity.Infrastructure.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsPrincipal(this ClaimsPrincipal principal) => principal.HasClaim(c => c.Type == AuthConstants.PrincipalType);

        public static bool IsProfessor(this ClaimsPrincipal principal) => principal.HasClaim(c => c.Type == AuthConstants.ProfessorType);

        public static bool IsStudent(this ClaimsPrincipal principal) => principal.HasClaim(c => c.Type == AuthConstants.StudentType);
    }
}
