using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCoreAutomapper.Models
{
    public class Author
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Joke> Jokes { get; set; }
        public decimal MonthlyPaymentAmount { get; set; }
        public int Age { get; set; }

        public static Author NewFrom(string firstName, string lastName) => new Author
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName
        };

        public Author WithAge(int age)
        {
            Age = age;
            return this;
        }

        public Author WithMontlyPayment(decimal amount)
        {
            MonthlyPaymentAmount = amount;
            return this;
        }
    }
}