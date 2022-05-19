using AspNetMediatR.Api.Domain.Jokes;

namespace AspNetMediatR.Api.ApplicationServices
{
    public interface IJokeRepository
    {
        Task<Joke> GetById(string id);
        Task<Joke> Add(Joke joke);
        Task<Joke> Update(Joke joke);
        Task Delete(string id);
        Task<Joke> GetRandomJoke();
    }
}