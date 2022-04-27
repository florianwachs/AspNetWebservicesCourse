namespace ChuckNorrisService.Models;

public interface IJokeProvider
{
    Task<Joke> GetRandomJokeAsync();
}