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

    public async Task<Joke> Add(Joke joke)
    {
        if (string.IsNullOrWhiteSpace(joke.Id))
        {
            joke.Id = Guid.NewGuid().ToString();
        }

        _jokes.Add(joke);
        return joke;
    }

    public async Task<Joke> Update(Joke joke)
    {
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
        _jokes.RemoveAll(x => x.Id == id);
    }

    public async Task<Joke?> GetJokeById(string id)
    {
        return _jokes.FirstOrDefault(j => j.Id == id);
    }

    public async Task<Joke> GetRandomJokeAsync()
    {
        return _jokes[random.Next(0, _jokes.Count + 1)];
    }

    public async Task<Joke[]> GetAll()
    {
        return _jokes.ToArray();
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
