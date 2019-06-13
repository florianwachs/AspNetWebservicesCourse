using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreSecurity.Infrastructure.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSecurity.Api
{
    public static class AuthConfig
    {
        public static void ConfigureAuth(this IServiceCollection services)
        {
            // 1
            services.AddAuthorization();

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

        public static void UseAuth(this IApplicationBuilder app)
        {
            // 4
            app.UseCors("default");

            // 5
            app.UseAuthentication();
        }
    }
}
