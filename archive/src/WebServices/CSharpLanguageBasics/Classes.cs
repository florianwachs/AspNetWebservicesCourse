using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CSharpLanguageBasics
{
    public class Classes
    {
        public static void Demo1()
        {
            // Default Konstruktor
            var noName = new Person();
            Console.WriteLine(noName.Created);

            // Custom Konstruktor
            var hansi = new Person("Hansi", "Hintermeier");

            // Object-Initializer Syntax
            // https://msdn.microsoft.com/en-us/library/bb384062.aspx
            var nice = new Person
            {
                FirstName = "Jason",
                LastName = "Bourne"
            };
        }

        public static void Demo2()
        {
            var student = new Student
            {
                FirstName = "Schakeline Mandy",
                LastName = "Mandy"
            };

            student.MakeParty();
            student.WriteTests();
            student.Save();
        }
    }
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

    // Nur Einfachvererbung erlaubt
    public class Student : Person
    {
        public int Id { get; private set; }
        public decimal Motivation { get; private set; }
        public string MoodStatus
        {
            get
            {
                if (Motivation <= 10)
                {
                    return ":-(";
                }
                else
                {
                    return ":-)";
                }
            }
        }

        public Student()
        {
        }

        // Mit base kann auf die Konstruktoren der Basis zugegriffen werden
        public Student(string firstName, string lastName, decimal motivation)
            : base(firstName, lastName)
        {
            Motivation = motivation;
        }


        public void MakeParty()
        {
            Motivation = decimal.MaxValue;
        }

        public void WriteTests()
        {
            Motivation = decimal.MinValue;
        }

        public override string ToString()
        {
            // mit base kann auf Implementierungen der Basisklasse
            // zugegriffen werden
            return string.Format("{0} {1}:", base.ToString(), MoodStatus);
        }
    }


    #region abstract
    // von abstrakten Klassen kann keine
    // Instanz erzeugt werden
    public abstract class DtoBase
    {
        // abstrakte Member müssen von
        // den Ableitungen implementiert werden
        public abstract string ElementName
        {
            get;
        }

        public abstract Task AppendTo(XmlWriter w);
        public abstract void ReadFrom(XElement e);

        // virtual Methoden bieten eine Implementierung
        // die aber von Ableitungen überschrieben werden
        // können
        public virtual string GetDebugMessage()
        {
            return "Type: " + GetType().Name;
        }
    }

    public class ArticleDto : DtoBase
    {
        public static readonly string XmlElementName = "Article";
        public string Name { get; set; }

        public override string ElementName
        {
            get { return XmlElementName; }
        }

        public override Task AppendTo(XmlWriter w)
        {
            throw new NotImplementedException();
        }

        public override void ReadFrom(XElement e)
        {
            throw new NotImplementedException();
        }

        public override string GetDebugMessage()
        {
            return string.Format("{0} {1}", base.GetDebugMessage(), Name);
        }
    }
    #endregion

}
