using AspNetCoreSwagger.DataAccess;
using AspNetCoreSwagger.Models;
using AspNetCoreSwagger.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSwagger
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