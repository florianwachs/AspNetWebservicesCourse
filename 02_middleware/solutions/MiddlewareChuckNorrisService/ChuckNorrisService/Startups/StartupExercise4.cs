using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChuckNorrisService.Startups
{
    public class StartupExercise4
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Dies fügt die von der Routing-Middleware benötigten
            // Dienste zum Dependency Injection Container hinzu.
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            var jokeProvider = new FileSystemJokeProvider();

            var routes = new RouteBuilder(app);

            routes.MapGet("api/jokes/random", async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(await GetSerializedJoke(jokeProvider), Encoding.UTF8);

            });

            routes.MapGet("{*path}", context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync("Well, IT'S YOUR FAULT!");
            });

            app.UseRouter(routes.Build());
        }

        private async Task<string> GetSerializedJoke(IJokeProvider provider)
        {
            return JsonConvert.SerializeObject(await provider.GetRandomJokeAsync());
        }
    }
}
