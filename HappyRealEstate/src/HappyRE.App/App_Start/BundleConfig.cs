using HappyRE.Core.Utils;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace HappyRE.App
{
    public class BundleConfig
    {
        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
                throw new ArgumentNullException("ignoreList");
            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/jquery.highlight.js",
                        "~/Scripts/jquery.textcomplete.js"));

            bundles.Add(new ScriptBundle("~/bundles/tqtjs").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/jquery.highlight.js",
                        "~/Scripts/jquery.textcomplete.js",
                        "~/Scripts/jquery.signalR-2.4.2.min.js",
                        "~/Scripts/modernizr-*",
                        "~/Scripts/toastr.min.js",
                       "~/Scripts/alertify.min.js",
                       "~/Scripts/select2.min.js",
                       "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js",
                        "~/Scripts/proui/js/app.js",
                      "~/Scripts/proui/js/plugins.js",
                      "~/Scripts/imgviewer/jquery.magnify.min.js",
                        "~/Scripts/angular.min.js",
               "~/Scripts/angular-sanitize.min.js",
               "~/Scripts/ngStorage.min.js",
               "~/Scripts/angular-local-storage.js",
               "~/Scripts/app/app.js",
               "~/Scripts/app/restfulService.js"
               ).Include("~/Scripts/app/app.js")
                .Include("~/Scripts/app/restfulService.js")
                .IncludeDirectory("~/Scripts/app", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/printthis").Include(
                      "~/Scripts/printThis.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
               "~/Scripts/angular.min.js",
               "~/Scripts/angular-sanitize.min.js",
               "~/Scripts/ngStorage.min.js",
               "~/Scripts/angular-local-storage.js"));

            bundles.Add(new ScriptBundle("~/bundles/app")
                .Include("~/Scripts/app/app.js")
                .Include("~/Scripts/app/restfulService.js")
                .IncludeDirectory("~/Scripts/app", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/proui").Include(
                      "~/Scripts/proui/js/app.js",
                      "~/Scripts/proui/js/plugins.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                      "~/Scripts/toastr.min.js",
                       "~/Scripts/alertify.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                      "~/Scripts/select2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new StyleBundle("~/Content/toastr").Include(
                "~/Content/toastr.min.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/select2.min.css",
                      "~/Content/site.css",
                      "~/Content/alertify.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
           "~/Scripts/kendo/kendo.all.min.js",
                "~/Scripts/kendo/cultures/kendo.culture.vi-VN.min.js",
           "~/Scripts/kendo/kendo.aspnetmvc.min.js"));

            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
            "~/Content/kendo/kendo.common.min.css",
            "~/Content/kendo/kendo.bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/proui/css/styles").Include(
                "~/Content/proui/css/plugins.css",
                "~/Content/proui/css/main.css",
                "~/Content/proui/css/themes.css"));

            BundleTable.EnableOptimizations =  bool.Parse(ConfigSettings.Get("BUNDLE", "true"));
        }
    }
}

