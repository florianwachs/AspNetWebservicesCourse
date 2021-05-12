using GraphQL;
using GraphQL.Server;
using GraphQL.SystemTextJson;
using graphqlservice.BookReviews;
using graphqlservice.Books;
using graphqlservice.GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace graphqlservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Repositories registrieren
            services.AddScoped<IBookRepository, InMemoryBookRepository>();
            services.AddScoped<IBookReviewRepository, InMemoryBookReviewRepository>();
            // Das Schema muss auch am DI registriert werden
            services.AddScoped<BookStoreSchema>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            // Von GraphQL.NET benötigte Services hinzufügen, inkl. GraphTypes
            services.AddGraphQL(options =>
            {
            })
            .AddSystemTextJson()
            .AddGraphTypes(ServiceLifetime.Scoped);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // qraphql-Endpunkt registrieren
            // Man kann auch mehrere Schemas unter verschiedenen Endpunkten registrieren
            app.UseGraphQL<BookStoreSchema>();

            // Der Playground ist unter /ui/playground erreichbar und hilft beim
            // erforschen des Schemas und erstellen von abfragen
            app.UseGraphQLPlayground();
        }
    }
}
