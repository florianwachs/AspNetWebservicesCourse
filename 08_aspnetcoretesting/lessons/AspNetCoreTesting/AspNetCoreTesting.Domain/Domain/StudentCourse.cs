using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreTesting.Domain.Domain
{
    public class StudentCourse
    {
        public string StudentId { get; private set; }
        public Student Student { get; private set; }
        public string CourseId { get; private set; }
        public Course Course { get; private set; }

        private StudentCourse()
        {
        }

        internal static StudentCourse Create(Student student, Course course)
        {
            return new StudentCourse
            {
                StudentId = student.Id,
                CourseId = course.Id,
            };
        }
    }
}
