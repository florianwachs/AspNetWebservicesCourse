using Xunit;

namespace Advanced;

public class AnonymousTypes
{
    [Fact]
    public void Definition()
    {
        // Der Compiler implementiert automatisch eine
        // Klasse und implementiert ToString Equals und GetHashCode
        // Die Klasse leitet direkt von Object an
        var person1 = new { FirstName = "Jason", LastName = "Bourne" };
        Console.WriteLine(person1);


        // Für person2 wird die gleiche Klasse
        // verwendet wie für Person 1
        var person2 = new { FirstName = "Jason", LastName = "Bourne" };

        Console.WriteLine(person1.Equals(person2)); // true

        // Für person3 erzeugt der Compiler eine neue
        // Klasse, es kommt auf die Reihenfolge der
        // Properties an
        var person3 = new { LastName = "Bourne", FirstName = "Jason" };

        Console.WriteLine(person1.Equals(person3)); // false
    }

    [Fact]
    public void UseCaseLinq()
    {
        var query = Enumerable.Range(0, 5).Select(n => new
        {
            Number = n,
            Square = n * n,
            Sqrt = Math.Sqrt(n)
        });

        foreach (var item in query)
        {
            Console.WriteLine("Number: {0}, Square: {1}, Sqrt: {2}", item.Number, item.Square, item.Sqrt);
        }
    }

    [Fact]
    public void UseCaseLinq2()
    {
        var query = from person in GetTestPersons()
                    group person by person.FirstName[0] into grp
                    orderby grp.Key
                    select new { FirstLetter = grp.Key, Personen = grp };

        foreach (var gruppe in query)
        {
            Console.WriteLine("First Letter: " + gruppe.FirstLetter);
            foreach (var person in gruppe.Personen)
            {
                Console.WriteLine("\t" + person.ToString());
            }
        }
    }

    private IEnumerable<Person> GetTestPersons()
    {
        return new[]
        {
            new Person{ FirstName="Jason", LastName="Bourne"},
            new Person{ FirstName="Angi", LastName="Meier"},
            new Person{ FirstName="Sepp", LastName="Meier"},
            new Person{ FirstName="T-1000", LastName="Müller"},
            new Person{ FirstName="T-800", LastName="Müller"},
        };
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
