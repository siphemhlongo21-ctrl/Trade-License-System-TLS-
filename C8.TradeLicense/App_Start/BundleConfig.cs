using System.Web.Optimization;

namespace C8.TradeLicense
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.datetimepicker.js",
                  
                        "~/Scripts/jquery.cookie.js"));

       
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js" ));
           
            bundles.Add(new ScriptBundle("~/bundles/jqueryvalidate").Include(
                      "~/Scripts/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery-ui.unobtrusive-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/jquery.datetimepicker.css",
                      "~/Content/jqx.base.css",
                      "~/Content/PagedList.css",
                      "~/Content/Validation.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                "~/Content/themes/base/jquery.ui.core.css",
                "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new Bundle("~/Models/HtmlHelperExtensions.cs"));
        }
    }
}