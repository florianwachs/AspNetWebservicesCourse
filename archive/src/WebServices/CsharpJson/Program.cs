using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Web.Script.Serialization;

namespace CsharpJson
{
    public enum JokeCategories
    {
        Nerdy,
        Classic,
    }
    public class Joke
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public JokeCategories[] Categories { get; set; }
        public int Rating { get; set; }
        public DateTime AddedToLibrary { get; set; }

        [JsonIgnore]
        public string TopSecretNotice { get; set; }
    }

    class Program
    {

        static void Main(string[] args)
        {
            var joke = new Joke
            {
                Id = 1,
                Text = "Chuck Norris can make a class that is both abstract and final.",
                Categories = new[] { JokeCategories.Nerdy, JokeCategories.Classic },
                Rating = 5,
                AddedToLibrary = DateTime.Now,
                TopSecretNotice = "Chuck worked for the NSA"
            };

            UseJavaScriptSerializer(joke);
            UseJsonDotNetWithoutSerializerOptions(joke);
            UseJsonDotNetWithSerializerOptions(joke);
        }

        private static void UseJavaScriptSerializer(Joke joke)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string jokeAsJson = serializer.Serialize(joke);
            Console.WriteLine(jokeAsJson);

            Joke parsedJoke = serializer.Deserialize<Joke>(jokeAsJson);
        }
        private static void UseJsonDotNetWithoutSerializerOptions(Joke joke)
        {
            string jokeAsJson = JsonConvert.SerializeObject(joke);
            Console.WriteLine(jokeAsJson);

            Joke parsedJoke = JsonConvert.DeserializeObject<Joke>(jokeAsJson);
        }

        private static void UseJsonDotNetWithSerializerOptions(Joke joke)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                // CamelCase für alle Properties
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            // Enums als String und nicht den numerischen Wert serialisieren
            settings.Converters.Add(new StringEnumConverter(camelCaseText: true));

            string jokeAsJson = JsonConvert.SerializeObject(joke, settings);
            Console.WriteLine(jokeAsJson);
            Joke parsedJoke = JsonConvert.DeserializeObject<Joke>(jokeAsJson);
        }
    }
}
