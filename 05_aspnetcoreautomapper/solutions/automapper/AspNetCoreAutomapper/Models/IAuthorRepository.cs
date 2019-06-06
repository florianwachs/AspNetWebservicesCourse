using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreAutomapper.Models
{
    public interface IAuthorRepository
    {
        Task<IReadOnlyCollection<Author>> GetAll();
    }
}