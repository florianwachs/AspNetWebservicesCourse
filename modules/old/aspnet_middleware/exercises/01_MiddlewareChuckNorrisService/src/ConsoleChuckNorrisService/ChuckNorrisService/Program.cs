using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;


await PrintJoke(new DummyJokeProvider());
await PrintJoke(new FileSystemJokeProvider());
await PrintJoke(new ApiJokeProvider());


static async Task PrintJoke(IJokeProvider jokeProvider)
{
    var joke = await jokeProvider.GetRandomJokeAsync();
    Console.WriteLine(joke.Value);
}