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
    public class UniversityService : IUniversityService
    {
        private static int id = 4;
        private static readonly Dictionary<int, Student> students = new Dictionary<int, Student>{
            {1, new Student{Id=1, FirstName="Jason", LastName="Bourne"}},
            {2, new Student{Id=2, FirstName="Captain", LastName="America"}},
            {3, new Student{Id=3, FirstName="Tony", LastName="Stark"}},
        };

        public Student[] GetStudents()
        {
            return students.Values.ToArray();
        }

        public Student GetStudentById(string studentId)
        {
            Student s;
            return students.TryGetValue(ConvertId(studentId), out s) ? s : null;
        }

        public Student AddStudent(Student student)
        {
            student.Id = GetUniqueId();
            students.Add(student.Id, student);
            return student;
        }

        public void UpdateStudent(string studentId, Student student)
        {

            students[ConvertId(studentId)] = student;
        }

        public void DeleteStudent(string studentId)
        {
            students.Remove(ConvertId(studentId));
        }

        private int ConvertId(string idString)
        {
            int id;
            int.TryParse(idString, out id);
            return id;
        }

        private static int GetUniqueId()
        {
            return id++;
        }
    }
}
