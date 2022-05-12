using ChuckNorrisService.Dtos;
using ChuckNorrisService.Models;
using ChuckNorrisService.Validation;

namespace ChuckNorrisService;

public class JokesEndpoints
{
    public static void Register(WebApplication app)
    {
        app.MapGet("api/jokes/random", GetRandomJoke);
        app.MapPost("api/jokes", AddJoke); // CREATE
        app.MapGet("api/jokes", GetAllJokes); // READ ALL
        app.MapGet("api/jokes/{id}", GetJokeById); // READ
        app.MapPut("api/jokes/{id}", UpdateJoke); // UPDATE
        app.MapPut("api/jokes/{id}", DeleteJoke); // DELETE
    }

    public static async Task<JokeApiDto> GetRandomJoke(IJokeRepository jokeRepository)
    {
        var joke = await jokeRepository.GetRandomJokeAsync();
        return JokeApiDto.From(joke);
    }

    public static async Task<IResult> GetJokeById(string id, IJokeRepository jokeProvider)
    {
        var joke = await jokeProvider.GetJokeById(id);
        return joke is null ? Results.NotFound() : Results.Ok(JokeApiDto.From(joke));
    }

    public static async Task<JokeApiDto[]> GetAllJokes(IJokeRepository jokeProvider)
    {
        var jokes = await jokeProvider.GetAll();
        return jokes.Select(j => JokeApiDto.From(j)).ToArray();
    }

    public static async Task<IResult> AddJoke(Joke joke, IJokeRepository jokeProvider)
    {
        var validator = new JokeValidator(false);
        var validationResult = await validator.ValidateAsync(joke);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var addedJoke = await jokeProvider.Add(joke);
        return Results.Ok(JokeApiDto.From(addedJoke));
    }

    public static async Task<IResult> UpdateJoke(string id, Joke joke, IJokeRepository jokeProvider)
    {
        joke.Id = id;
        var validator = new JokeValidator(true);
        var validationResult = await validator.ValidateAsync(joke);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var updatedJoke = await jokeProvider.Update(joke);
        return Results.Ok(JokeApiDto.From(updatedJoke));
    }

    public static async Task DeleteJoke(string id, IJokeRepository jokeProvider)
    {
        await jokeProvider.Delete(id);
    }

}
