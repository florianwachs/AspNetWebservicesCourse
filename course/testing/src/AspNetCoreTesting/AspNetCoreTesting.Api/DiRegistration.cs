using AspNetCoreTesting.Api.DataAccess;
using AspNetCoreTesting.Api.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreTesting.Api
{
    public static class DiRegistration
    {
        public static IServiceCollection AddJokesServices(this IServiceCollection services)
        {
            services.AddSingleton<IJokeRepository, InMemoryJokeRepository>();
            services.AddTransient<IValidator<Joke>, JokeValidator>();
            return services;
        }

    }
}
