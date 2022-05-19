using Serilog;
using Serilog.Formatting.Compact;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


ConfigureRegularLogging(builder);
// ConfigureOnlyConsole(builder);
// ConfigureSerilog(builder);

var app = builder.Build();

app.MapGet("/", IndexRouteHandler);
app.MapGet("/concrete", ConcreteDiHandler);
app.MapGet("/structural", StructuralLoggingRouteHandler);

app.Run();

string IndexRouteHandler(ILoggerFactory loggerFactory)
{
    var logger = loggerFactory.CreateLogger(nameof(IndexRouteHandler));
    logger.LogTrace("Im Handler der Index route");
    logger.LogDebug("Hoffentlich klappt es");
    logger.LogInformation("Berechne komplexe Nachricht");
    logger.LogWarning("Berechnung dauert länger als erwartet.....");
    logger.LogError("Der Microservice für die Berechnung ist schon wieder down....");
    logger.LogCritical("Hat da grad jemand Cola in den Server verschüttet?");

    return "Hello World";
}

string ConcreteDiHandler(ILogger<Program> logger)
{
    logger.LogInformation("Berechne komplexe Nachricht");

    return "Hello World";
}

void StructuralLoggingRouteHandler(ILoggerFactory loggerFactory)
{
    var logger = loggerFactory.CreateLogger(nameof(StructuralLoggingRouteHandler));

    var userId = "[USER 123]";
    logger.LogInformation("Wir können interpolierte Strings loggen {UserId}", userId);

    var bookDto = new CreateBookDto("123", "Chucks Cooking Tips", 2000000000m);
    logger.LogError("Konnte das Buch {@Buch} nicht anlegen", bookDto);
}

void ConfigureRegularLogging(WebApplicationBuilder builder)
{// Standard mäßig werden folgende Logger konfiguriert (defaults)
 // Console
 // Debug
 // EventSource
 // EventLog (nur auf Windows)
}

void ConfigureOnlyConsole(WebApplicationBuilder builder)
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
}
void ConfigureSerilog(WebApplicationBuilder builder)
{
    // Default Logger entfernen
    builder.Logging.ClearProviders();

    // Serilog verwendet eine eigene Konfiguration, kann auch in der appsettings.json angegeben werden,
    // hat jedoch ein anderes Format als die Default Logger
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.Seq("http://localhost:5341")
        .WriteTo.File(new CompactJsonFormatter(), "log-.json", rollingInterval: RollingInterval.Day)
        .Enrich.WithProperty("Release", "0.0.1-beta-nightmare") // Ready for Production :-)
        .CreateLogger();

    // Serilog als Logger
    builder.Logging.AddSerilog(Log.Logger);
}

public record CreateBookDto(string Isin, string Name, decimal? Price);
