using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNET.EFCodeFirst.Domain
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        // Die EF-Convention erkennt auch
        // [classname]Id als Key
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public Grade? Grade { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }

}
