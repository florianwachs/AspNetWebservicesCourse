using AspNetCoreAutomapper.DataAccess;
using AspNetCoreAutomapper.Models;
using AspNetCoreAutomapper.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreAutomapper
{
    public static class DIRegistration
    {
        public static IServiceCollection AddJokesServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<IJokeRepository, JokeRepository>();
            services.AddTransient<IAuthorRepository, AuthorRepository>();
            services.AddDbContext<JokeDbContext>(options => options.UseInMemoryDatabase("JokesDb"));
            return services;
        }
    }
}