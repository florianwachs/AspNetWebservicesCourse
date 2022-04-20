using ChuckNorrisService.Models;
using System.Text.Json;

namespace ChuckNorrisService.Providers;

public class FileSystemJokeProvider : IJokeProvider
{
    private static readonly Random random = new Random();
    private static readonly string JokeFilePath = Path.Combine("Data", "jokes.json");
    private List<Joke> _jokes = new();

    public async Task<Joke> Add(Joke joke)
    {
        await InitIfNecessary();
        if (string.IsNullOrWhiteSpace(joke.Id))
        {
            joke.Id = Guid.NewGuid().ToString();
        }

        _jokes.Add(joke);
        return joke;
    }

    public async Task<Joke> Update(Joke joke)
    {
        await InitIfNecessary();

        var existing = _jokes.FirstOrDefault(x => x.Id == joke.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"No Joke found with id {joke.Id}");
        }

        _jokes.Remove(existing);
        _jokes.Add(joke);
        return joke;
    }

    public async Task Delete(string id)
    {
        await InitIfNecessary();
        _jokes.RemoveAll(x => x.Id == id);
    }

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

    public async Task<Joke[]> GetAll()
    {
        await InitIfNecessary();
        return _jokes.ToArray();
    }
}
