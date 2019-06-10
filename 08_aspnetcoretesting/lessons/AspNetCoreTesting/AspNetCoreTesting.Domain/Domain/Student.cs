using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreTesting.Domain.Domain
{
    public class Student
    {
        public string Id { get; private set; }
        public string Identifier { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string EMail { get; private set; }

        private List<StudentCourse> _enrolledCourses = new List<StudentCourse>();
        public IReadOnlyCollection<StudentCourse> EnrolledCourses => _enrolledCourses.AsReadOnly();

        private List<CourseRequest> _coursesRequests = new List<CourseRequest>();
        public IReadOnlyCollection<CourseRequest> CoursesRequests => _coursesRequests.AsReadOnly();

        private List<CourseGrade> _grades = new List<CourseGrade>();
        public IReadOnlyCollection<CourseGrade> Grades => _grades.AsReadOnly();

        private Student()
        {
        }

        public static Student Create(string id, string firstName, string lastName, string email, string identifier)
        {
            // TODO: Validation
            return new Student
            {
                Id = id,
                Identifier = identifier,
                FirstName = firstName,
                LastName = lastName,
                EMail = email,
            };
        }
    }
}
