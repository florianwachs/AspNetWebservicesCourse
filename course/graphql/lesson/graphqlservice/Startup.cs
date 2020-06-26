using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using graphqlservice.BookReviews;
using graphqlservice.Books;
using graphqlservice.GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            services.AddScoped<BookRepository>();
            services.AddScoped<BookReviewRepository>();

            // GraphQL.NET verwendet eine eigene Resolver Abstraktion
            // Hier wird der AspNetCore-Resolver verwendet
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

            // Das Schema muss auch am DI registriert werden
            services.AddScoped<BookStoreSchema>();

            // Von GraphQL.NET benötigte Services hinzufügen, inkl. GraphTypes
            services.AddGraphQL(options =>
            {
                options.ExposeExceptions = true;
            }).AddGraphTypes(ServiceLifetime.Scoped);
            services.AddControllers();

            // FIXME: Workaround bis GraphQL.NET System.Text.Json verwendet
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
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
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
        }
    }
}
