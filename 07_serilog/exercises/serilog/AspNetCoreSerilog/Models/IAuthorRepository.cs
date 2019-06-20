using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreSerilog.Models
{
    public interface IAuthorRepository
    {
        Task<IReadOnlyCollection<Author>> GetAll();
    }
}