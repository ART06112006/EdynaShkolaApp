using System.Web;
using System.Web.Optimization;

namespace WebApplication_Students_Teachers_
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/teacher.index").Include(
                        "~/Scripts/teacher.index.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/logout").Include(
                        "~/Scripts/logout.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/confirm.password.external.login").Include(
                        "~/Scripts/confirm.password.external.login.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/confirm.password.reset.password").Include(
                        "~/Scripts/confirm.password.reset.password.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/confirm.password.register").Include(
                        "~/Scripts/confirm.password.register.js"));
        }
    }
}
