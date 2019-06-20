using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCoreAutomapper.Models
{
    public class JokeCategory : IEquatable<JokeCategory>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Name { get; set; }
        public static JokeCategory FromName(string name) 
            => new JokeCategory { Id = Guid.NewGuid().ToString(), Name = name };

        public bool Equals(JokeCategory other)
        {
            return other != null && other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JokeCategory);
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }
}
