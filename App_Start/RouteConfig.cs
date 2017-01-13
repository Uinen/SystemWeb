using System.Web.Mvc;
using System.Web.Routing;

namespace SystemWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            /*
            routes.IgnoreRoute("{folder}/{*pathInfo}", new { folder = "Contenuti" });
            routes.IgnoreRoute("{folder}/{*pathInfo}", new { folder = "MainFont" });
            routes.IgnoreRoute("{folder}/{*pathInfo}", new { folder = "fonts" });
            */
            routes.MapRoute("UserCarico", "Carico", new { controller = "User", action = "Carico", dateFrom = UrlParameter.Optional, dateTo = UrlParameter.Optional });
            routes.MapRoute("UserPvErogatori", "PvErogatori", new { controller = "User", action = "PvErogatori", dateFrom = UrlParameter.Optional, dateTo = UrlParameter.Optional });
        }
    }
}
