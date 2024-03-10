using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IJokeProvider, FileSystemJokeProvider>();

var app = builder.Build();

app.MapGet("api/jokes/random", async (IJokeProvider jokeProvider) =>
{
    return await jokeProvider.GetRandomJokeAsync();
});

app.Run();