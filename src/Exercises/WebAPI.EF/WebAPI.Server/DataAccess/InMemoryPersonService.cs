using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Server.Models;

namespace WebAPI.Server.DataAccess
{
    public class InMemoryPersonService : IPersonRepository
    {
        private static int id = 4;
        private static readonly Dictionary<string, Person> personen = new Dictionary<string, Person>{
            {"1", new Person{Id="1", Name="Jason Bourne", Age="30"}},
            {"2", new Person{Id="2", Name="Captain America", Age="80"}},
            {"3", new Person{Id="3", Name="Tony Stark", Age="40"}},
        };

        public Person Add(Person p)
        {
            p.Id = GetUniqueId();
            personen.Add(p.Id, p);
            return p;
        }

        public bool Delete(string id)
        {
            return personen.Remove(id);
        }

        public IEnumerable<Person> GetAll()
        {
            return personen.Values;
        }

        public Person GetById(string id)
        {
            Person p;
            return personen.TryGetValue(id, out p) ? p : null;
        }

        public Person Update(string id, Person p)
        {
            if (!personen.ContainsKey(id))
            {
                return null;
            }

            p.Id = id;
            personen[id] = p;
            return p;
        }
        private string GetUniqueId()
        {
            return (id++).ToString();
        }
    }
}