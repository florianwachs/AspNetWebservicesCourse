using BackendApi.DataAccess;
using BackendApi.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await SeedDb(host);
            await host.RunAsync();
        }

        private static async Task SeedDb(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<JokeDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
            await dbContext.Database.MigrateAsync();

            if (dbContext.Jokes.Any())
            {
                return;
            }

            var joke = new Joke
            {
                Text = "Haha",
                Author = new JokeAuthor
                {
                    Name = "Name of the Author"
                }
            };

            dbContext.Jokes.Add(joke);
            await dbContext.SaveChangesAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
