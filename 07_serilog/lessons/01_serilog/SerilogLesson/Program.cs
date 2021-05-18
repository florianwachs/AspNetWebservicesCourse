using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using SerilogLesson.DataAccess;
using SerilogLesson.Models;

namespace SerilogLesson
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341/")
                .CreateBootstrapLogger();

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
            // docker run --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().UseSerilog(
                (hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.Seq("http://localhost:5341/")
                    //                    .WriteTo.File(new CompactJsonFormatter(), "log.json",
                    //                        shared: true, fileSizeLimitBytes: 10000000,
                    //                        retainedFileCountLimit: 1000)
                    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day,
                        shared: true, fileSizeLimitBytes: 10000000,
                        retainedFileCountLimit: 1000)
            );
        }
    }
}