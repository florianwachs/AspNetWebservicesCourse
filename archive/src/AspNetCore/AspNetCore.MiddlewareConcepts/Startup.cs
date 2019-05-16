using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace AspNetCore.MiddlewareConcepts
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            // TimingMiddleware
            //app.Use(async (context, next) =>
            //{
            //    Stopwatch watch = Stopwatch.StartNew();
            //    await next();
            //    watch.Stop();
            //    Console.WriteLine($"Processing Duration: {watch.ElapsedMilliseconds} ms");
            //});

            app.UseTimingMiddleware();

            // JokeMiddleware
            app.Map("/jokes", jokesPipelineBranch =>
            {
                jokesPipelineBranch.Run(async context =>
                {
                    await context.Response.WriteAsync("This is funny....");
                });
            });

            // Short-Circuit Middleware
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }

    public class TimingMiddleware
    {
        private readonly RequestDelegate _next;

        public TimingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Die Invoke-Methode muss vorhanden sein und kann auch DI-Services anfordern,
        // indem man sie in die Parameterliste aufnimmt
        public async Task Invoke(HttpContext context)
        {
            Stopwatch watch = Stopwatch.StartNew();
            await _next(context);
            watch.Stop();
            Console.WriteLine($"Processing Duration: {watch.ElapsedMilliseconds} ms");
        }
    }

    public static class TimingMiddlewareExtensions
    {
        public static IApplicationBuilder UseTimingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TimingMiddleware>();
        }
    }
}
