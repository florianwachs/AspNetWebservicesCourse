using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Automapper.Models;
using WebAPI.Automapper.ViewModels;

namespace WebAPI.Automapper.Infrastructure
{
    public static class AutomapperConfig
    {
        public static void RegisterMappings(IMapperConfigurationExpression config)
        {
            config.CreateMap<Book, BookViewModel>()
                .ForMember(target => target.FormattedPrice, opt => opt.ResolveUsing(src => src.Price.ToString("C")))
                .ForMember(target => target.ReleaseDate, opt => opt.ResolveUsing(src => src.ReleaseDate.HasValue ? src.ReleaseDate.Value.ToShortDateString() : "???"));
        }
    }
}