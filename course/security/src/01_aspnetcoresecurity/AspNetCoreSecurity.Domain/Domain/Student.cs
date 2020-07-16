using System.Collections.Generic;
using System.Linq;
using AspNetCoreSecurity.Domain.DomainErrors;
using Unit = System.ValueTuple;

namespace AspNetCoreSecurity.Domain.Domain
{
    public class Student
    {
        public string Id { get; private set; }
        public string Identifier { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string EMail { get; private set; }

        private readonly List<StudentCourse> _enrolledCourses = new List<StudentCourse>();
        public IReadOnlyCollection<StudentCourse> EnrolledCourses => _enrolledCourses.AsReadOnly();

        private readonly List<CourseRequest> _coursesRequests = new List<CourseRequest>();
        public IReadOnlyCollection<CourseRequest> CoursesRequests => _coursesRequests.AsReadOnly();

        private readonly List<CourseGrade> _grades = new List<CourseGrade>();
        public IReadOnlyCollection<CourseGrade> Grades => _grades.AsReadOnly();

        private Student()
        {
        }

        public static Student Create(string id, string firstName, string lastName, string email, string identifier) =>
            // TODO: Validation
            new Student
            {
                Id = id,
                Identifier = identifier,
                FirstName = firstName,
                LastName = lastName,
                EMail = email,
            };

        public Validation<Unit> RequestCourseEnrolement(Course course)
        {
            if (course.IsEnroled(this))
            {
                return Validation.Fail(new StudentAlreadyEnrolledInCourse());
            }

            if (HasRequestedEnrollment(course))
            {
                return Validation.Fail(new StudentAlreadyRequestedCourseEnrollment());
            }

            _coursesRequests.Add(CourseRequest.Create(this, course));

            return Validation.Success();
        }

        public bool HasRequestedEnrollment(Course course) => CoursesRequests.Any(cr => cr.RequestedCourseId == course.Id);
    }
}
