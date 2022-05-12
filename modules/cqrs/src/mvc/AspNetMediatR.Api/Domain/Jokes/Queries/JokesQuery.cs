using AspNetMediatR.Api.Domain.Jokes.Models;
using MediatR;

namespace AspNetMediatR.Api.Domain.Jokes.Queries;

public class JokesQuery : IRequest<JokesQueryResult>
{
    public int PerPage { get; set; } = 10;
}

public class JokesQueryResult
{
    public IReadOnlyCollection<Joke> Jokes { get; private set; }

    public JokesQueryResult(IEnumerable<Joke> jokes)
    {
        Jokes = jokes?.ToArray() ?? Array.Empty<Joke>();
    }        
}

public class JokesQueryHandler : IRequestHandler<JokesQuery, JokesQueryResult>
{
    public JokesQueryHandler(IJokeRepository jokeRepository)
    {
        JokeRepository = jokeRepository;
    }

    private IJokeRepository JokeRepository { get; }

    public async Task<JokesQueryResult> Handle(JokesQuery request, CancellationToken cancellationToken)
    {
        var jokeQuery = Enumerable.Range(0, request.PerPage).Select(_ => JokeRepository.GetRandomJoke()).ToArray();

        var result = await Task.WhenAll(jokeQuery);

        return new JokesQueryResult(result);
    }
}

