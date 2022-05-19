using AspNetOpenApi;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(o =>
{
    o.LowercaseUrls = true; // Api Controller Namen klein ausgeben
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Jokes API", Version = "v1" });

    // C# XML-Kommentare für API-Beschreibung nutzen
    // ACHTUNG: Funktioniert nur für Controller aktuell
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Swagger Endpunkt nur in DEV aktivieren
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

MapMinimalApi(app);

app.Run();


void MapMinimalApi(WebApplication app)
{
    app.MapGet("api/v1/minimal/noinfo", () =>
    {
        return GenerateForecast();
    });

    app.MapGet("api/v1/minimal/fluent", () =>
    {

        return GenerateForecast();

    }).Produces<WeatherForecast>(contentType: "application/json")
    .WithTags("MinimalAPI");


    app.MapGet("api/v1/minimal/fluent/{id}", () =>
    {
        bool fail = Random.Shared.Next(1, 11) > 5;

        if (fail)
        {
            return Results.BadRequest(new ApiError("112", "OH NOOOOO, fehlender Infinity-Stein..."));
        }
        else
        {
            var forecast = GenerateForecast();
            return Results.Ok(forecast);
        }
    }).Produces<WeatherForecast>(contentType: "application/json")
    .Produces<ApiError>(statusCode: StatusCodes.Status404NotFound, contentType: "application/json")
    .WithTags("MinimalAPI");
}

WeatherForecast GenerateForecast()
{
    var result = new WeatherForecast
    {
        Date = DateTime.Now.AddDays(1),
        TemperatureC = Random.Shared.Next(-20, 55),
    };

    return result;
}