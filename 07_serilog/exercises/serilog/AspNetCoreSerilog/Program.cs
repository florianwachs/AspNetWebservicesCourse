using AspNetCoreSerilog.DataAccess;
using AspNetCoreSerilog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace AspNetCoreSerilog
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IHost host = CreateWebHostBuilder(args).Build();
            await SeedDb(host);
            await host.RunAsync();
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
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webHost => webHost.UseStartup<Startup>());
        }
    }
}