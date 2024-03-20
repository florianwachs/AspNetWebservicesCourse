using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using System.Diagnostics;

await Aufgabe2();
await Aufgabe3();

WaitForUser();

static void WaitForUser()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Please press the any-Key and only the any-Key");
    Console.ReadKey();
}

async Task Aufgabe2()
{
    var watch = Stopwatch.StartNew();
    var provider = new ApiJokeProvider();

    var jokes = new List<Joke>();
    jokes.Add(await provider.GetRandomJokeAsync());
    jokes.Add(await provider.GetRandomJokeAsync());
    jokes.Add(await provider.GetRandomJokeAsync());
    jokes.Add(await provider.GetRandomJokeAsync());
    jokes.Add(await provider.GetRandomJokeAsync());

    var längsterWitz = jokes.OrderByDescending(j => j.Value.Length).FirstOrDefault();

    watch.Stop();

    Console.WriteLine($"[{watch.ElapsedMilliseconds} ms]: {längsterWitz.Value}");
}

async Task Aufgabe3()
{
    var watch = Stopwatch.StartNew();
    var provider = new ApiJokeProvider();

    var jokeTasks = new List<Task<Joke>>();
    jokeTasks.Add(provider.GetRandomJokeAsync());
    jokeTasks.Add(provider.GetRandomJokeAsync());
    jokeTasks.Add(provider.GetRandomJokeAsync());
    jokeTasks.Add(provider.GetRandomJokeAsync());
    jokeTasks.Add(provider.GetRandomJokeAsync());

    var jokes = await Task.WhenAll(jokeTasks);

    var jokesByLength = jokes.GroupBy(j => GetLength(j));
    foreach (var grp in jokesByLength)
    {
        Console.WriteLine($"Gruppe {grp.Key}");
        foreach (var joke in grp)
        {
            Console.WriteLine(joke.Value);
        }
    }

    watch.Stop();

    Console.WriteLine($"[{watch.ElapsedMilliseconds} ms]");
}

async Task Aufgabe4()
{
    // Joke als record umsetzen
}

JokeLengths GetLength(Joke joke)
{
    return joke.Value.Length switch
    {
        > 80 => JokeLengths.Long,
        > 60 => JokeLengths.Medium,
        _ => JokeLengths.Short,
    };
}

public enum JokeLengths
{
    Short,
    Medium,
    Long,
}



