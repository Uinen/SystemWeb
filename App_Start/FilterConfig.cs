using System.Web.Mvc;
using System.Web.Routing;
using NWebsec.Mvc.HttpHeaders;
using Boilerplate.Web.Mvc.Filters;
using NWebsec.Mvc.HttpHeaders.Csp;
using GestioniDirette.Service.Static;
using System;

namespace GestioniDirette
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            AddSearchEngineOptimizationFilters(filters);
            AddSecurityFilters(filters);
            AddContentSecurityPolicyFilters(filters);
        }

        /// <summary>
        /// Adds filters which help improve search engine optimization (SEO).
        /// </summary>
        private static void AddSearchEngineOptimizationFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RedirectToCanonicalUrlAttribute(
                RouteTable.Routes.AppendTrailingSlash,
                RouteTable.Routes.LowercaseUrls));
        }
        /// <summary>
        /// Add filters to improve security.
        /// </summary>
        
        private static void AddSecurityFilters(GlobalFilterCollection filters)
        {
            // Require HTTPS to be used across the whole site. System.Web.Mvc.RequireHttpsAttribute performs a
            // 302 Temporary redirect from a HTTP URL to a HTTPS URL. This filter gives you the option to perform a
            // 301 Permanent redirect or a 302 temporary redirect. You should perform a 301 permanent redirect if the
            // page can only ever be accessed by HTTPS and a 302 temporary redirect if the page can be accessed over
            // HTTP or HTTPS.
            filters.Add(new RedirectToHttpsAttribute(true));

            // Several NWebsec Security Filters are added here. See
            // http://rehansaeed.com/nwebsec-asp-net-mvc-security-through-http-headers/ and
            // http://www.dotnetnoob.com/2012/09/security-through-http-response-headers.html and
            // https://github.com/NWebsec/NWebsec/wiki for more information.
            // Note: All of these filters can be applied to individual controllers and actions and indeed
            // some of them only make sense when applied to a controller or action instead of globally here.

            // Cache-Control: no-cache, no-store, must-revalidate
            // Expires: -1
            // Pragma: no-cache
            //      Specifies whether appropriate headers to prevent browser caching should be set in the HTTP response.
            //      Do not apply this attribute here globally, use it sparingly to disable caching. A good place to use
            //      this would be on a page where you want to post back credit card information because caching credit
            //      card information could be a security risk.
            // filters.Add(new SetNoCacheHttpHeadersAttribute());

            // X-Robots-Tag - Adds the X-Robots-Tag HTTP header. Disable robots from any action or controller this
            //                attribute is applied to.
            // filters.Add(new XRobotsTagAttribute() { NoIndex = true, NoFollow = true });

            // X-Content-Type-Options - Adds the X-Content-Type-Options HTTP header. Stop IE9 and below from sniffing
            //                          files and overriding the Content-Type header (MIME type).
            //filters.Add(new XContentTypeOptionsAttribute());

            // X-Download-Options - Adds the X-Download-Options HTTP header. When users save the page, stops them from
            //                      opening it and forces a save and manual open.
            filters.Add(new XDownloadOptionsAttribute());

            // X-Frame-Options - Adds the X-Frame-Options HTTP header. Stop clickjacking by stopping the page from
            //                   opening in an iframe or only allowing it from the same origin.
            //      SameOrigin - Specifies that the X-Frame-Options header should be set in the HTTP response,
            //                   instructing the browser to display the page when it is loaded in an iframe - but only
            //                   if the iframe is from the same origin as the page.
            //      Deny - Specifies that the X-Frame-Options header should be set in the HTTP response, instructing
            //             the browser to not display the page when it is loaded in an iframe.
            //      Disabled - Specifies that the X-Frame-Options header should not be set in the HTTP response.
            filters.Add(
                new XFrameOptionsAttribute()
                {
                    Policy = XFrameOptionsPolicy.SameOrigin
                });
        }

        /// <summary>
        /// Adds the Content-Security-Policy (CSP) and/or Content-Security-Policy-Report-Only HTTP headers. This
        /// creates a white-list from where various content in a web page can be loaded from. (See
        /// <see cref="http://rehansaeed.com/content-security-policy-for-asp-net-mvc/"/> and
        /// <see cref="https://developer.mozilla.org/en-US/docs/Web/Security/CSP/CSP_policy_directives"/>
        /// <see cref="https://github.com/NWebsec/NWebsec/wiki"/> and for more information).
        /// Note: If you are using the 'Browser Link' feature of the Webs Essentials Visual Studio extension, it will
        /// not work if you enable CSP (See
        /// <see cref="http://webessentials.uservoice.com/forums/140520-general/suggestions/6665824-browser-link-support-for-content-security-policy"/>).
        /// Note: All of these filters can be applied to individual controllers and actions e.g. If an action requires
        /// access to content from YouTube.com, then you can add the following attribute to the action:
        /// [CspFrameSrc(CustomSources = "*.youtube.com")].
        /// </summary>
        private static void AddContentSecurityPolicyFilters(GlobalFilterCollection filters)
        {
            // Content-Security-Policy - Add the Content-Security-Policy HTTP header to enable Content-Security-Policy.
            filters.Add(new CspAttribute());
            // OR
            // Content-Security-Policy-Report-Only - Add the Content-Security-Policy-Report-Only HTTP header to enable
            //      logging of violations without blocking them. This is good for testing CSP without enabling it. To
            //      make use of this attribute, rename all the attributes below to their ReportOnlyAttribute versions
            //      e.g. CspDefaultSrcAttribute becomes CspDefaultSrcReportOnlyAttribute.
            // filters.Add(new CspReportOnlyAttribute());


            // Enables logging of CSP violations. See the NWebsecHttpHeaderSecurityModule_CspViolationReported method
            // in Global.asax.cs to see where they are logged.
            filters.Add(new CspReportUriAttribute() { EnableBuiltinHandler = true });
        }
    }
}
