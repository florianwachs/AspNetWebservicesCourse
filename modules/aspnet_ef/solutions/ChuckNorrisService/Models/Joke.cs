namespace ChuckNorrisService.Models
{
    public class Joke
    {
        public string Id { get; set; } = "";
        public string Value { get; set; }= "";
        public List<JokeCategory> Categories { get; set; } = new();
    }
}
