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
        // Wir f�hren den Request aus
        await next();

        // und verz�gern die Antwort
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

    // Immer direkt mit dem Context zu arbeiten ist aufw�ndig,
    // die Minimal APIs machen dies auch un�tig
    //app.MapGet("api/jokes/random", async context =>
    //{
    //    await context.Response.WriteAsJsonAsync(await jokeProvider.GetRandomJokeAsync());
    //});

    app.MapGet("api/jokes/random", async () =>
    {
        return await jokeProvider.GetRandomJokeAsync();
    });

    app.MapGet("{*path}", context =>
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return context.Response.WriteAsync("Well, IT'S YOUR FAULT!");
    });
}

