﻿using System.Threading.Tasks;

namespace AspNetMediatR.Api.Domain.Jokes.Models
{
    public interface IJokeRepository
    {
        Task<Joke> GetById(string id);
        Task<Joke> Add(Joke joke);
        Task<Joke> Update(Joke joke);
        Task Delete(string id);
        Task<Joke> GetRandomJoke();
    }
}