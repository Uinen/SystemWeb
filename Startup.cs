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
            var CurrentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string VersionNumber = CurrentAssembly.GetName().Version.ToString();
        }
    }
}
