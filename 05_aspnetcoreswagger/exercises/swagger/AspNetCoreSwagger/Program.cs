using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreSwagger.DataAccess;
using AspNetCoreSwagger.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreSwagger
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