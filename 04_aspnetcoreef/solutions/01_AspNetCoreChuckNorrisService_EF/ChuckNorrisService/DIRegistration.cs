using ChuckNorrisService.DataAccess;
using ChuckNorrisService.Models;
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

            string connectionString = configuration.GetConnectionString("Default");
            services.AddSingleton(new DbConnectionString(connectionString));
            services.AddDbContext<JokeDbContext>(options => options.UseSqlServer(connectionString));
            return services;
        }
    }
}