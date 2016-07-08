using System.Collections.Generic;
using System.Linq;
using WebAPI.IoC.Autofac.Models;

namespace WebAPI.IoC.Autofac.UnitOfWork
{
    public class PersonsRepository : IRepository<Person>
    {
        private static readonly IDictionary<int, Person> Persons = new Dictionary<int, Person>
        {
            {1, new Person(1, "Master", "Of Desaster")},
            {2, new Person(2, "Grand Sheppard", "Of the unicorns")},
            {3, new Person(3, "Chuck", "Norris")}
        };

        public Person Find(int id)
        {
            Person result;
            return Persons.TryGetValue(id, out result) ? result : null;
        }

        public IQueryable<Person> GetAll()
        {
            return Persons.Values.AsQueryable();
        }
    }
}