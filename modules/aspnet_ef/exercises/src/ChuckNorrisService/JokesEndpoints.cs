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

    public static async Task<Joke> GetRandomJoke(IJokeProvider jokeProvider)
    {
        return await jokeProvider.GetRandomJokeAsync();
    }

    public static async Task<IResult> GetJokeById(string id, IJokeProvider jokeProvider)
    {
        var joke = await jokeProvider.GetJokeById(id);
        return joke is null ? Results.NotFound() : Results.Ok(joke);
    }

    public static async Task<Joke[]> GetAllJokes(IJokeProvider jokeProvider)
    {
        var jokes = await jokeProvider.GetAll();
        return jokes;
    }

    public static async Task<IResult> AddJoke(Joke joke, IJokeProvider jokeProvider)
    {
        var validator = new JokeValidator(false);
        var validationResult = await validator.ValidateAsync(joke);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        return Results.Ok(await jokeProvider.Add(joke));
    }

    public static async Task<IResult> UpdateJoke(string id, Joke joke, IJokeProvider jokeProvider)
    {
        joke.Id = id;
        var validator = new JokeValidator(true);
        var validationResult = await validator.ValidateAsync(joke);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        return Results.Ok(await jokeProvider.Update(joke));
    }

    public static async Task DeleteJoke(string id, IJokeProvider jokeProvider)
    {
        await jokeProvider.Delete(id);
    }
}
