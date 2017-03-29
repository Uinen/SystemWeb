using System.Web.Mvc;
using System.Web.Routing;

namespace SystemWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute("UserIndex", "Index", new { controller = "User", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute("UserCarico", "Carico", new { controller = "User", action = "Carico", dateFrom = UrlParameter.Optional, dateTo = UrlParameter.Optional });
            routes.MapRoute("UserPvErogatori", "PvErogatori", new { controller = "User", action = "PvErogatori", dateFrom = UrlParameter.Optional, dateTo = UrlParameter.Optional });
        }
    }
}
