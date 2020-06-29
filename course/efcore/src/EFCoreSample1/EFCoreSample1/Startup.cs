using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreSample1.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EFCoreSample1
{
    public enum DbModes
    {
        SqlLiteInMemory,
        SqlServerLocalDb,
        PostgreSql,
    }
    
    public class Startup
    {
        private DbModes DbMode = DbModes.SqlLiteInMemory;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ConfigureEntityFramework(services);
        }

        private void ConfigureEntityFramework(IServiceCollection services)
        {
            switch (DbMode)
            {
                case DbModes.SqlLiteInMemory:
                    UseInMemorySqlLiteDb(services);
                    break;
                case DbModes.SqlServerLocalDb:
                    UseSqlServerLocalDb(services);
                    break;
                case DbModes.PostgreSql:
                    UseDockerPostgreSql(services);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void UseSqlServerLocalDb(IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        private void UseDockerPostgreSql(IServiceCollection services)
        {
            // Achtung: Vorher muss per Docker Compose oder direkt mit Docker ein Docker Container gestartet werden.
            throw new NotImplementedException();
        }
        
        private static void UseInMemorySqlLiteDb(IServiceCollection services)
        {
            // Manuelles erzeugen der SqliteConection damit sie hier geöffnet werden kann
            // Sonst schließt der erste DBContext der Disposed wird die Connection und die
            // Db geht offline
            SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<BookDbContext>(options => { options.UseSqlite(connection); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
