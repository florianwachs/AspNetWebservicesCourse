using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.IoC.Autofac.Models;

namespace WebAPI.IoC.Autofac.DataAccess
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAll();
        Book GetById(int id);
        Book Add(Book p);
        Book Update(int id, Book p);
        bool Delete(int id);
    }
}
