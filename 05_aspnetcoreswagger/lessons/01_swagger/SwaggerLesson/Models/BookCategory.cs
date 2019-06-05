using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwaggerLesson.Models
{
    public class BookCategory : IEquatable<BookCategory>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Name { get; set; }
        public static BookCategory FromName(string name) 
            => new BookCategory { Id = Guid.NewGuid().ToString(), Name = name };

        public bool Equals(BookCategory other)
        {
            return other != null && other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BookCategory);
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }
}
