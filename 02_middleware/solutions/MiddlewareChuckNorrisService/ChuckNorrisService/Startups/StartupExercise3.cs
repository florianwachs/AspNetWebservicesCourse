using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChuckNorrisService.Startups
{
    public class StartupExercise3
    {
        public void Configure(IApplicationBuilder app)
        {
            FileSystemJokeProvider jokeProvider = new FileSystemJokeProvider();

            app.Use(async (context, next) =>
            {
                // Wir führen den Request aus
                await next();

                // und verzögern die Antwort
                await Task.Delay(TimeSpan.FromSeconds(2));
            });

            app.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(await GetSerializedJoke(jokeProvider), Encoding.UTF8);
            });
        }

        private async Task<string> GetSerializedJoke(IJokeProvider provider)
        {
            return JsonSerializer.Serialize(await provider.GetRandomJokeAsync());
        }
    }
}
