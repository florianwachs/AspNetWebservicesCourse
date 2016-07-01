using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;

namespace WebAPI.IoC.Autofac.Services
{
    public static class IoCContext
    {
        private static bool _isInitialized;

        public static IContainer Container { get; private set; }

        public static void Initialize(HttpConfiguration httpConfig)
        {
            if (_isInitialized) return;

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterWebApiFilterProvider(httpConfig);

            containerBuilder.RegisterModule(new RepositoriesModule());

            Container = containerBuilder.Build();

            _isInitialized = true;
        }
    }
}