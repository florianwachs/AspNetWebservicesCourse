using ChuckNorrisService.DataAccess;
using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using ChuckNorrisService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChuckNorrisService
{

    public static class DIRegistration
    {
        public static IServiceCollection AddJokesServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IJokeRepository, EFJokeRepository>();
            // services.AddDbContext<JokeDbContext>(options => options.UseInMemoryDatabase("JokesDb"));

            var connectionString = configuration.GetConnectionString("Default");
            services.AddDbContext<JokeDbContext>(options => options.UseSqlServer(connectionString));
            return services;
        }
    }
}