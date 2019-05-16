using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Server.Models;

namespace WebAPI.Server.DataAccess
{
    public class EFPersonRepository : IPersonRepository
    {
        public Person Add(Person p)
        {
            using (var context = new PersonDbContext())
            {
                context.Persons.Add(p);
                context.SaveChanges();
                return p;
            }
        }

        public bool Delete(int id)
        {
            using (var context = new PersonDbContext())
            {
                var person = context.Persons.Where(p => p.Id == id).FirstOrDefault();
                if (person == null)
                    return false;
                context.Persons.Remove(person);
                context.SaveChanges();
                return true;
            }
        }

        public IEnumerable<Person> GetAll()
        {
            using (var context = new PersonDbContext())
            {
                return context.Persons.ToArray();
            }
        }

        public Person GetById(int id)
        {
            using (var context = new PersonDbContext())
            {
                return context.Persons.Where(p => p.Id == id).FirstOrDefault();
            }
        }

        public Person Update(int id, Person update)
        {
            using (var context = new PersonDbContext())
            {
                var person = context.Persons.Where(p => p.Id == id).FirstOrDefault();
                if (person == null)
                    return null;
                person.Age = update.Age;
                person.Name = update.Name;
                
                context.SaveChanges();
                return person;
            }
        }
    }
}