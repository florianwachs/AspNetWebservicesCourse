using ChuckNorrisService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuckNorrisService.Providers
{
    public class FileSystemJokeProvider : IJokeProvider
    {
        private static readonly Random random = new Random();
        private static readonly string JokeFilePath = Path.Combine("Data", "jokes.json");
        private List<Joke> _jokes;
        public async Task<Joke> GetRandomJokeAsync()
        {
            await InitIfNecessary();
            return _jokes[random.Next(0, _jokes.Count + 1)];
        }

        private async Task InitIfNecessary()
        {
            if (_jokes?.Any() == true)
                return;

            if (!File.Exists(JokeFilePath))
            {
                throw new InvalidOperationException($"no jokes file located in {JokeFilePath}");
            }

            var rawJson = await File.ReadAllTextAsync(JokeFilePath);
            _jokes = JsonConvert.DeserializeObject<List<Joke>>(rawJson);
        }
    }
}
