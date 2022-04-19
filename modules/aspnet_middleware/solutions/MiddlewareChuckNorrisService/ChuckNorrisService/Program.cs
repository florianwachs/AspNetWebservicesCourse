using ChuckNorrisService.Providers;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Bitte nur jeweils eine der Konfigurationen einkommentieren
Exercise_1_and_2(app);
//Exercise_3(app);
//Exercise_4(app);

app.Run();


void Exercise_1_and_2(WebApplication app)
{
    FileSystemJokeProvider jokeProvider = new FileSystemJokeProvider();
    app.Run(async context =>
    {
        await context.Response.WriteAsJsonAsync(await jokeProvider.GetRandomJokeAsync());
    });
}

void Exercise_3(WebApplication app)
{
    FileSystemJokeProvider jokeProvider = new FileSystemJokeProvider();


    app.Use(async (context, next) =>
    {
        // Wir führen den Request aus
        await next();

        // und verzögern die Antwort
        await Task.Delay(TimeSpan.FromSeconds(2));
    });

    app.Run(async context =>
    {
        await context.Response.WriteAsJsonAsync(await jokeProvider.GetRandomJokeAsync());
    });
}

void Exercise_4(WebApplication app)
{
    FileSystemJokeProvider jokeProvider = new FileSystemJokeProvider();

    if (app.Environment.IsDevelopment())
    {
        app.MapGet("api/jokes/random", async context =>
        {
            await context.Response.WriteAsJsonAsync(await jokeProvider.GetRandomJokeAsync());
        });
    }

    app.MapGet("{*path}", context =>
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return context.Response.WriteAsync("Well, IT'S YOUR FAULT!");
    });
}

