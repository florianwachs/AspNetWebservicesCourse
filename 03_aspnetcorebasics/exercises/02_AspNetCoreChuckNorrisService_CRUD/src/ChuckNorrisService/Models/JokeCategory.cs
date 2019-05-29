using System;

namespace ChuckNorrisService.Models
{
    public class JokeCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public static JokeCategory FromName(string name) 
            => new JokeCategory { Id = Guid.NewGuid().ToString(), Name = name };      
    }
}
