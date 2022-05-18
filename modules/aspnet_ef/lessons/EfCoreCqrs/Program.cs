using EfCoreCqrs.Api;
using EfCoreCqrs.DataAccess;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

ConfigureDiServices(builder.Services);

var app = builder.Build();

await new DbSeeder().Seed(app.Services);


app.MapGet("/", (BookDbContext context) => "Hi bitte einen Api Endpunkt unter /api/v1 aufrufen");
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
    services.AddMediatR(typeof(BookDbContext).Assembly);
}