using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ChuckNorrisService
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            CreateHostBuilder<Startup>(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder<TStartup>(string[] args) where TStartup : class
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<TStartup>());
        }
    }
}
