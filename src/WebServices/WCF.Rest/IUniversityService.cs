using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WCF.Rest.Dtos;

namespace WCF.Rest
{
    [ServiceContract]
    public interface IUniversityService
    {
        // In REST-Services kann OperationContract auch weggelassen werden, 
        // wenn WebGet oder WebInvoke definiert wurde
        [OperationContract]
        [WebGet(UriTemplate = "/students")]
        Student[] GetStudents();

        [OperationContract]
        [WebGet(UriTemplate = "/students/{id}")]
        // Leider erlaubt UriTemplate nur string-Parameter
        Student GetStudentById(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/students", Method = "POST")]
        Student AddStudent(Student student);

        [OperationContract]
        [WebInvoke(UriTemplate = "/students/{id}", Method = "PUT")]
        void UpdateStudent(string id, Student student);

        [OperationContract]
        [WebInvoke(UriTemplate = "/students/{id}", Method = "DELETE")]
        void DeleteStudent(string id);
    }
}
