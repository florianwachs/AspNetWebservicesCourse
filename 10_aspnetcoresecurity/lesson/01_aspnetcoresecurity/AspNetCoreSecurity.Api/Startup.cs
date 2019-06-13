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
            services.ConfigureAuth();

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

            // 4-5
            app.UseAuth();

            // 6
            app.UseMvc();
        }
    }
}
