using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ChuckNorrisService
{

    public static class DIRegistration
    {
        public static IServiceCollection AddJokesServices(this IServiceCollection services)
        {
            services.AddSingleton<IJokeRepository, InMemoryJokeRepository>();
            return services;
        }
    }
}