using AspNetCoreAutomapper.Models;
using AutoMapper;

namespace AspNetCoreAutomapper.Api.Authors
{
    public class AuthorVm
    {
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public int Age { get; set; }
    }

    public class AuthorMappingProfile : Profile
    {
        public AuthorMappingProfile()
        {
            CreateMap<Author, AuthorVm>()
                .ForMember(m => m.FullName, map => map.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(m => m.DisplayName, map => map.MapFrom(s => $"{s.FirstName} {s.LastName} ({s.Age})"));
        }
    }
}