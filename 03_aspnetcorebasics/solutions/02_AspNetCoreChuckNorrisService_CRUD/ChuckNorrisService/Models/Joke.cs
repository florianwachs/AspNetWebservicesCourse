using System;
using System.Collections.Generic;

namespace ChuckNorrisService.Models
{
    public class Joke
    {
        public string Id { get; set; }
        public string JokeText { get; set; }
        public IEnumerable<JokeCategory> Categories { get; set; }
    }

    public class JokeDto
    {
        public string Id { get; set; }
        public string JokeText { get; set; }
        public string[] Category { get; set; }
    }
}
