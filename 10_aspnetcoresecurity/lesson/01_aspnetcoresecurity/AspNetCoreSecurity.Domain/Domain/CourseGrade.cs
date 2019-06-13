using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreSecurity.Domain.Domain
{
    public class CourseGrade
    {
        public string CourseId { get; private set; }
        public Course Course { get; private set; }
        public string StudentId { get; private set; }
        public Student Student { get; private set; }
        public decimal Points { get; private set; }
        public decimal Grade { get; private set; }

        private CourseGrade()
        {
        }
    }
}
