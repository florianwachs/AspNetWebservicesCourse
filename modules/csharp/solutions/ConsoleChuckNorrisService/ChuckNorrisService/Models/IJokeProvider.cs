using System.Threading.Tasks;

namespace ChuckNorrisService.Models
{
    public interface IJokeProvider
    {
        Task<Joke> GetRandomJokeAsync();
    }
}