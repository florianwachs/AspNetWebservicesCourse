using AspNetCoreSerilog.DataAccess;
using AspNetCoreSerilog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.Threading.Tasks;

namespace AspNetCoreSerilog
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
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                IHost host = CreateWebHostBuilder(args).Build();
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

        private static async Task SeedDb(IHost host)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                JokeDbContext dbContext = scope.ServiceProvider.GetRequiredService<JokeDbContext>();
                await JokeDbSeeder.Seed(dbContext);
            }
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webHost => webHost.UseStartup<Startup>().UseSerilog(
                (hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(new CompactJsonFormatter(), "log.json",
                        shared: true, fileSizeLimitBytes: 10000000,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 1000)
            //                    .WriteTo.RollingFile("log.txt",
            //                        shared: true, fileSizeLimitBytes: 10000000,
            //                        retainedFileCountLimit: 1000)
            ));
        }
    }
}