using AspNetMediatR.Api.Domain.Jokes.Models;
using AspNetMediatR.Api.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetMediatR.Api
{
    public static class DIRegistration
    {
        public static IServiceCollection AddJokesServices(this IServiceCollection services)
        {
            services.AddSingleton<IJokeRepository, InMemoryJokeRepository>();
            services.AddTransient<IValidator<Joke>, JokeValidator>();
            return services;
        }
    }
}
