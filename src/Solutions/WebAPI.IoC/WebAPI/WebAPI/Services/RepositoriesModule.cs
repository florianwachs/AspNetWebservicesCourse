using Autofac;
using WebAPI.IoC.Autofac.Models;
using WebAPI.IoC.Autofac.UnitOfWork;

namespace WebAPI.IoC.Autofac.Services
{
    public class RepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new PersonsRepository())
                .As<IRepository<Person>>()
                .InstancePerRequest();
        }
    }
}