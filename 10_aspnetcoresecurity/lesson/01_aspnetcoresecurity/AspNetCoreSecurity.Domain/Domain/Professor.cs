using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreSecurity.Domain.Domain
{
    public class Professor
    {
        public string Id { get; private set; }
        public string Identifier { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string EMail { get; private set; }

        private List<Course> _assignedCourses = new List<Course>();
        public IReadOnlyCollection<Course> AssignedCourses => _assignedCourses.AsReadOnly();

        private Professor()
        {
        }

        public static Professor Create(string id, string firstName, string lastName, string email, string identifier)
        {
            // TODO: Validation
            return new Professor
            {
                Id = id,
                Identifier = identifier,
                FirstName = firstName,
                LastName = lastName,
                EMail = email,
            };
        }
    }
}
