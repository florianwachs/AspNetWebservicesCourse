using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
    public class StartupExercise1_and_2
    {
        public void Configure(IApplicationBuilder app)
        {
            var jokeProvider = new FileSystemJokeProvider();
            app.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(await GetSerializedJoke(jokeProvider), Encoding.UTF8);
            });
        }

        private async Task<string> GetSerializedJoke(IJokeProvider provider)
        {
            return JsonConvert.SerializeObject(await provider.GetRandomJokeAsync());
        }
    }
}
