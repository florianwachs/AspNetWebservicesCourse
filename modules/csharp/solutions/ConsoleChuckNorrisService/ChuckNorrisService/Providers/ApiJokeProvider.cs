using ChuckNorrisService.Client;
using ChuckNorrisService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChuckNorrisService.Providers
{
    public class ApiJokeProvider : IJokeProvider
    {
        private readonly ChuckNorrisApi _api;

        public ApiJokeProvider()
        {
            _api = new ChuckNorrisApi();
        }

        public async Task<Joke> GetRandomJokeAsync()
        {
            return (await _api.GetRandomJokeFromCategory(JokeCategories.Dev)).AsJoke();
        }
    }
}
