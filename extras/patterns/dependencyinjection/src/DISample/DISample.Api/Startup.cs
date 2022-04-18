using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DISample.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DISample.Api
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
            services.AddControllers();

            // Eigene Services registrieren welche im DI System verfügbar sein sollen

            //       👇 Wenn eine Komponente nach ITimeService verlangt, wird ein Singelton von DefaultTimeService zurückgegeben
            services.AddSingleton<ITimeService, DefaultTimeService>();

            //       👇 Während der Laufzeit eines Requests wird immer das gleiche IBookRepository an die Komponenten ausgegeben
            services.AddScoped<IBookRepository, DummyBookRepository>();

            //       👇 Jedesmal wenn eine Komponente ein IBookRepository anfrägt, erhält es eine neue Instanz
            services.AddTransient<IBookRepository, DummyBookRepository>();
        }

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
