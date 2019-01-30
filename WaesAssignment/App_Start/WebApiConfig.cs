using System.Web.Http;
using Unity;
using Unity.Lifetime;
using WaesAssignment.DataServices;
using WaesAssignment.Services;

namespace WaesAssignment
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();
            container.RegisterType<IDiffService, DiffService>(new HierarchicalLifetimeManager());
            container.RegisterType<IMemoryCacheDataService, MemoryCacheDataService>(new SingletonLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
