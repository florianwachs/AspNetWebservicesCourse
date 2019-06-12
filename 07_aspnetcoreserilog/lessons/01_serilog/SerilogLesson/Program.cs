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
using Serilog.Formatting.Compact;
using SerilogLesson.DataAccess;
using SerilogLesson.Models;

namespace SerilogLesson
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {            
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
            //return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().UseSerilog();

            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().UseSerilog(
                (hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
//                    .WriteTo.RollingFile(new CompactJsonFormatter(), "log-{Date}.json",
//                        shared: true, fileSizeLimitBytes: 10000000,
//                        retainedFileCountLimit: 1000)
                    .WriteTo.RollingFile("log-{Date}.txt",
                        shared: true, fileSizeLimitBytes: 10000000,
                        retainedFileCountLimit: 1000)
            );
        }
    }
}