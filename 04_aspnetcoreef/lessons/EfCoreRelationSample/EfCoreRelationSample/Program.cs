using EfCoreRelationSample;
using EfCoreRelationSample.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ChuckNorrisService
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            SeedDb(host);
            await host.RunAsync();
        }

        private static void SeedDb(IHost host)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                DbSeeder.SeedDb(dbContext);
            }
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}
