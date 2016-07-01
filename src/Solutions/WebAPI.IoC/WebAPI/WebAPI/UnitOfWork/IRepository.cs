using System.Linq;

namespace WebAPI.IoC.Autofac.UnitOfWork
{
    public interface IRepository<T>
    {
        T Find(int id);

        IQueryable<T> GetAll();
    }
}