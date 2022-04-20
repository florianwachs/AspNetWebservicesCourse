using ChuckNorrisService.Models;
using System.Text.Json;

namespace ChuckNorrisService.Providers;

public class FileSystemJokeProvider : IJokeProvider
{
    private static readonly Random random = new Random();
    private static readonly string JokeFilePath = Path.Combine("Data", "jokes.json");
    private List<Joke> _jokes;

    public async Task<Joke?> GetJokeById(string id)
    {
        await InitIfNecessary();
        return _jokes.FirstOrDefault(j => j.Id == id);
    }

    public async Task<Joke> GetRandomJokeAsync()
    {
        await InitIfNecessary();
        return _jokes[random.Next(0, _jokes.Count + 1)];
    }

    private async Task InitIfNecessary()
    {
        if (_jokes?.Any() == true)
            return;

        if (!File.Exists(JokeFilePath))
        {
            throw new InvalidOperationException($"no jokes file located in {JokeFilePath}");
        }

        var rawJson = await File.ReadAllTextAsync(JokeFilePath);
        _jokes = JsonSerializer.Deserialize<List<Joke>>(rawJson);
    }
}
