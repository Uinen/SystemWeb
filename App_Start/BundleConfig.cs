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
            bundles.Add(new StyleBundle("~/Content/bootstrapV3", 
                ContentDeliveryNetwork.Gestionidirette.BootstrapUrl).Include(
                "~/Contenuti/Bootstrap/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrapV4",
                ContentDeliveryNetwork.Gestionidirette.Bootstrap4Url).Include(
                "~/Contenuti/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/style",
                ContentDeliveryNetwork.Gestionidirette.StyleUrl).Include(
                "~/Contenuti/css/style.min.css"));

            bundles.Add(new StyleBundle("~/Content/custom",
                ContentDeliveryNetwork.Gestionidirette.CustomUrl).Include(
                "~/Contenuti/css/colors/custom.min.css"));

            bundles.Add(new StyleBundle("~/Content/ionicons",
                ContentDeliveryNetwork.Gestionidirette.IonIconsUrl).Include(
                "~/Contenuti/css/ionicons.min.css"));

            bundles.Add(new StyleBundle("~/Content/animate",
                ContentDeliveryNetwork.Gestionidirette.AnimateUrl).Include(
                "~/Contenuti/css/animate.min.css"));

            bundles.Add(new StyleBundle("~/Content/owlcarousel",
                ContentDeliveryNetwork.Gestionidirette.OwlCarouselUrl).Include(
                "~/Contenuti/css/owl.carousel.min.css"));

            bundles.Add(new StyleBundle("~/Content/owlcarousel-theme",
                ContentDeliveryNetwork.Gestionidirette.OwlCarouselThemeUrl).Include(
                "~/Contenuti/css/owl.theme.min.css"));

            bundles.Add(new StyleBundle("~/Content/nivolightbox",
                ContentDeliveryNetwork.Gestionidirette.NivoLightBoxUrl).Include(
                "~/Contenuti/css/nivo-lightbox/nivo-lightbox.css"));

            bundles.Add(new StyleBundle("~/Content/fontawesome",
                ContentDeliveryNetwork.MaxCdn.FontAwesomeUrl).Include(
                      "~/MainFont/font-awesome.css" /*,rewrite*/));

            bundles.Add(new StyleBundle("~/Content/EjWidgets",
                ContentDeliveryNetwork.Gestionidirette.EjWidgetsUrl).Include(
                      "~/Contenuti/ej/web/ej.widgets.core.min.css"));

            bundles.Add(new StyleBundle("~/Content/EjTheme",
                ContentDeliveryNetwork.Gestionidirette.EjThemeUrl).Include(
                      "~/Contenuti/ej/web/default-theme/ej.theme.min.css"));

            bundles.Add(new StyleBundle("~/Content/EjResponsive",
                ContentDeliveryNetwork.Gestionidirette.EjResponsiveUrl).Include(
                      "~/Contenuti/ej/web/responsive-css/ej.responsive.min.css"));

            bundles.Add(new StyleBundle("~/Content/EjGridResponsive",
                ContentDeliveryNetwork.Gestionidirette.EjGridResponsiveUrl).Include(
                      "~/Contenuti/ej/web/responsive-css/ejgrid.responsive.min.css"));

            #endregion
        }

        private static void AddJavaScript(BundleCollection bundles)
        {
            #region Script

            bundles.Add(new ScriptBundle("~/bundles/jquery", 
                ContentDeliveryNetwork.Google.JQueryUrl).Include(
                "~/Scripts/jquery-1.10.2.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryeasing",
                ContentDeliveryNetwork.Cdnjs.JQueryEasingUrl).Include(
                "~/Scripts/jquery.easing-{version}.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryglobalize",
                ContentDeliveryNetwork.Gestionidirette.JQueryGlobalizeUrl).Include(
                "~/Scripts/jquery.globalize.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jsrender",
                ContentDeliveryNetwork.Gestionidirette.JsRender).Include(
                "~/Scripts/jsrender.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jquerybrowser",
                ContentDeliveryNetwork.Gestionidirette.JQueryBrowser).Include(
                "~/Scripts/jquery.browser.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajaxchimp",
                ContentDeliveryNetwork.Gestionidirette.JQueryAjaxChimp).Include(
                "~/Scripts/jquery.ajaxchimp.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval",
                ContentDeliveryNetwork.Microsoft.JQueryValidateUrl).Include(
                "~/Scripts/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalunobtrusive",
                ContentDeliveryNetwork.Microsoft.JQueryValidateUnobtrusiveUrl).Include(
                "~/Scripts/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/ej",
                ContentDeliveryNetwork.Gestionidirette.SyncFusionScriptsUrl).Include(
                "~/Scripts/ej/web/ej.web.all.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/ejunobtrusive",
                ContentDeliveryNetwork.Gestionidirette.EjUnobtrusiveUrl).Include(
                "~/Scripts/ej/common/ej.unobtrusive.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/microsoft").Include(
                "~/Scripts/MicrosoftAjax.js",
                "~/Scripts/MicrosoftMvcAjax.js",
                "~/Scripts/MicrosoftMvcValidation.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapV3",
                ContentDeliveryNetwork.Microsoft.BootstrapUrl).Include(
                "~/Scripts/bootstrap/v3/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapV4",
                ContentDeliveryNetwork.Microsoft.BootstrapUrl).Include(
                "~/Scripts/bootstrap/v4/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr",
                ContentDeliveryNetwork.Microsoft.ModernizrUrl).Include(
                "~/Scripts/modernizr-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/theme",
                ContentDeliveryNetwork.Gestionidirette.ThemeUrl).Include(
                "~/Scripts/Theme.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/sorting",
                ContentDeliveryNetwork.Gestionidirette.SortableUrl).Include(
                "~/Scripts/sortable.js"));

            bundles.Add(new ScriptBundle("~/bundles/GridOption",
                ContentDeliveryNetwork.Gestionidirette.ActionCompleteUrl).Include(
                "~/Scripts/ActionComplete.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/EjCulture",
                ContentDeliveryNetwork.Gestionidirette.EjCultureUrl).Include(
                "~/Scripts/ej/i18n/ej.culture.it-IT.min.js"));

            #endregion
        }
    }
}
