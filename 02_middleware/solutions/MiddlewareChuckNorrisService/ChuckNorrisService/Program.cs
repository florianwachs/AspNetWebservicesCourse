using ChuckNorrisService.Startups;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace ChuckNorrisService
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IWebHostBuilder builder = CreateWebHostBuilder<StartupExercise1>(args);
            builder.Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder<TStartup>(string[] args) where TStartup : class
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<TStartup>();
        }
    }
}
