namespace ChuckNorrisService.Models
{
    public class JokeCategory        
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<Joke> Jokes { get; set; }
    }
}