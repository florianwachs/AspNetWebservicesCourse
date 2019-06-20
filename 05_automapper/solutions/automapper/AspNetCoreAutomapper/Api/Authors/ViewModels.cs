using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCoreAutomapper.Models;
using AutoMapper;

namespace AspNetCoreAutomapper.Api.Authors
{
    public class AuthorVm
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public int Age { get; set; }
    }

    public class AuthorWithJokesVm
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public int Age { get; set; }
        public IEnumerable<string> Jokes { get; set; }
    }
    
    public class AuthorEditVm
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class AuthorMappingProfile : Profile
    {
        public AuthorMappingProfile()
        {
            CreateMap<Author, AuthorVm>()
                .ForMember(m => m.FullName, map => map.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(m => m.DisplayName, map => map.MapFrom(s => $"{s.FirstName} {s.LastName} ({s.Age})"));

            CreateMap<Author, AuthorWithJokesVm>()
                .ForMember(m => m.FullName, map => map.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(m => m.DisplayName, map => map.MapFrom(s => $"{s.FirstName} {s.LastName} ({s.Age})"))
                .ForMember(m => m.Jokes,
                    map => map.MapFrom(s => (s.Jokes ?? Array.Empty<Joke>()).Select(j => j.JokeText)));


            CreateMap<AuthorEditVm, Author>();
        }
    }
}