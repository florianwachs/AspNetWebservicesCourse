using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreTesting.Domain.Domain
{
    public class Professor
    {
        public string Id { get; private set; }
        public string Identifier { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        private List<Course> _assignedCourses = new List<Course>();
        public IReadOnlyCollection<Course> AssignedCourses => _assignedCourses.AsReadOnly();
    }
}
