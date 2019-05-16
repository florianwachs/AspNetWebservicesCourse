using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Logging
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Konfiguriert das Logging auf die Console, damit es sichtbar ist,
            // muss die Anwendung auch als Konsolenanwendung gestartet werden
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            // Fügt einen LoggingProvider für die Visual Studio Debug-Console ein
            loggerFactory.AddDebug();

            // Ein Logger kann manuell mit CreateLogger erzeugt werden
            // Der TypParameter definiert die Category unter der geloggt wird
            //var startupLogger = loggerFactory.CreateLogger("AspNetCore.Logging.Startup");
            //var startupLogger = loggerFactory.CreateLogger(typeof(Startup));
            var startupLogger = loggerFactory.CreateLogger<Startup>();

            startupLogger.LogInformation("configuration phase");

            app.UseMvc();

            startupLogger.LogInformation("request pipeline successfully configured at {TimeStamp}", DateTime.Now);
        }
    }
}
