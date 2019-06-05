using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwaggerLesson.Models
{
    public class Author
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Book> Books { get; set; }

        public static Author NewFrom(string firstName, string lastName) => new Author
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName
        };
    }
}