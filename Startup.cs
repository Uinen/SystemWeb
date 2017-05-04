using System.Reflection;
using Microsoft.Owin;
using Owin;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(SystemWeb.Startup))]
namespace SystemWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureContainer(app);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            var currentAssembly = Assembly.GetExecutingAssembly();
            var versionNumber = currentAssembly.GetName().Version.ToString();
        }
    }
}
