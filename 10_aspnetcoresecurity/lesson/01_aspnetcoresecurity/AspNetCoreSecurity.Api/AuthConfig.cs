using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreSecurity.Domain.Domain;
using AspNetCoreSecurity.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSecurity.Api
{
    public static class AuthConfig
    {
        public static void ConfigureAuth(this IServiceCollection services)
        {
            // 1
            ConfigureAuthorization(services);
            AddAuthorizationHandler(services);

            // 2
            services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
            {
                // Url des Identity Servers
                options.Authority = "https://localhost:44318";
                options.Audience = "api";
            });

            // 3
            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddRequireClaimAttributeAuthorization();
        }

        private static void AddAuthorizationHandler(IServiceCollection services)
        {
            // Authorization-Handler müssen am DI-System registriert werden.
            services.AddSingleton<IAuthorizationHandler, IsPrincipalHandler>();
            services.AddSingleton<IAuthorizationHandler, IsProfessorHandler>();
            services.AddSingleton<IAuthorizationHandler, IsUniversityMemberHandler>();
            services.AddSingleton<IAuthorizationHandler, CanReadCourseGradesHandler>();
        }

        private static void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AppPolicies.CanReadAllStudents, policy =>
                {
                    policy.RequireClaim(AuthConstants.PrincipalType);
                });

                options.AddPolicy(AppPolicies.CanCreateNewStudent, policy =>
                {
                    policy.RequireClaim(AuthConstants.PrincipalType);
                });

                options.AddPolicy(AppPolicies.CanEditCourse, policy =>
                {
                    policy.RequireAssertion(authContext => authContext.User.IsPrincipal()
                    || authContext.User.IsProfessor());
                });

                options.AddPolicy(AppPolicies.CanDeleteCourse, policy =>
                {
                    policy.AddRequirements(new ProfessorOrPrincipalRequirement());
                });

                options.AddPolicy(AppPolicies.CanReadStudentsEnrolledInCourse, policy =>
                {
                    policy.AddRequirements(new ProfessorOrPrincipalRequirement());
                });

                options.AddPolicy(AppPolicies.CanReadCourses, policy =>
                {
                    policy.AddRequirements(new UniversityMemberRequirement());
                });

                options.AddPolicy(AppPolicies.CanReadCourseGrades, policy => policy.AddRequirements(new CanReadCourseGradesRequirement()));

            });
        }

        public static void UseAuth(this IApplicationBuilder app)
        {
            // 4
            app.UseCors("default");

            // 5
            app.UseAuthentication();
        }

        public class ProfessorOrPrincipalRequirement : IAuthorizationRequirement
        {
        }

        public class IsProfessorHandler : AuthorizationHandler<ProfessorOrPrincipalRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfessorOrPrincipalRequirement requirement)
            {
                if (context.User.IsProfessor())
                {
                    // Wenn ein Authorization Handler pro Requirement Succeeded, ist das Requirement erfüllt.
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }
        }

        public class IsPrincipalHandler : AuthorizationHandler<ProfessorOrPrincipalRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfessorOrPrincipalRequirement requirement)
            {
                if (context.User.IsPrincipal())
                {
                    // Wenn ein Authorization Handler pro Requirement Succeeded, ist das Requirement erfüllt.
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }
        }

        public class UniversityMemberRequirement : IAuthorizationRequirement
        {
        }

        public class IsUniversityMemberHandler : AuthorizationHandler<UniversityMemberRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UniversityMemberRequirement requirement)
            {
                var user = context.User;
                if (user.IsPrincipal() || user.IsProfessor() || user.IsStudent())
                {
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }
        }

        // Requirements können sich auch auf Ressourcen beziehen:
        public class CanReadCourseGradesRequirement : IAuthorizationRequirement
        {
        }

        public class CanReadCourseGradesHandler : AuthorizationHandler<CanReadCourseGradesRequirement, Course>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanReadCourseGradesRequirement requirement, Course resource)
            {
                if (context.User.IsPrincipal())
                {
                    context.Succeed(requirement);
                }

                var userid = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrWhiteSpace(userid) && userid == resource.ProfessorId)
                {
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }
        }
    }
}
