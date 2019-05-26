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
        private readonly object lockObj = new object();
        private static readonly Random random = new Random();
        private static readonly string JokeFilePath = Path.Combine("Data", "jokes.json");
        private Dictionary<string, Joke> _jokes;

        public InMemoryJokeRepository()
        {
            Init();
        }

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

        public async Task<Joke> GetRandomJoke()
        {
            return _jokes.Values.ToList()[random.Next(0, _jokes.Count + 1)];
        }

        public Task<Joke> Update(Joke joke)
        {
            throw new NotImplementedException();
        }

        private void Init()
        {
            if (!File.Exists(JokeFilePath))
            {
                throw new InvalidOperationException($"no jokes file located in {JokeFilePath}");
            }

            var rawJson = File.ReadAllText(JokeFilePath);
            var jokeDtos = JsonConvert.DeserializeObject<List<JokeDto>>(rawJson);
            _jokes = GetJokesFromDtos(jokeDtos).ToDictionary(k => k.Id);
        }

        private List<Joke> GetJokesFromDtos(List<JokeDto> jokeDtos)
        {
            var allCategoryNames = jokeDtos.SelectMany(jokeDto => jokeDto.Category).Distinct();
            var categoryMap = allCategoryNames.Select(JokeCategory.FromName).ToDictionary(k => k.Name);

            return jokeDtos.Select(dto => new Joke
            {
                Id = dto.Id,
                JokeText = dto.JokeText,
                Categories = GetMatchingCategoriesOrEmpty(dto)
            }).ToList();

            JokeCategory[] GetMatchingCategoriesOrEmpty(JokeDto dto) => dto.Category?.Select(cat => categoryMap[cat]).ToArray() ?? Array.Empty<JokeCategory>();
        }
    }
}
