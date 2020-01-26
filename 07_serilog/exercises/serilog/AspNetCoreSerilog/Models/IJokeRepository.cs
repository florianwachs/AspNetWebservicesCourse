using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreSerilog.Models
{
    public interface IJokeRepository
    {
        Task<IReadOnlyCollection<Joke>> GetAll();
        ValueTask<Joke> GetById(string id);
        Task<Joke> Add(Joke joke);
        Task<Joke> Update(Joke joke);
        Task Delete(string id);
        Task<Joke> GetRandomJoke();
    }
}