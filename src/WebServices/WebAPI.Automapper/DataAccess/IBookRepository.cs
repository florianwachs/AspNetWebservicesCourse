using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Automapper.Models;

namespace WebAPI.Automapper.DataAccess
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
