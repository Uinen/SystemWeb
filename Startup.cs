﻿using System.Reflection;
using Microsoft.Owin;
using Owin;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(GestioniDirette.Startup))]
namespace GestioniDirette
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Strict-Transport-Security - Adds the Strict-Transport-Security HTTP header to responses.
            //      This HTTP header is only relevant if you are using TLS. It ensures that content is loaded over 
            //      HTTPS and refuses to connect in case of certificate errors and warnings. NWebSec currently does 
            //      not support an MVC filter that can be applied globally. Instead we can use Owin (Using the 
            //      added NWebSec.Owin NuGet package) to apply it.
            //      Note: Including subdomains and a minimum maxage of 18 weeks is required for preloading.
            //      Note: You can view preloaded HSTS domains in Chrome here: chrome://net-internals/#hsts
            //      https://developer.mozilla.org/en-US/docs/Web/Security/HTTP_strict_transport_security
            //      http://www.troyhunt.com/2015/06/understanding-http-strict-transport.html
            //app.UseHsts(options => options.MaxAge(days: 18 * 7).IncludeSubdomains().Preload());

            // Public-Key-Pins - Adds the Public-Key-Pins HTTP header to responses.
            //      This HTTP header is only relevant if you are using TLS. It stops man-in-the-middle attacks by 
            //      telling browsers exactly which TLS certificate you expect.
            //      Note: The current specification requires including a second pin for a backup key which isn't yet 
            //      used in production. This allows for changing the server's public key without breaking accessibility 
            //      for clients that have already noted the pins. This is important for example when the former key 
            //      gets compromised. 
            //      Note: You can use the ReportUri option to provide browsers a URL to post JSON violations of the 
            //      HPKP policy. Note that the report URI must not be this site as a violation would mean that the site
            //      is blocked. You must use a separate domain using HTTPS to report to. Consider using this service:
            //      https://report-uri.io/ for this purpose.
            //      Note: You can change UseHpkp to UseHpkpReportOnly to stop browsers blocking anything but continue
            //      reporting any violations.
            //      See https://developer.mozilla.org/en-US/docs/Web/Security/Public_Key_Pinning
            //      and https://scotthelme.co.uk/hpkp-http-public-key-pinning/
            //      1Key:da088409868dd5b4874d3d2b0e2e6bef
            //      2Key:2b2e6eead975366c148a6edba37c8c07

            /*app.UseHpkp(options => options
                 .Sha256Pins(
                     "0RTuqaNYK0FvRSS8N4/KyrMX+XCYY7XcAzcqrXRA/7w=",
                     "32/ZoKy4nAodOZejd23CjLQ28Z2rBNPA1gNX23jqfvc=")
                 .MaxAge(days: 18 * 7)
                 .IncludeSubdomains());*/

            // Content-Security-Policy:upgrade-insecure-requests - Adds the 'upgrade-insecure-requests' directive to
            //      the Content-Security-Policy HTTP header. This is only relevant if you are using HTTPS. Any objects
            //      on the page using HTTP is automatically upgraded to HTTPS.
            //      See https://scotthelme.co.uk/migrating-from-http-to-https-ease-the-pain-with-csp-and-hsts/
            //      and http://www.w3.org/TR/upgrade-insecure-requests/
            //app.UseCsp(x => x.UpgradeInsecureRequests());
            ConfigureContainer(app);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            var currentAssembly = Assembly.GetExecutingAssembly();
            var versionNumber = currentAssembly.GetName().Version.ToString();
        }
    }
}
