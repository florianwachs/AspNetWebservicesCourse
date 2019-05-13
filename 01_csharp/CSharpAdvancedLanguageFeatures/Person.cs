using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvancedLanguageFeatures
{
    public class Person
    {
        // Fields
        private long? id;

        // Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Created { get; private set; }
        public bool IsNew
        {
            get { return !id.HasValue; }
        }

        // Konstruktoren: Expliziter Default
        public Person()
        {
            Created = DateTime.Now;
        }

        // Custom Konstruktor der den Default-Construktor aufruft
        public Person(string firstName, string lastName)
            : this()
        {
            FirstName = firstName;
            LastName = lastName;
        }

        // Methoden
        public void Save()
        {
            // Sehr empfehlenswert!
            id = new Random().Next();
        }

        public override string ToString()
        {
            return FirstName + ", " + LastName;
        }

        // indexer
        // events
        // delegates       
    }
}
