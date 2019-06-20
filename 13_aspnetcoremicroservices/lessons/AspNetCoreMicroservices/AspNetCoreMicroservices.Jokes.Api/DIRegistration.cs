using AspNetCoreMicroservices.Jokes.Api.DataAccess;
using AspNetCoreMicroservices.Jokes.Api.Models;
using AspNetCoreMicroservices.Jokes.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreMicroservices.Jokes.Api
{
    public static class DiRegistration
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