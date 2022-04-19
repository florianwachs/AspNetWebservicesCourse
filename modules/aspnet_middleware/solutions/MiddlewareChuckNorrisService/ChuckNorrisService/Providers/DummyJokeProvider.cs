using ChuckNorrisService.Models;

namespace ChuckNorrisService.Providers;

public class DummyJokeProvider : IJokeProvider
{
    public Task<Joke> GetRandomJokeAsync()
    {
        return Task.FromResult(new Joke { Id = "test", Value = "Hahaha" });
    }
}
