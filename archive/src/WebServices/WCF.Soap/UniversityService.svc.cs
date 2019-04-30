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

        public Student GetStudentById(int id)
        {
            Student s;
            return students.TryGetValue(id, out s) ? s : null;
        }

        public Student AddStudent(Student student)
        {
            student.Id = GetUniqueId();
            students.Add(student.Id, student);
            return student;
        }

        private static int GetUniqueId()
        {
            return id++;
        }

        public void UpdateStudent(Student student)
        {
            students[student.Id] = student;
        }

        public void DeleteStudent(int id)
        {
            students.Remove(id);
        }
    }
}
