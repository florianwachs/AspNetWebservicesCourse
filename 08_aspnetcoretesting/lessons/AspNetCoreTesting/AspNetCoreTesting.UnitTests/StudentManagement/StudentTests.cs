using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreTesting.Domain.ApplicationServices;
using AspNetCoreTesting.Domain.Domain;
using Xunit;

namespace AspNetCoreTesting.UnitTests.StudentManagement
{
    public class StudentTests
    {
        private IdGenerator IdGen { get; } = new IdGenerator();

        [Fact]
        public void Should_be_able_to_request_enrollment_in_course()
        {
            var course = CreateCourse();
            var student = CreateStudent();

            var result = student.RequestCourseEnrolement(course);
            Assert.True(result.IsValid);
            Assert.True(student.HasRequestedEnrollment(course));
        }

        [Fact]
        public void Should_not_be_able_to_request_enrollment_if_already_in_course()
        {
            var course = CreateCourse();
            var student = CreateStudent();

            student.RequestCourseEnrolement(course);
            course.AcceptEnrollment(student);
            student.RequestCourseEnrolement(course);
        }

        [Fact]
        public void Should_not_be_able_to_request_enrollment_if_already_requested()
        {
            var course = CreateCourse();
            var student = CreateStudent();
            var result = student.RequestCourseEnrolement(course);
            Assert.True(result.IsValid);

            result = student.RequestCourseEnrolement(course);
            Assert.False(result.IsValid);
        }

        private Course CreateCourse()
        {
            return Course.Create("test-1", "TestCourse1", "Test Course", "lorem...");
        }

        public Student CreateStudent(string id = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = IdGen.NewEntityId();
            }

            return Student.Create(id, "Firstname", "Lastname", "f@f.de", "FiLa");
        }
    }
}
