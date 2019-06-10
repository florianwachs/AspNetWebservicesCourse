using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreTesting.Domain.Domain
{
    public class Course
    {
        public string Id { get; private set; }
        public string Identifier { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Professor Professor { get; private set; }

        private List<StudentCourse> _students = new List<StudentCourse>();
        public IReadOnlyCollection<StudentCourse> Students => _students.AsReadOnly();
    }
}
