using ChuckNorrisService.Models;
using System.Text.Json;

namespace ChuckNorrisService.Providers;

public class FileSystemJokeProvider : IJokeProvider
{
    private static readonly Random random = new Random();
    private static readonly string JokeFilePath = Path.Combine("Data", "jokes.json");
    private List<Joke> _jokes = new();
    public FileSystemJokeProvider()
    {
        Init();
    }

    public async Task<Joke> GetRandomJokeAsync()
    {
        return _jokes[random.Next(0, _jokes.Count + 1)];
    }

    private void Init()
    {
        if (!File.Exists(JokeFilePath))
        {
            throw new InvalidOperationException($"no jokes file located in {JokeFilePath}");
        }

        var rawJson = File.ReadAllText(JokeFilePath);
        _jokes = JsonSerializer.Deserialize<List<Joke>>(rawJson) ?? new();
    }
}
