using ChuckNorrisService.DataAccess;
using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChuckNorrisService
{

    public static class DIRegistration
    {
        public static IServiceCollection AddJokesServices(this IServiceCollection services)
        {
            services.AddSingleton<IJokeRepository, InMemoryJokeRepository>();
            services.AddDbContext<JokeDbContext>(options => options.UseInMemoryDatabase("JokesDb"));
            return services;
        }
    }
}