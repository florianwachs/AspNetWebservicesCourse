using AspNetMediatR.Api.ApplicationServices;
using AspNetMediatR.Api.DataAccess;
using AspNetMediatR.Api.Domain.Jokes;
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
