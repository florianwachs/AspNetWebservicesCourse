using ChuckNorrisService.Client;
using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using System.Text.Json;

await PrintJoke(new DummyJokeProvider());
await PrintJoke(new FileSystemJokeProvider());
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

/// <summary>
/// Hilfs Methode welche von der Jokes-API die Witze lädt und lokal abspeichert
/// </summary>
/// <returns></returns>
static async Task SaveJokesToFile()
{
    var api = new ChuckNorrisApi();
    var result = await api.GetRandomJokesFromCategory(JokeCategories.Dev, 10);

    SaveJokes(result);

    void SaveJokes(ChuckNorrisApi.ChuckNorrisJoke[] jokesToSerialize)
    {
        // TODO: Nur für Vorlesungszwecke! Das referenzieren von Strings erzeugt
        // eine Kopie, problematisch bei großen strings (LOH)
        var raw = JsonSerializer.Serialize(jokesToSerialize);
        File.WriteAllText($"jokes_{DateTime.Now.Ticks}.json", raw);
    }
}

