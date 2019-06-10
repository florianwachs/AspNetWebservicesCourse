using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreTesting.Domain.Domain
{
    public class CourseGrade
    {
        public string Id { get; set; }
        public Course Course { get; private set; }
        public Student Student { get; private set; }
        public decimal Points { get; private set; }
        public decimal Grade { get; private set; }
    }
}
