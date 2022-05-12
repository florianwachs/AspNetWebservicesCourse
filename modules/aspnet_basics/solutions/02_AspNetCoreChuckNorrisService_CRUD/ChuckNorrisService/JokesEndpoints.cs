using ChuckNorrisService.Models;
using ChuckNorrisService.Validation;

namespace ChuckNorrisService;

public class JokesEndpoints
{
    public static void Register(WebApplication app)
    {
        app.MapGet("/api/jokes/random", GetRandomJoke);
        app.MapPost("/api/jokes", AddJoke); // CREATE
        app.MapGet("/api/jokes", GetAllJokes); // READ
        app.MapGet("/api/jokes/{id}", GetJokeById).WithName("GetJokeById");
        app.MapPut("/api/jokes/{id}", UpdateJoke); // UPDATE
        app.MapDelete("/api/jokes/{id}", DeleteJoke); // DELETE
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

    public static async Task<IResult> AddJoke(Joke joke, IJokeProvider jokeProvider, LinkGenerator linkGenerator)
    {
        var validator = new JokeValidator(false);
        var validationResult = await validator.ValidateAsync(joke);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        if (joke.Id == null)
        {
            joke.Id = Guid.NewGuid().ToString();
        }

        var uri = linkGenerator.GetPathByName("GetJokeById", new { id = joke.Id });
        return Results.Created(uri, await jokeProvider.Add(joke));
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
