using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreMicroservices.Books.Api.Models
{
    public interface IAuthorRepository
    {
        Task<IReadOnlyCollection<Author>> GetAll();
    }
}