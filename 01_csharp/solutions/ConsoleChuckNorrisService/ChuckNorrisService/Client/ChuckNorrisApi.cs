using ChuckNorrisService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChuckNorrisService.Client
{
    public class ChuckNorrisApi
    {
        private const int MaxJokesPerRequest = 50;
        private static readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("https://api.chucknorris.io/jokes/random") };

        public async Task<ChuckNorrisJoke> GetRandomJokeFromCategory(JokeCategories category)
        {
            HttpResponseMessage result = await _client.GetAsync($"?category={category.ToApiCategoryParameter()}");
            result.EnsureSuccessStatusCode();

            ChuckNorrisJoke joke = await result.Content.ReadAsAsync<ChuckNorrisJoke>();

            return joke;
        }

        public async Task<ChuckNorrisJoke[]> GetRandomJokesFromCategory(JokeCategories category, int maxJokes)
        {
            maxJokes = Math.Clamp(maxJokes, 1, MaxJokesPerRequest);
            ChuckNorrisJoke[] result = await Task.WhenAll(Enumerable.Range(0, maxJokes).Select(_ => GetRandomJokeFromCategory(category)));
            HashSet<string> uniqueIds = new HashSet<string>();
            return result.Where(joke => uniqueIds.Add(joke.Id)).ToArray();
        }

        public class ChuckNorrisJoke
        {
            public string[] Category { get; set; }

            public Uri IconUrl { get; set; }

            public string Id { get; set; }

            public Uri Url { get; set; }

            public string Value { get; set; }

            public Joke AsJoke()
            {
                return new Joke { Id = Id, Value = Value };
            }
        }
    }
}
