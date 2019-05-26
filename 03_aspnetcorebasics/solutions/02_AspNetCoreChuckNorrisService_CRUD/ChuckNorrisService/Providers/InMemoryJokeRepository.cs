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
    public class InMemoryJokeRepository : IJokeRepository
    {
        private static readonly Random random = new Random();
        private static readonly string JokeFilePath = Path.Combine("Data", "jokes.json");
        private List<Joke> _jokes;

        public Task<Joke> Add(Joke joke)
        {
            throw new NotImplementedException();
        }

        public Task<Joke> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Joke> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Joke> GetRandomJokeAsync()
        {
            await InitIfNecessary();
            return _jokes[random.Next(0, _jokes.Count + 1)];
        }

        public Task<Joke> Update(Joke joke)
        {
            throw new NotImplementedException();
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
            var jokeDtos = JsonConvert.DeserializeObject<List<JokeDto>>(rawJson);
            _jokes = GetJokesFromDtos(jokeDtos);
        }

        private List<Joke> GetJokesFromDtos(List<JokeDto> jokeDtos)
        {
            var allCategoryNames = jokeDtos.SelectMany(jokeDto => jokeDto.Category).Distinct();
            var categoryMap = allCategoryNames.Select(JokeCategory.FromName).ToDictionary(k => k.Name);

            return jokeDtos.Select(dto => new Joke
            {
                Id = dto.Id,
                JokeText = dto.JokeText,
                Categories = dto.Category?.Select(cat => categoryMap[cat]).ToArray() ?? Array.Empty<JokeCategory>()
            }).ToList();
        }
    }
}
