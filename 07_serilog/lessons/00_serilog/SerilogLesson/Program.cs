using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SerilogLesson.DataAccess;
using SerilogLesson.Models;
using System.Threading.Tasks;

namespace SerilogLesson
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
                BookDbContext dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
                await BookDbSeeder.Seed(dbContext);
            }
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webHost => webHost.UseStartup<Startup>());
        }
    }
}