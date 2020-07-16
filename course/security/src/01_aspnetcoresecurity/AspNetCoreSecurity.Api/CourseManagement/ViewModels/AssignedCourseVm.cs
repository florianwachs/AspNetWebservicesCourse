using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreSecurity.Api.CourseManagement.ViewModels
{
    public class AssignedCourseVm
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string ProfessorId { get; set; }
        public string ProfessorName { get; set; }
    }
}
