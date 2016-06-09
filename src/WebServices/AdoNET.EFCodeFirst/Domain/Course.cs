using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNET.EFCodeFirst.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Descriptions { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
