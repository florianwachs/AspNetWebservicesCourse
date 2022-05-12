
using EFCoreSample1.DataAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// ACHTUNG: Wenn der DbMode ge�ndert wird m�sst Ihr die Migrations im Order "Migrations" l�schen,
// das Projekt builden und �ber die Konsole "dotnet ef migrations add [Name]" aufrufen.
// Anschlie�end k�nnt Ihr "dotnet ef database update" aufrufen um die Migrationen auszuf�hren.
// Sonst passen die Migrationen nicht zu Eurer DB-Technologie
var dbMode = DbModes.SqlServerLocalDb;

ConfigureEntityFramework(builder.Services);

var app = builder.Build();

// Zwischen Build() und Run() ist eine g�nstige Gelegenheit f�r einfache Anwendungen / Microservices
// die Migrationen auszuf�hren.
// Falls mehrere Instanzen der gleichen App gestartet werden und auf die gleiche DB zeigen sind komplizierte
// Migrationsstrategien notwendig, etwa das Migration-Bundle Feature oder direkte SQL Erzeugung mit dem EF-Tool
var seeder = new DbSeeder();
await seeder.Seed(app.Services);

app.Run();




// *************************** Hilfsmethoden ***************************

void ConfigureEntityFramework(IServiceCollection services)
{

    switch (dbMode)
    {
        case DbModes.SqlLiteInMemory:
            UseInMemorySqlLiteDb(services);
            break;
        case DbModes.SqlServerLocalDb:
            UseSqlServerLocalDb(services);
            break;
        case DbModes.PostgreSql:
            UseDockerPostgreSql(services);
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
}

void UseSqlServerLocalDb(IServiceCollection services)
{
    services.AddDbContext<BookDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDb")));
}

static void UseInMemorySqlLiteDb(IServiceCollection services)
{
    // Manuelles erzeugen der SqliteConection damit sie hier ge�ffnet werden kann
    // Sonst schlie�t der erste DBContext der Disposed wird die Connection und die
    // Db geht offline
    SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
    connection.Open();

    services.AddDbContext<BookDbContext>(options => { options.UseSqlite(connection); });
}

void UseDockerPostgreSql(IServiceCollection services)
{
    // Achtung: Vorher muss per Docker Compose oder direkt mit Docker ein Docker Container gestartet werden.
    // Docker Container mit folgendem Befehl starten
    // docker run --rm   --name pg-docker -e POSTGRES_PASSWORD=docker -d -p 5432:5432 postgres
    // Dieser Befehl startet einen postgres Container und entfernt ihn samt Daten wenn er gestoppt wird.
    services.AddDbContext<BookDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlDocker"));
    });
}


public enum DbModes
{
    SqlLiteInMemory,
    SqlServerLocalDb,
    PostgreSql,
}