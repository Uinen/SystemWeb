using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Helpers;
using Boilerplate.Web.Mvc;
using SystemWeb.Services;
using NWebsec.Csp;
using static System.Web.Mvc.DependencyResolver;
using Autofac;
using SystemWeb.DI.Autofac.Modules;
using MvcSiteMapProvider.Loader;
using MvcSiteMapProvider.Xml;
using System.Web.Hosting;
using MvcSiteMapProvider.Web.Mvc;

namespace SystemWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ConfigureViewEngines();
            ConfigureAntiForgeryTokens();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Application["Version"] = $"{version.Major}.{version.Minor}.{version.Build}";
            // Create a container builder (typically part of your DI setup already)
            var builder = new ContainerBuilder();

            // Register modules
            builder.RegisterModule(new MvcSiteMapProviderModule()); // Required
            builder.RegisterModule(new MvcModule()); // Required by MVC. Typically already part of your setup (double check the contents of the module).

            // Create the DI container (typically part of your config already)
            var container = builder.Build();

            // Setup global sitemap loader (required)
            MvcSiteMapProvider.SiteMaps.Loader = container.Resolve<ISiteMapLoader>();

            // Check all configured .sitemap files to ensure they follow the XSD for MvcSiteMapProvider (optional)
            var validator = container.Resolve<ISiteMapXmlValidator>();
            validator.ValidateXml(HostingEnvironment.MapPath("~/Mvc.sitemap"));

            // Register the Sitemaps routes for search engines (optional)
            XmlSiteMapController.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Handles the Content Security Policy (CSP) violation errors. For more information see FilterConfig.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CspViolationReportEventArgs"/> instance containing the event data.</param>
        protected void NWebsecHttpHeaderSecurityModule_CspViolationReported(object sender, CspViolationReportEventArgs e)
        {
            // Log the Content Security Policy (CSP) violation.
            var violationReport = e.ViolationReport;
            var reportDetails = violationReport.Details;
            var violationReportString =
                $"UserAgent:<{violationReport.UserAgent}>\r\nBlockedUri:<{reportDetails.BlockedUri}>\r\nColumnNumber:<{reportDetails.ColumnNumber}>\r\nDocumentUri:<{reportDetails.DocumentUri}>\r\nEffectiveDirective:<{reportDetails.EffectiveDirective}>\r\nLineNumber:<{reportDetails.LineNumber}>\r\nOriginalPolicy:<{reportDetails.OriginalPolicy}>\r\nReferrer:<{reportDetails.Referrer}>\r\nScriptSample:<{reportDetails.ScriptSample}>\r\nSourceFile:<{reportDetails.SourceFile}>\r\nStatusCode:<{reportDetails.StatusCode}>\r\nViolatedDirective:<{reportDetails.ViolatedDirective}>";
            var exception = new CspViolationException(violationReportString);
            Current.GetService<ILoggingService>().Log(exception);
        }

        /// <summary>
        /// Configures the view engines. By default, Asp.Net MVC includes the Web Forms (WebFormsViewEngine) and 
        /// Razor (RazorViewEngine) view engines that supports both C# (.cshtml) and VB (.vbhtml). You can remove view 
        /// engines you are not using here for better performance and include a custom Razor view engine that only 
        /// supports C#.
        /// </summary>
        private static void ConfigureViewEngines()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new CSharpRazorViewEngine());
        }

        /// <summary>
        /// Configures the anti-forgery tokens. See 
        /// http://www.asp.net/mvc/overview/security/xsrfcsrf-prevention-in-aspnet-mvc-and-web-pages
        /// </summary>
        private static void ConfigureAntiForgeryTokens()
        {
            // Rename the Anti-Forgery cookie from "__RequestVerificationToken" to "f". This adds a little security 
            // through obscurity and also saves sending a few characters over the wire. Sadly there is no way to change 
            // the form input name which is hard coded in the @Html.AntiForgeryToken helper and the 
            // ValidationAntiforgeryTokenAttribute to  __RequestVerificationToken.
            // <input name="__RequestVerificationToken" type="hidden" value="..." />
            AntiForgeryConfig.CookieName = "f";

            // If you have enabled SSL. Uncomment this line to ensure that the Anti-Forgery 
            // cookie requires SSL to be sent across the wire. 
            AntiForgeryConfig.RequireSsl = false;
        }
    }
}
