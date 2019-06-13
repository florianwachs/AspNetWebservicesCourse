using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreSecurity.Domain.Domain
{
    public class AuthConstants
    {
        public const string Issuer = "https://th-rosenheim.de/university-demo";

        // Claim Types
        public const string StudentType = "student";
        public const string PrincipalType = "principal";
        public const string ProfessorType = "professor";
    }

    public static class AppPolicies
    {
        public const string CanReadAllStudents = "CanReadAllStudents";
        public const string CanCreateNewStudent = "CanCreateNewStudent";
        public const string CanEditCourse = "CanEditCourse";
        public const string CanDeleteCourse = "CanDeleteCourse";
        public const string CanReadCourses = "CanReadCourses";
        public const string CanReadCourseGrades = "CanReadCourseGrades";
        public const string CanReadStudentsEnrolledInCourse = "CanReadStudentsEnrolledInCourse";
    }
}
