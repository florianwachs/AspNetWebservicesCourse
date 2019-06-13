using AspNetCoreSecurity.Infrastructure.DataAccess;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSecurity.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddFluentValidation();
            services.AddUniversityServices();

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
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UniversityDbContext universityDb)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            DbSeeder.SeedDb(universityDb);

            app.UseHttpsRedirection();

            // 4
            app.UseCors("default");

            // 5
            app.UseAuthentication();

            // 6
            app.UseMvc();
        }
    }
}
