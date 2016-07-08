namespace WebAPI.IoC.Autofac.Models
{
    public class Person
    {
        public Person()
        {
        }

        public Person(int personId, string firstName, string lastName)
        {
            PersonId = personId;
            FirstName = firstName;
            LastName = lastName;
        }

        public int PersonId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}