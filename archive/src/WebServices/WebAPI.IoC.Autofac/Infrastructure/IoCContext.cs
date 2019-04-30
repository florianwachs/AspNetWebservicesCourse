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

            // Neuen Container für die Registrierung der Dependencies erzeugen
            var containerBuilder = new ContainerBuilder();

            // Alle WebApi-Controller aus dem aktuellen Projekt dem Container hinzufügen
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Spezial-Fall Filter, da Standard-WebApi-Filter nicht DI-freundlich gebaut sind
            containerBuilder.RegisterWebApiFilterProvider(httpConfig);

            // Module helfen, Registrierungen auf wiederverwenbare Art und Weise durchzuführen
            // Auch die Konfiguration per XML ist möglich
            containerBuilder.RegisterModule(new RepositoriesModule() { InMemory = false });
            containerBuilder.RegisterModule(new CommonServicesModule());
            // Nach alle Registrierungen werde der Container zusammengebaut
            _container = containerBuilder.Build();

            _isInitialized = true;
        }
    }

}