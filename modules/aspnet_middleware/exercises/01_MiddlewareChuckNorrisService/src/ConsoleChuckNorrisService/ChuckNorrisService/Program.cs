﻿using ChuckNorrisService.Client;
using ChuckNorrisService.Models;
using ChuckNorrisService.Providers;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChuckNorrisService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await PrintJoke(new DummyJokeProvider());
            await PrintJoke(new FileSystemJokeProvider());
            await PrintJoke(new ApiJokeProvider());
        }

        private static async Task PrintJoke(IJokeProvider jokeProvider)
        {
            var joke = await jokeProvider.GetRandomJokeAsync();
            Console.WriteLine(joke.Value);
        }

        private static async Task SaveJokesToFile()
        {
            var api = new ChuckNorrisApi();
            var result = await api.GetRandomJokesFromCategory(JokeCategories.Dev, 10);

            SaveJokes(result);

            void SaveJokes(ChuckNorrisApi.ChuckNorrisJoke[] jokesToSerialize)
            {
                // TODO: Nur für Vorlesungszwecke! Das referenzieren von Strings erzeugt
                // eine Kopie, problematisch bei großen strings (LOH)
                // Mit .net core 3.1 gibt es performantere Möglichkeiten zur
                // Serialisierung und Deserialisierung über Streams.

                var raw = JsonSerializer.Serialize(jokesToSerialize);
                File.WriteAllText($"jokes_{DateTime.Now.Ticks}.json", raw);
            }
        }
    }
}
