using EfCoreCqrs.Api;
using EfCoreCqrs.DataAccess;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

ConfigureDiServices(builder.Services);

var app = builder.Build();

await new DbSeeder().Seed(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", (BookDbContext context) => "Hi bitte einen Api Endpunkt unter /api/v1 aufrufen")
    .WithTags("Sample");

app.MapBooks();
app.MapAuthors();

app.Run();


static void ConfigureDiServices(IServiceCollection services)
{
    // Manuelles erzeugen der SqliteConection damit sie hier geöffnet werden kann
    // Sonst schließt der erste DBContext der Disposed wird die Connection und die
    // Db geht offline
    SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
    connection.Open();

    services.AddDbContext<BookDbContext>(options => { options.UseSqlite(connection); });
    services.AddMediatR(cfg=> cfg.RegisterServicesFromAssembly(typeof(BookDbContext).Assembly));
    
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Sample API",
            Description = "Sample",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Example Contact",
                Url = new Uri("https://example.com/contact")
            },
            License = new OpenApiLicense
            {
                Name = "Example License",
                Url = new Uri("https://example.com/license")
            }
        });
    });
}