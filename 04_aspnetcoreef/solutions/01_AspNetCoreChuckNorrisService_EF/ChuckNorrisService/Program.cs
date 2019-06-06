using ChuckNorrisService.DataAccess;
using ChuckNorrisService.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ChuckNorrisService
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            await SeedDb(host);
            await host.RunAsync();
        }

        private static async Task SeedDb(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<JokeDbContext>();
                await JokeDbSeeder.Seed(dbContext);
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
        }
    }
}
