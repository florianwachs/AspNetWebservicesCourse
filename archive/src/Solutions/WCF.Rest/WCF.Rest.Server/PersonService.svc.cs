using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using WCF.Rest.Server.Dtos;

namespace WCF.Rest.Server
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PersonService : IPersonService
    {

        private int id = 4;
        private readonly Dictionary<string, Person> personen = new Dictionary<string, Person>{
            {"1", new Person{Id="1", Name="Jason Bourne", Age="30"}},
            {"2", new Person{Id="2", Name="Captain America", Age="80"}},
            {"3", new Person{Id="3", Name="Tony Stark", Age="40"}},
        };

        public Dtos.Person[] GetAllPerson()
        {
            return personen.Values.ToArray();
        }

        public Dtos.Person GetAPerson(string id)
        {
            Person p;
            return personen.TryGetValue(id, out p) ? p : null;
        }

        public Dtos.Person InsertPerson(Dtos.Person newPerson)
        {
            newPerson.Id = GetUniqueId();
            personen.Add(newPerson.Id, newPerson);
            return newPerson;
        }

        public Dtos.Person UpdatePerson(string id, Dtos.Person updatePerson)
        {
            updatePerson.Id = id;
            personen[id] = updatePerson;
            return updatePerson;
        }

        public void DeletePerson(string id)
        {
            personen.Remove(id);
        }

        private string GetUniqueId()
        {
            return (id++).ToString();
        }
    }
}
