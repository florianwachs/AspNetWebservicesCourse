using EfCoreRelationSample.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EfCoreRelationSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            ConfigureDb(services);
        }      

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DemoDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            DbSeeder.SeedDb(dbContext);

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private static void ConfigureDb(IServiceCollection services)
        {
            // Manuelles erzeugen der SqliteConection damit sie hier geöffnet werden kann
            // Sonst schließt der erste DBContext der Disposed wird die Connection und die
            // Db geht offline
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<DemoDbContext>(options =>
            {
                options.UseSqlite(connection);
            });
        }
    }
}
