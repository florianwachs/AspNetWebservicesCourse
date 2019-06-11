using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreTesting.Domain.Domain
{
    public enum CourseStates
    {
        New,
        OpenForEnrollment,
        InProgress,
        Grades,
        Completed,
        Canceled,
    }

    public class Course
    {
        public string Id { get; private set; }
        public string Identifier { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public CourseStates State { get; private set; }
        public string ProfessorId { get; private set; }
        public Professor Professor { get; private set; }

        private Course()
        {
        }

        private List<StudentCourse> _students = new List<StudentCourse>();
        public IReadOnlyCollection<StudentCourse> Students => _students.AsReadOnly();

        private List<CourseGrade> _grades = new List<CourseGrade>();
        public IReadOnlyCollection<CourseGrade> Grades => _grades.AsReadOnly();

        public static Course Create(string id, string identifier, string name, string description)
        {
            return new Course
            {
                Id = id,
                Identifier = identifier,
                Name = name,
                Description = description
            };
        }

        public void AcceptEnrollment(Student student)
        {
            if (IsEnroled(student))
            {
                return;
            }

            _students.Add(StudentCourse.Create(student, this));
        }

        public bool IsEnroled(Student student)
        {
            return Students.Any(s => s.StudentId == student.Id);
        }

        public void AssignProfessorToCourse(Professor professor)
        {
            if (!IsProfessorAssignmentAllowed())
            {
                throw new ApplicationException("Assignment not allowed at this state");
            }

            Professor = professor;
        }

        public bool IsProfessorAssignmentAllowed()
        {
            return State == CourseStates.New || State == CourseStates.OpenForEnrollment;
        }

        public void OpenCourseForEnrollment()
        {
            if (State != CourseStates.New)
            {
                throw new ApplicationException("Only new courses can be opened for enrollment");
            }

            State = CourseStates.OpenForEnrollment;
        }
    }
}
