using ChuckNorrisService.Models;

namespace ChuckNorrisService.Dtos;

public record JokeApiDto(string Id, string Value, string[]? Category)
{
    public static JokeApiDto From(Joke joke)
    {
        return new JokeApiDto(joke.Id, joke.Value, joke.Categories.Select(j => j.Name).ToArray());
    }
}

