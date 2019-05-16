using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPI.IoC.Autofac.Infrastructure;

namespace WebAPI.IoC.Autofac
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Dependency Injection Configuration
            IoCContext.Initialize(config);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(IoCContext.Container);
        }
    }
}
