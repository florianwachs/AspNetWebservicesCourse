using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;

await PrintJoke(new ApiJokeProvider());

WaitForUser();

static void WaitForUser()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Please press the any-Key and only the any-Key");
    Console.ReadKey();
}

static async Task PrintJoke(IJokeProvider jokeProvider)
{
    var joke = await jokeProvider.GetRandomJokeAsync();
    Console.WriteLine(joke.Value);
}

