using ChuckNorrisService.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChuckNorrisService.Models
{
    public static class JokeDbSeeder
    {
        private static readonly string JokeFilePath = Path.Combine("Data", "jokes.json");

        public static async Task Seed(JokeDbContext dbContext)
        {
            if (await dbContext.Jokes.AnyAsync())
            {
                return;
            }

            List<Joke> jokes = GetJokes();
            await dbContext.Jokes.AddRangeAsync(jokes);
            await dbContext.SaveChangesAsync();
        }

        private static List<Joke> GetJokes()
        {
            string rawJson = File.ReadAllText(JokeFilePath);
            List<JokeDto> jokeDtos = JsonSerializer.Deserialize<List<JokeDto>>(rawJson);
            List<Joke> jokes = GetJokesFromDtos(jokeDtos);
            return jokes;
        }

        private static List<Joke> GetJokesFromDtos(List<JokeDto> jokeDtos)
        {
            IEnumerable<string> allCategoryNames = jokeDtos.SelectMany(jokeDto => jokeDto.Category).Distinct();
            Dictionary<string, JokeCategory> categoryMap = allCategoryNames.Select(JokeCategory.FromName).ToDictionary(k => k.Name);

            return jokeDtos.Select(dto => new Joke
            {
                Id = dto.Id,
                JokeText = dto.JokeText,
                Categories = GetMatchingCategories(dto)
            }).ToList();

            List<JokeCategory> GetMatchingCategories(JokeDto dto)
            {
                return dto.Category?.Select(cat => categoryMap[cat]).ToList();
            }
        }
    }
}
