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
            //Log.Logger = new LoggerConfiguration()
            //             .Enrich.FromLogContext()
            //             .WriteTo.LiterateConsole()
            //             .WriteTo.RollingFile("myapp-{Date}.txt")
            //             .CreateLogger();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
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
