using ChuckNorrisService.Providers;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

FileSystemJokeProvider jokeProvider = new FileSystemJokeProvider();

app.MapGet("api/jokes/random", async context =>
{
    await context.Response.WriteAsJsonAsync(await jokeProvider.GetRandomJokeAsync());
});

app.Run();


