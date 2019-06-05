using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreSwagger.DataAccess;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AspNetCoreSwagger.Models
{
    public static class JokeDbSeeder
    {
        private static readonly Random rnd = new Random();
        private static readonly string JokeFilePath = Path.Combine("Data", "jokes.json");

        private static readonly List<Author> DummyAuthors = new List<Author>
        {
            Author.NewFrom("Chuck", "Norris"),
            Author.NewFrom("Jason", "Bourne"),
            Author.NewFrom("Jean Claude", "Van Damme"),
            Author.NewFrom("Arnold", "Schwarzenegger")
        };

        public static async Task Seed(JokeDbContext dbContext)
        {
            if (await dbContext.Jokes.AnyAsync())
                return;

            var jokes = GetJokes();
            await dbContext.Jokes.AddRangeAsync(jokes);
            await dbContext.SaveChangesAsync();
        }

        private static List<Joke> GetJokes()
        {
            var rawJson = File.ReadAllText(JokeFilePath);
            var jokeDtos = JsonConvert.DeserializeObject<List<JokeDto>>(rawJson);
            var jokes = GetJokesFromDtos(jokeDtos);
            return jokes;
        }

        private static List<Joke> GetJokesFromDtos(List<JokeDto> jokeDtos)
        {
            var allCategoryNames = jokeDtos.SelectMany(jokeDto => jokeDto.Category).Distinct();
            var categoryMap = allCategoryNames.Select(JokeCategory.FromName).ToDictionary(k => k.Name);

            return jokeDtos.Select(dto => new Joke
            {
                Id = dto.Id,
                JokeText = dto.JokeText,
                Categories = GetMatchingCategories(dto),
                Author = DummyAuthors[rnd.Next(0, DummyAuthors.Count)]
            }).ToList();

            List<JokeCategory> GetMatchingCategories(JokeDto dto) =>
                dto.Category?.Select(cat => categoryMap[cat]).ToList();
        }
    }
}