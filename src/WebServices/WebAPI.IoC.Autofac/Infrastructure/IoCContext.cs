using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace WebAPI.IoC.Autofac.Infrastructure
{
    public static class IoCContext
    {
        private static bool _isInitialized;
        private static IContainer _container;
        public static IContainer Container
        {
            get
            {
                if (!_isInitialized)
                    throw new InvalidOperationException($"Initialization with {nameof(IoCContext)}.{nameof(Initialize)} is required.");
                return _container;
            }
        }

        public static void Initialize(HttpConfiguration httpConfig)
        {
            if (_isInitialized) return;

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterWebApiFilterProvider(httpConfig);

            containerBuilder.RegisterModule(new RepositoriesModule() { InMemory = false });

            _container = containerBuilder.Build();

            _isInitialized = true;
        }
    }

}