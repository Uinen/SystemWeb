using System.Web.Optimization;
using SystemWeb.Helpers;

namespace SystemWeb
{
    public class BundleConfig
    {
        // Per ulteriori informazioni sulla creazione di bundle, visitare http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
            bundles.UseCdn = true;
            AddCss(bundles);
            AddJavaScript(bundles);
        }
        private static void AddCss(BundleCollection bundles)
        {
            CssRewriteUrlTransform rewrite = new CssRewriteUrlTransform();

            #region Style
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                "~/Contenuti/bootstrap.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/bootstrap-theme").Include(
                "~/Contenuti/bootstrap-theme.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/ejBootstrap-Themes").Include(
                "~/Contenuti/ej/web/bootstrap-theme/ej.web.all.min.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/ejWidgets-Core").Include(
                "~/Contenuti/ej/web/ej.widgets.core.min", rewrite));

            bundles.Add(new StyleBundle("~/Content/ejTheme").Include(
                "~/Contenuti/ej/web/flat-saffron/ej.theme.min.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/ejResponsive").Include(
                "~/Contenuti/ej/web/responsive-css/ej.responsive.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/ejGridResponsive").Include(
                "~/Contenuti/ej/web/responsive-css/ejgrid.responsive.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/style").Include(
                "~/Contenuti/css/style.css"));

            bundles.Add(new StyleBundle("~/Content/custom").Include(
                "~/Contenuti/colors/custom.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/ionicons").Include(
                "~/Contenuti/css/ionicons.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/animate").Include(
                "~/Contenuti/css/animate.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/owlcarousel").Include(
                "~/Contenuti/css/owl.carousel.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/owlcarousel-theme").Include(
                "~/Contenuti/css/owl.theme.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/pagedlist").Include(
                "~/Contenuti/css/PagedList.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/nivolightbox").Include(
                "~/Contenuti/nivo-lightbox/nivo-lightbox.css", rewrite));

            bundles.Add(new StyleBundle("~/Content/fontawesome", 
                ContentDeliveryNetwork.MaxCdn.FontAwesomeUrl).Include(
                      "~/MainFont/font-awesome.css", rewrite));
            #endregion
        }

        private static void AddJavaScript(BundleCollection bundles)
        {
            #region Script
            bundles.Add(new ScriptBundle("~/bundles/jquery", ContentDeliveryNetwork.Google.JQueryUrl).Include(
                "~/Scripts/jquery-1.10.2.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryeasing").Include(
                "~/Scripts/jquery.easing-{version}.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryglobalize").Include(
                "~/Scripts/jquery.globalize.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jsrender").Include(
                "~/Scripts/jsrender.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jquerybrowser").Include(
                "~/Scripts/jquery.browser.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajaxchimp").Include(
                "~/Scripts/jquery.ajaxchimp.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval",
                ContentDeliveryNetwork.Microsoft.JQueryValidateUrl).Include(
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalunobtrusive",
                ContentDeliveryNetwork.Microsoft.JQueryValidateUnobtrusiveUrl).Include(
                "~/Scripts/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/ej").Include(
                "~/Scripts/ej/web/ej.web.all.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/microsoft").Include(
                "~/Scripts/MicrosoftAjax.js",
                "~/Scripts/MicrosoftMvcAjax.js",
                "~/Scripts/MicrosoftMvcValidation.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap",
                ContentDeliveryNetwork.Microsoft.BootstrapUrl).Include(
                "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr",
                ContentDeliveryNetwork.Microsoft.ModernizrUrl).Include(
                "~/Scripts/modernizr-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/theme").Include(
                "~/Scripts/WOW.js",
                "~/Scripts/owl.carousel.js",
                "~/Scripts/nivo-lightbox.js",
                "~/Scripts/smoothscroll.js",
                "~/Scripts/script.js"));

            bundles.Add(new ScriptBundle("~/bundles/sorting").Include(
                "~/Scripts/sortable.js"));

            bundles.Add(new ScriptBundle("~/bundles/GridOption").Include(
                "~/Scripts/ActionComplete.js",
                "~/Scripts/ejDatePickerOption.js"));
            #endregion
        }
    }
}
