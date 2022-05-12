using ChuckNorrisService.DataAccess;
using ChuckNorrisService.Models;
using ChuckNorrisService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChuckNorrisService;

public static class DiRegistration
{
    public static IServiceCollection AddJokesServices(this IServiceCollection services, IConfiguration configuration, bool useInMemory)
    {
        services.AddTransient<IJokeRepository, EfJokeRepository>();

        if (useInMemory)
        {
            services.AddDbContext<JokeDbContext>(options => options.UseInMemoryDatabase("JokesDb"));
        }
        else
        {
            string connectionString = configuration.GetConnectionString("Default");
            services.AddDbContext<JokeDbContext>(options => options.UseSqlServer(connectionString));
        }
        
        return services;
    }

}
