using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChuckNorrisService.Startups
{
    public class StartupExercise1_and_2
    {
        public void Configure(IApplicationBuilder app)
        {
            FileSystemJokeProvider jokeProvider = new FileSystemJokeProvider();
            app.Run(async context =>
            {              
                await context.Response.WriteAsJsonAsync(await jokeProvider.GetRandomJokeAsync());
            });
        }
    }
}
