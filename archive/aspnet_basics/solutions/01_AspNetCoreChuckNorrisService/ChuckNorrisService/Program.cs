using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;


// Bitte jeweils nur eine Methode einkommentieren

//Exercise_1();
//Exercise_2();
//Exercise_3();
Exercise_4();

void Exercise_1()
{
    var builder = WebApplication.CreateBuilder(args);

    var app = builder.Build();

    FileSystemJokeProvider jokeProvider = new FileSystemJokeProvider();
    app.MapGet("api/jokes/random", async () =>
    {
        return await jokeProvider.GetRandomJokeAsync();
    });

    app.Run();
}

void Exercise_2()
{
    var builder = WebApplication.CreateBuilder(args);

    var app = builder.Build();

    FileSystemJokeProvider jokeProvider = new FileSystemJokeProvider();
    app.MapGet("api/jokes/random", async () =>
    {
        return await jokeProvider.GetRandomJokeAsync();
    });

    app.MapGet("api/jokes/{id}", async (string id) =>
    {
        return await jokeProvider.GetJokeById(id);
    });

    app.Run();
}


void Exercise_3()
{
    var builder = WebApplication.CreateBuilder(args);


    builder.Services.AddSingleton<IJokeProvider, FileSystemJokeProvider>();

    var app = builder.Build();

    app.MapGet("api/jokes/random", async (IJokeProvider jokeProvider) =>
    {
        return await jokeProvider.GetRandomJokeAsync();
    });

    app.MapGet("api/jokes/{id}", async (string id, IJokeProvider jokeProvider) =>
    {
        return await jokeProvider.GetJokeById(id);
    });

    app.Run();
}

void Exercise_4()
{
    var builder = WebApplication.CreateBuilder(args);


    builder.Services.AddSingleton<IJokeProvider, ApiJokeProvider>();

    var app = builder.Build();

    app.MapGet("api/jokes/random", async (IJokeProvider jokeProvider) =>
    {
        return await jokeProvider.GetRandomJokeAsync();
    });

    app.MapGet("api/jokes/{id}", async (string id, IJokeProvider jokeProvider) =>
    {
        return await jokeProvider.GetJokeById(id);
    });

    app.Run();
}

