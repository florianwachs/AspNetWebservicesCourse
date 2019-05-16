using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using WCF.Rest.Server.Dtos;

namespace WCF.Rest.Server
{
    [ServiceContract]
    public interface IPersonService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/personen")]
        Person[] GetAllPerson();

        [OperationContract]
        [WebGet(UriTemplate = "/personen/{id}")]
        Person GetAPerson(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/personen", Method = "POST")]
        Person InsertPerson(Person newPerson);

        [OperationContract]
        [WebInvoke(UriTemplate = "/personen/{id}", Method = "PUT")]
        Person UpdatePerson(string id, Person updatePerson);

        [OperationContract]
        [WebInvoke(UriTemplate = "/personen/{id}", Method = "DELETE")]
        void DeletePerson(string id);
    }
}
