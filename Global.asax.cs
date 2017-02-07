using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SystemWeb.ViewModels;

namespace SystemWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders.Add(typeof(IRegistrationViewModel), new RegistrationViewModelBinder());
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Application["Version"] = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build/*, version.Revision */);
        }
    }
}
