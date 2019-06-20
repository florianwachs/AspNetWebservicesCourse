using AspNetCoreSerilog.DataAccess;
using AspNetCoreSerilog.Models;
using AspNetCoreSerilog.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSerilog
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