using ChuckNorrisService.Client;
using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ChuckNorrisService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Aufgabe 1
            await OutputRandomJokeFromApiAsync();

            // Aufgabe 2
            await SaveJokesToFileAsync();

            // Aufgabe 3
            await OutputRandomJokeFromFileSystemAsync();
        }

        private static async Task OutputRandomJokeFromApiAsync()
        {
            var api = new ChuckNorrisApi();
            var randomJoke = await api.GetRandomJokeFromCategory(JokeCategories.Dev);
            Console.WriteLine(randomJoke.Value);
        }

        private static async Task SaveJokesToFileAsync()
        {
            var api = new ChuckNorrisApi();
            var result = await api.GetRandomJokesFromCategory(JokeCategories.Dev, 10);

            SaveJokes(result);

            void SaveJokes(ChuckNorrisApi.ChuckNorrisJoke[] jokesToSerialize)
            {
                // TODO: Nur für Vorlesungszwecke! Das referenzieren von Strings erzeugt
                // eine Kopie, problematisch bei großen strings (LOH)
                // Mit .net core 3.0 wird es performantere Möglichkeiten zur
                // Serialisierung und Deserialisierung geben.

                var raw = JsonConvert.SerializeObject(jokesToSerialize);
                File.WriteAllText($"jokes_{DateTime.Now.Ticks}.json", raw);
            }
        }

        private static async Task OutputRandomJokeFromFileSystemAsync()
        {
            var provider = new FileSystemJokeProvider();
            var joke = await provider.GetRandomJokeAsync();
            Console.WriteLine(joke.Value);
        }
    }
}
