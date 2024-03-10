﻿using ChuckNorrisService.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChuckNorrisService.Providers
{
    public class InMemoryJokeRepository : IJokeRepository
    {
        private static readonly Random random = new Random();
        private static readonly string JokeFilePath = Path.Combine("Data", "jokes.json");
        private ConcurrentDictionary<string, Joke> _jokes;

        public InMemoryJokeRepository()
        {
            Init();
        }

        public Task<Joke> Add(Joke joke)
        {
            EnsureId(joke);
            _jokes.AddOrUpdate(joke.Id, joke, (id, existingJoke) => joke);
            return Task.FromResult(joke);
        }

        private void EnsureId(Joke joke)
        {
            joke.Id = string.IsNullOrWhiteSpace(joke.Id) ? Guid.NewGuid().ToString() : joke.Id;
        }

        public Task Delete(string id)
        {
            _jokes.Remove(id, out _);
            return Task.CompletedTask;
        }

        public Task<Joke> GetById(string id)
        {
            var result = _jokes.TryGetValue(id, out var joke) ? joke : default;
            return Task.FromResult(result);
        }

        public Task<Joke> GetRandomJoke()
        {
            return Task.FromResult(_jokes.Values.ToList()[random.Next(0, _jokes.Count + 1)]);
        }

        public Task<Joke> Update(Joke joke)
        {
            if (string.IsNullOrWhiteSpace(joke.Id))
                throw new InvalidOperationException("no joke.Id provided");

            _jokes.AddOrUpdate(joke.Id, joke, (existingKey, existingJoke) => joke);
            return Task.FromResult(joke);
        }

        private void Init()
        {
            if (!File.Exists(JokeFilePath))
            {
                throw new InvalidOperationException($"no jokes file located in {JokeFilePath}");
            }

            var rawJson = File.ReadAllText(JokeFilePath);
            var jokeDtos = JsonSerializer.Deserialize<List<JokeDto>>(rawJson);
            _jokes = new ConcurrentDictionary<string, Joke>(GetJokesFromDtos(jokeDtos).ToDictionary(k => k.Id));
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
