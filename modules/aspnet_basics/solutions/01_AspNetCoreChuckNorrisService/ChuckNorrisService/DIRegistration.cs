using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace ChuckNorrisService
{

    public static class DIRegistration
    {
        public static IServiceCollection AddJokesServices(this IServiceCollection services)
        {
            services.AddSingleton<IJokeProvider, FileSystemJokeProvider>();
            return services;
        }
    }
}