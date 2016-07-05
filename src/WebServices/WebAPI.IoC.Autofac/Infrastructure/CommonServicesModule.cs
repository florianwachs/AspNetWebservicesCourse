using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.IoC.Autofac.Infrastructure
{
    public class CommonServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new DefaultTimeService()).As<ITimeService>().SingleInstance();
        }
    }
}