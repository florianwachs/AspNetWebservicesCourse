using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Server.Models;

namespace WebAPI.Server.DataAccess
{
    public interface IPersonRepository
    {
        IEnumerable<Person> GetAll();
        Person GetById(int id);
        Person Add(Person p);
        Person Update(int id, Person p);
        bool Delete(int id);
    }
}
