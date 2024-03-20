namespace ChuckNorrisService.Models;

public interface IJokeProvider
{
    Task<Joke[]> GetAll();
    Task<Joke> GetRandomJokeAsync();
    Task<Joke?> GetJokeById(string id);
    Task<Joke> Add(Joke joke);
    Task<Joke> Update(Joke joke);
    Task Delete(string id);
}