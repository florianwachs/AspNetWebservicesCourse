using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.IoC.Autofac.DataAccess;

namespace WebAPI.IoC.Autofac.Infrastructure
{
    public class RepositoriesModule : Module
    {
        public bool InMemory { get; set; } = true;
        protected override void Load(ContainerBuilder builder)
        {
            if (InMemory)
            {
                //builder.RegisterType<InMemoryBookRepository>().As<IBookRepository>().InstancePerRequest();
                builder.Register(context => new InMemoryBookRepository()).As<IBookRepository>().InstancePerRequest();
            }
            else
            {
                builder.Register(context => new BookDbContext()).AsSelf().InstancePerRequest();
                builder.Register(context => new EFBookRepository(context.Resolve<BookDbContext>())).As<IBookRepository>().InstancePerRequest();
            }
        }
    }
}