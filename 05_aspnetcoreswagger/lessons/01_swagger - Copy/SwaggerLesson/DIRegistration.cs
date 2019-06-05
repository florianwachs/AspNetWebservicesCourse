using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwaggerLesson.DataAccess;
using SwaggerLesson.Models;
using SwaggerLesson.Repositories;

namespace SwaggerLesson
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