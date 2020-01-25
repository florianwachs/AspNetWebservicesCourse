using ChuckNorrisService.Startups;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ChuckNorrisService
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // var builder = CreateWebHostBuilder<StartupExercise1_and_2>(args);
            // var builder = CreateWebHostBuilder<StartupExcercise3>(args);
            var builder = CreateWebHostBuilder<StartupExercise4>(args);
            builder.Build().Run();
        }

        private static IHostBuilder CreateWebHostBuilder<TStartup>(string[] args) where TStartup : class
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<TStartup>());
        }
    }
}
