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
        public static IServiceCollection AddBooksServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<IAuthorRepository, AuthorRepository>();
            services.AddDbContext<BookDbContext>(options => options.UseInMemoryDatabase("BooksDb"));
            return services;
        }
    }
}