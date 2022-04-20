namespace ChuckNorrisService.Models;

public interface IJokeProvider
{
    Task<Joke> GetRandomJokeAsync();
    Task<Joke?> GetJokeById(string id);
}