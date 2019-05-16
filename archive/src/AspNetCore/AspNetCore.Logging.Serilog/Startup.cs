using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using AspNetCore.Logging.Serilog.Infrastructure;
using AspNetCore.Logging.Serilog.Repositories;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Core;

namespace AspNetCore.Logging.Serilog
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

            // Globalen Serilog-Logger konfigurieren
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration) // Konfiguration kann teilweise oder vollständig aus dem Konfigurationsobjekt stammen
                .WriteTo.RollingFile("Log-{Date}.txt")
                //.WriteTo.RollingFile(new JsonFormatter(null, true), "Log-{Date}.json", shared: true, fileSizeLimitBytes: 10000000, retainedFileCountLimit: 1000)
                //  .WriteTo.Seq("http://localhost:5341")
                .Enrich.WithProperty("Release", "0.0.1-beta-nightmare")
                .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            RegisterMyServices(services);
            services.AddMvc();
        }

        private void RegisterMyServices(IServiceCollection services)
        {
            services.AddSingleton<ITimeService, DefaultTimeService>();
            services.AddScoped<IBookRepository, InMemoryBookRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            loggerFactory.AddSerilog();
            loggerFactory.AddDebug();

            // Sicherstellen das beim Beenden der Anwendung alle Logs an die
            // konfigurierten Sinks weitergegeben wurden
            applicationLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            app.UseMvc();
        }
    }
}
