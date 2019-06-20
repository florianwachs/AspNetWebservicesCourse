using AspNetCoreMicroservices.Books.Api.DataAccess;
using AspNetCoreMicroservices.Books.Api.Models;
using AspNetCoreMicroservices.Books.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreMicroservices.Books.Api
{
    public static class DiRegistration
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