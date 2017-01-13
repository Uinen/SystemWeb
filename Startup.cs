using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SystemWeb.Startup))]
namespace SystemWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            var CurrentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string VersionNumber = CurrentAssembly.GetName().Version.ToString();
        }
    }
}
