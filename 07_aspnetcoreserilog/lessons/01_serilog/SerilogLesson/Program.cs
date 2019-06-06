using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SerilogLesson.DataAccess;
using SerilogLesson.Models;

namespace SerilogLesson
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Configuration im Code, so ist aber kein Zugriff auf das Configfile appsettings möglich
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                var host = CreateWebHostBuilder(args).Build();
                await SeedDb(host);
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "API Host Crashed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async Task SeedDb(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
                await BookDbSeeder.Seed(dbContext);
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().UseSerilog();
        }
    }
}