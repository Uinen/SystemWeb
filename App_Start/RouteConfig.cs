using System.Web.Mvc;
using System.Web.Routing;

namespace SystemWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.AppendTrailingSlash = true;
            routes.LowercaseUrls = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Contenuti/{*pathInfo}");
            routes.IgnoreRoute("Scripts/{*pathInfo}");
            routes.IgnoreRoute("Error/Forbidden.html");
            routes.IgnoreRoute("Error/GatewayTimeout.html");
            routes.IgnoreRoute("Error/ServiceUnavailable.html");
            routes.IgnoreRoute("humans.txt");

            routes.MapMvcAttributeRoutes();


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                "UserPvErogatori", 
                "PvErogatori", new { controller = "User", action = "PvErogatori", dateFrom = UrlParameter.Optional, dateTo = UrlParameter.Optional }
                );
        }
    }
}
