using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreMicroservices.Jokes.Api.Models
{
    public interface IAuthorRepository
    {
        Task<IReadOnlyCollection<Author>> GetAll();
    }
}