using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreSwagger.Models
{
    public interface IAuthorRepository
    {
        Task<IReadOnlyCollection<Author>> GetAll();
    }
}