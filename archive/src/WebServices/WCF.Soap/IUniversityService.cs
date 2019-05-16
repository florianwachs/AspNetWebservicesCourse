using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WCF.Soap.Dtos;

namespace WCF.Soap
{
    [ServiceContract]
    public interface IUniversityService
    {
        [OperationContract]
        Student[] GetStudents();

        [OperationContract]
        Student GetStudentById(int id);

        [OperationContract]
        Student AddStudent(Student student);

        [OperationContract]
        void UpdateStudent(Student student);

        [OperationContract]
        void DeleteStudent(int id);
    }
}
