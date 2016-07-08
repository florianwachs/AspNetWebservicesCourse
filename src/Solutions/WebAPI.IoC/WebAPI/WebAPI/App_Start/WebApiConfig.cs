using System.Web.Http;
using Autofac.Integration.WebApi;
using WebAPI.IoC.Autofac.Services;

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
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
                );

            IoCContext.Initialize(config);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(IoCContext.Container);
        }
    }
}