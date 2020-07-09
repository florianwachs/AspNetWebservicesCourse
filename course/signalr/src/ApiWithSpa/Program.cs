using AspNetCoreSignalR.ApiWithSpa.Jobs;
using Coravel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreSignalR.ApiWithSpa
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Services.UseScheduler(scheduler =>
            {
                scheduler
                    .Schedule<CheckForWeatherUpdates>()
                    .EveryFifteenSeconds();
            });
            host.Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .UseStartup<Startup>()
                .UseSetting("detailedErrors", "true");
            }).ConfigureServices(services =>
            {
                services.AddScheduler();
                services.AddTransient<CheckForWeatherUpdates>();
            });

        }
    }
}
