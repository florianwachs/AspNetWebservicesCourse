using Autofac;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Automapper.DataAccess;

namespace WebAPI.Automapper.Infrastructure
{
    public class CommonServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                AutomapperConfig.RegisterMappings(config);
            });

            IMapper mapper = mapperConfig.CreateMapper();

            builder.RegisterInstance(mapper).As<IMapper>();

            builder.Register(context => new InMemoryBookRepository()).As<IBookRepository>().SingleInstance();
        }
    }
}